using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.IO.Compression;

namespace CadflairBlazorServer.Controllers
{
    [Authorize]
    [RequiredScope("User.InventorAddin")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ForgeServicesManager _forgeServicesManager;
        private readonly DataServicesManager _dataServicesManager;
        private readonly FileHandlingService _fileHandlingService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ForgeServicesManager forgeServicesManager, DataServicesManager dataServicesManager, FileHandlingService fileHandlingService, ILogger<ProductController> logger)
        {
            _forgeServicesManager = forgeServicesManager;
            _dataServicesManager = dataServicesManager;
            _fileHandlingService = fileHandlingService;
            _logger = logger;
        }

        #region "Product"

        public class ProductUploadForm
        {
            public string? ProductData { get; set; }
            public IFormFile? InventorZipFile { get; set; }
            public IFormFile? SvfZipFile { get; set; }
            public IFormFile? StpFile { get; set; }
        }

        /// <summary>
        /// Receive a file from the client and upload to the bucket
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/products")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductUploadForm form)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(form.ProductData)) 
                    return ValidationProblem($"Parameter '{nameof(ProductUploadForm.ProductData)}' was not provided!");

                if (form.InventorZipFile == null || form.InventorZipFile.Length == 0 ||
                    form.SvfZipFile == null || form.SvfZipFile.Length == 0 ||
                    form.StpFile == null || form.StpFile.Length == 0)
                    return ValidationProblem("Required files were not provided!");

                if (Path.GetExtension(form.InventorZipFile.FileName) != ".zip" || 
                    Path.GetExtension(form.SvfZipFile.FileName) != ".zip" ||
                    Path.GetExtension(form.StpFile.FileName) != ".stp") 
                    return ValidationProblem("Invalid file type!");

                if (form.InventorZipFile.Length > FileHandlingService.MAX_UPLOAD_SIZE ||
                    form.SvfZipFile.Length > FileHandlingService.MAX_UPLOAD_SIZE ||
                    form.StpFile.Length > FileHandlingService.MAX_UPLOAD_SIZE)
                    return ValidationProblem($"File exceeds maximum file size ({FileHandlingService.MAX_UPLOAD_SIZE/1000000} MB)!");

                // NEED TO VALIDATE INPUTS!!!!!!!!!!!!!!!
                dynamic productData = JsonConvert.DeserializeObject<dynamic>(form.ProductData)!;
                int userId = (int)productData.UserId;
                int subscriptionId = (int)productData.SubscriptionId;
                int productFolderId = (int)productData.ProductFolderId;
                string displayName = (string)productData.DisplayName;
                string iLogicFormJson = (string)productData.ILogicFormJson;
                string argumentJson = (string)productData.ArgumentJson;
                string rootFileName = (string)productData.RootFileName;
                bool isPublic = (bool)productData.IsPublic;
                bool isConfigurable = (bool)productData.IsConfigurable;

                // check to see if product already exists
                Product product = await _dataServicesManager.ProductService.GetProductBySubscriptionIdAndDisplayName(subscriptionId, displayName);

                if (product == null)
                {
                    // Create new Product
                    product = await _dataServicesManager.ProductService.CreateProduct(subscriptionId: subscriptionId,
                                                                                      productFolderId: productFolderId,
                                                                                      displayName: displayName,
                                                                                      isPublic: isPublic,
                                                                                      createdById: userId);
                }

                // Create new ProductVersion
                ProductVersion productVersion = await _dataServicesManager.ProductService.CreateProductVersion(productId: product.Id,
                                                                                                               rootFileName: rootFileName,
                                                                                                               iLogicFormJson: iLogicFormJson,
                                                                                                               isConfigurable: isConfigurable,
                                                                                                               createdById: userId);

                // Create default ProductConfiguration 
                ProductConfiguration productConfiguration = await _dataServicesManager.ProductService.CreateProductConfiguration(productVersionId: productVersion.Id,
                                                                                                                                 argumentJson: argumentJson,
                                                                                                                                 isDefault: true);
                // setup bucket for file uploads
                string bucketKey = Guid.NewGuid().ToString();
                await _forgeServicesManager.ObjectStorageService.CreateBucket(bucketKey);

