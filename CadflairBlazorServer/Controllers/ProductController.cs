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
            public IFormFile? ViewablesZipFile { get; set; }
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
                    form.ViewablesZipFile == null || form.ViewablesZipFile.Length == 0 ||
                    form.StpFile == null || form.StpFile.Length == 0)
                    return ValidationProblem("Required files were not provided!");

                if (Path.GetExtension(form.InventorZipFile.FileName) != ".zip" || 
                    Path.GetExtension(form.ViewablesZipFile.FileName) != ".zip" ||
                    Path.GetExtension(form.StpFile.FileName) != ".stp") 
                    return ValidationProblem("Invalid file type!");

                if (form.InventorZipFile.Length > FileHandlingService.MAX_UPLOAD_SIZE ||
                    form.ViewablesZipFile.Length > FileHandlingService.MAX_UPLOAD_SIZE ||
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

                // Temporarily save files to server
                string inventorFileName = await _fileHandlingService.UploadFormFileToTempFolder(form.InventorZipFile);
                string stpFileName = await _fileHandlingService.UploadFormFileToTempFolder(form.StpFile);
                string viewablesZipName = await _fileHandlingService.UploadFormFileToTempFolder(form.ViewablesZipFile);

                // unzip viewables folder
                DirectoryInfo extractDirectory = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(viewablesZipName)!, $"{Path.GetFileName(viewablesZipName)}_extract"));
                ZipFile.ExtractToDirectory(viewablesZipName, extractDirectory.FullName);

                // setup bucket for file uploads
                string bucketKey = Guid.NewGuid().ToString();
                await _forgeServicesManager.ObjectStorageService.CreateBucket(bucketKey);

                // Upload files to OSS
                List<Task<bool>> ossTasks = new();
                string zipObjectKey = bucketKey + ".zip";
                string stpObjectKey = bucketKey + ".stp";
                ossTasks.Add(_forgeServicesManager.ObjectStorageService.UploadFile(bucketKey, zipObjectKey, inventorFileName));
                ossTasks.Add(_forgeServicesManager.ObjectStorageService.UploadFile(bucketKey, stpObjectKey, stpFileName));

                foreach (FileInfo fileInfo in extractDirectory.GetFiles())
                {
                    // there is a log file in the svf output that is usaully empty, which causes the Forge library to throw an exception 
                    if (fileInfo.Length == 0)
                        continue;

                    // add slashes back into object names so the svf filenames match the original svf folder structure
                    string objectKey = Path.GetFileName(fileInfo.FullName).Replace("%2F", "/");
                    ossTasks.Add(_forgeServicesManager.ObjectStorageService.UploadFile(bucketKey, objectKey, fileInfo.FullName));
                }

                // wait for all uploads to complete, then check results
                var ossResults = await Task.WhenAll(ossTasks.ToArray());

                // Delete the temp files from the server
                System.IO.File.Delete(inventorFileName);
                System.IO.File.Delete(stpFileName);
                System.IO.File.Delete(viewablesZipName);
                extractDirectory.Delete(true);

                if (ossResults.Any(i => i == false))
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Error = $"Failed to upload files to Autodesk OSS." });

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


        [HttpGet]
        [Route("api/v1/products/{subscriptionId:int}/{displayName}")]
        public async Task<IActionResult> GetProductBySubscriptionIdAndDisplayName(int subscriptionId, string displayName)
        {
            try
            {
                Product product = await _dataServicesManager.ProductService.GetProductBySubscriptionIdAndDisplayName(subscriptionId, displayName);
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
