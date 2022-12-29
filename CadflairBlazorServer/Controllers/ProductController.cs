using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairForgeAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CadflairBlazorServer.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ForgeServicesManager _forgeServicesManager;
        private readonly DataServicesManager _dataServicesManager;

        public ProductController(ForgeServicesManager forgeServicesManager, DataServicesManager dataServicesManager)
        {
            _forgeServicesManager = forgeServicesManager;
            _dataServicesManager = dataServicesManager;
        }

        #region "Product"

        public class ProductUploadForm
        {
            public string? ProductData { get; set; }
            public IFormFile? ZipFile { get; set; }
        }

        /// <summary>
        /// Receive a file from the client and upload to the bucket
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/product/create")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductUploadForm form)
        {
            try
            {
                //validate data
                if (string.IsNullOrWhiteSpace(form.ProductData)) return ValidationProblem("Parameter 'ProductData' was not provided!");
                if (form.ZipFile == null || form.ZipFile.Length == 0) return ValidationProblem("No zip file was provided!");
                if (Path.GetExtension(form.ZipFile.FileName) != ".zip") return ValidationProblem("Invalid file type!");


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

                //Debug.WriteLine($@"UserId: {productData.UserId}");
                //Debug.WriteLine($@"SubscriptionId: {productData.SubscriptionId}");
                //Debug.WriteLine($@"ProductFolderId: {productData.ProductFolderId}");
                //Debug.WriteLine($@"DisplayName: {productData.DisplayName}");
                //Debug.WriteLine($@"ArgumentJson: {productData.ArgumentJson}");
                //Debug.WriteLine($@"IsPublic: {productData.IsPublic}");
                //Debug.WriteLine($@"IsConfigurable: {productData.IsConfigurable}");

                //// Upload check if greater than 2mb!
                //if (memoryStream.Length > 2097152)
                //{
                //}

                // check to see if product already exists
                Product product = await _dataServicesManager.ProductService.GetProductBySubscriptionIdAndDisplayName(subscriptionId, displayName);

                // setup bucket for upload
                string bucketKey;
                if (product == null)
                {
                    // Create bucket for product new product
                    bucketKey = Guid.NewGuid().ToString();
                    await _forgeServicesManager.ObjectStorageService.CreateBucket(bucketKey);
                }
                else
                {
                    // use existing bucket for upload
                    bucketKey = product.ForgeBucketKey;
                }

                // Temporarily save the file to server
                string tempFileName = Path.GetTempFileName();

                using (FileStream stream = System.IO.File.Create(tempFileName))
                {
                    await form.ZipFile.CopyToAsync(stream);
                }

                // Upload file to Autodesk Forge OSS 
                string objectKey = Guid.NewGuid().ToString() + ".zip";
                bool uploadSuccessful = await _forgeServicesManager.ObjectStorageService.UploadFile(bucketKey, objectKey, tempFileName);

                // Delete the temp file from the server
                System.IO.File.Delete(tempFileName);

                if (!uploadSuccessful) return BadRequest(new { Error = $"Unable to upload files to Autodesk Forge OSS." });

                // start the Model Derivative translation so the default configuration is viewable in the browser
                var tranlationJob = await _forgeServicesManager.ModelDerivativeService.TranslateObject(bucketKey, objectKey, rootFileName);
                Debug.WriteLine($@"Model derivative translation started.");

                if (product == null)
                {
                    // Create new Product
                    product = await _dataServicesManager.ProductService.CreateProduct(subscriptionId: subscriptionId,
                                                                                      productFolderId: productFolderId,
                                                                                      displayName: displayName,
                                                                                      forgeBucketKey: bucketKey,
                                                                                      isPublic: isPublic,
                                                                                      createdById: userId);

                    Debug.WriteLine($@"Created new product: {product.DisplayName}");
                }

                // Create new ProductVersion
                ProductVersion productVersion = await _dataServicesManager.ProductService.CreateProductVersion(productId: product.Id,
                                                                                                               rootFileName: rootFileName,
                                                                                                               iLogicFormJson: iLogicFormJson,
                                                                                                               isConfigurable: isConfigurable,
                                                                                                               createdById: userId);

                Debug.WriteLine($@"Created new product version: {product.DisplayName} Version: {productVersion.VersionNumber}");

                // Create default ProductConfiguration 
                ProductConfiguration productConfiguration = await _dataServicesManager.ProductService.CreateProductConfiguration(productVersionId: productVersion.Id,
                                                                                                                                 argumentJson: argumentJson,
                                                                                                                                 forgeZipKey: objectKey,
                                                                                                                                 isDefault: true);
                Debug.WriteLine($@"Created new product configuration: {productConfiguration.Id}");

                return Ok("Product created successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Product creation failed: {ex}" });
            }
        }

        #endregion



        #region "ProductFolder"

        [Authorize]
        [HttpPost]
        [Route("api/productfolder/create/{subscriptionId:int}/{createdById:int}/{displayName}/{parentId:int?}")]
        public async Task<IActionResult> CreateProductFolder(int subscriptionId, int createdById, string displayName, int? parentId)
        {
            try
            {
                ProductFolder folder = await _dataServicesManager.ProductService.CreateProductFolder(subscriptionId, createdById, displayName, parentId);
                return Ok(folder);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Exception occurred: {ex}" });
            }
        }


        [HttpGet]
        [Route("api/productfolder/get/{subscriptionId:int}/{parentId:int?}")]
        public async Task<IActionResult> GetProductFoldersBySubscriptionIdAndParentId(int subscriptionId, int? parentId)
        {
            try
            {
                List<ProductFolder> folders = await _dataServicesManager.ProductService.GetProductFoldersBySubscriptionIdAndParentId(subscriptionId, parentId);
                return Ok(folders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Exception occurred: {ex}" });
            }
        }

        #endregion

    }
}