                #region "upload inventor file"

                // Temporarily save the inventor zip file to server
                string tempFileName = await _fileHandlingService.UploadFormFileToTempFolder(form.InventorZipFile);

                // Upload inventor zip to Autodesk Forge OSS 
                string zipObjectKey = bucketKey + ".zip";
                bool inventorUploadSuccessful = await _forgeServicesManager.ObjectStorageService.UploadFile(bucketKey, zipObjectKey, tempFileName);

                // Delete the temp file from the server
                System.IO.File.Delete(tempFileName);

                if (!inventorUploadSuccessful)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Error = $"Unable to upload inventor files to Autodesk OSS." });

                #endregion

                #region "upload stp file"

                // Temporarily save the inventor zip file to server
                string tempStpName = await _fileHandlingService.UploadFormFileToTempFolder(form.StpFile);

                // Upload inventor zip to Autodesk Forge OSS 
                string stpObjectKey = bucketKey + ".stp";
                bool stpUploadSuccessful = await _forgeServicesManager.ObjectStorageService.UploadFile(bucketKey, stpObjectKey, tempStpName);

                // Delete the temp file from the server
                System.IO.File.Delete(tempStpName);

                if (!stpUploadSuccessful)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Error = $"Unable to upload stp file to Autodesk OSS." });

                #endregion

                #region "upload svf files"

                // Temporarily save the svf zip file to server
                string tempSvfName = await _fileHandlingService.UploadFormFileToTempFolder(form.SvfZipFile);

                // unzip files and upload all svf fragments to OSS
                DirectoryInfo svfExtractDirectory = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(tempSvfName)!, $"{Path.GetFileName(tempSvfName)}_extract"));
                ZipFile.ExtractToDirectory(tempSvfName, svfExtractDirectory.FullName);

                // upload all svf files to OSS
                bool svfUploadSuccessful = false;
                foreach (FileInfo fileInfo in svfExtractDirectory.GetFiles())
                {
                    // there is a log file in the svf output that is usaully empty, which causes the Forge library to throw an exception 
                    if (fileInfo.Length == 0)
                        continue;

                    // add slashes back into object names so the svf filenames match the original svf folder structure
                    string objectKey = Path.GetFileName(fileInfo.FullName).Replace("%2F", "/");
                    svfUploadSuccessful = await _forgeServicesManager.ObjectStorageService.UploadFile(bucketKey, objectKey, fileInfo.FullName);
                }

                // Delete temp files
                System.IO.File.Delete(tempSvfName);
                svfExtractDirectory.Delete(true);

                if (!svfUploadSuccessful)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Error = $"Unable to upload svf files to Autodesk OSS." });

                #endregion

                // update product configuration
                productConfiguration.BucketKey = bucketKey;
                productConfiguration.ZipObjectKey = zipObjectKey;
                productConfiguration.StpObjectKey = stpObjectKey;
                await _dataServicesManager.ProductService.UpdateProductConfiguration(productConfiguration);


                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unknown error occurred!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion


        #region "ProductFolder"

        [HttpPost]
        [Route("api/v1/productfolders/{subscriptionId:int}/{createdById:int}/{displayName}/{parentId:int?}")]
        public async Task<IActionResult> CreateProductFolder(int subscriptionId, int createdById, string displayName, int? parentId)
        {
            try
            {
                ProductFolder folder = await _dataServicesManager.ProductService.CreateProductFolder(subscriptionId, createdById, displayName, parentId);
                return Ok(folder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unknown error occurred!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/v1/productfolders/{subscriptionId:int}/{parentId:int?}")]
        public async Task<IActionResult> GetProductFoldersBySubscriptionIdAndParentId(int subscriptionId, int? parentId)
        {
            try
            {
                List<ProductFolder> folders = await _dataServicesManager.ProductService.GetProductFoldersBySubscriptionIdAndParentId(subscriptionId, parentId);
                return Ok(folders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unknown error occurred!");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

    }
}
