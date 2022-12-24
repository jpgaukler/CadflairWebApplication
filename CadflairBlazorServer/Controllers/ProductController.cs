using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairForgeAccess;
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

        //[HttpPost]
        //[Route("api/productfolder/create")]
        //public async Task<IActionResult> CreateProductFolder(int subscriptionId, int parentId, string displayName, int createdById)
        //{
        //    try
        //    {
        //        //validate data
        //        if (subscriptionId == null) return ValidationProblem("Parameter 'SubscriptionId' was not provided!");
        //        if (parentId == null) return ValidationProblem("Parameter 'ProductFolderId' was not provided!");
        //        if (createdById == null) return ValidationProblem("Parameter 'UserId' was not provided!");
        //        if (string.IsNullOrWhiteSpace(displayName)) return ValidationProblem("Parameter 'DisplayName' was not provided!");

        //        // Create new Product record in the database
        //        ProductFolder newProductFolder = await _dataServicesManager.ProductService.CreateProductFolder(subscriptionId, parentId, displayName, createdById);

        //        return Ok(new { Result = "ProductFolder created successfully!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Error = $"Exception occurred: {ex}" });
        //    }
        //}


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

                Debug.WriteLine($@"UserId: {productData.UserId}");
                Debug.WriteLine($@"SubscriptionId: {productData.SubscriptionId}");
                Debug.WriteLine($@"ProductFolderId: {productData.ProductFolderId}");
                Debug.WriteLine($@"DisplayName: {productData.DisplayName}");
                Debug.WriteLine($@"ArgumentJson: {productData.ArgumentJson}");
                Debug.WriteLine($@"IsPublic: {productData.IsPublic}");
                Debug.WriteLine($@"IsConfigurable: {productData.IsConfigurable}");


                //// Upload check if greater than 2mb!
                //if (memoryStream.Length > 2097152)
                //{
                //}

                // Create bucket for product
                Guid bucketKey = Guid.NewGuid();
                Guid objectKey = Guid.NewGuid();
                await _forgeServicesManager.ObjectStorageService.CreateBucket(bucketKey.ToString());

                // Temporarily save the file to server
                string tempFileName = Path.GetTempFileName();

                using (FileStream stream = System.IO.File.Create(tempFileName))
                {
                    await form.ZipFile.CopyToAsync(stream);
                }

                // Upload file to Autodesk Forge OSS 
                bool uploadSuccessful = await _forgeServicesManager.ObjectStorageService.UploadFile(bucketKey.ToString(), objectKey.ToString(), tempFileName);

                // Delete the temp file from the server
                System.IO.File.Delete(tempFileName);

                if (!uploadSuccessful) return BadRequest(new { Error = $"Unable to upload files to Autodesk Forge OSS." });

                // start the Model Derivative translation so the default configuration is viewable in the browser
                var forgeObject = await _forgeServicesManager.ObjectStorageService.GetObjectDetails(bucketKey.ToString(), objectKey.ToString());
                var tranlationJob = _forgeServicesManager.ModelDerivativeService.TranslateObject(forgeObject.encoded_urn, rootFileName);
                Debug.WriteLine($@"Model derivative translation started:");
                Debug.WriteLine($@"{tranlationJob}");

                //need to check the status of the job that was posted

                // check to see if product already exists, create a new product if no match is found
                Product product = await _dataServicesManager.ProductService.GetProductBySubscriptionIdAndDisplayName(subscriptionId, displayName);

                if(product == null)
                {
                    // Create new Product
                    product = await _dataServicesManager.ProductService.CreateProduct(subscriptionId, productFolderId, displayName, forgeBucketKey: bucketKey, isPublic, createdById: userId);
                    Debug.WriteLine($@"Created new product: {product.DisplayName}");
                }

                // Create new ProductVersion
                ProductVersion productVersion = await _dataServicesManager.ProductService.CreateProductVersion(productId: product.Id, rootFileName, iLogicFormJson, isConfigurable, createdById: userId);
                Debug.WriteLine($@"Created new product version: {product.DisplayName} Version: {productVersion.VersionNumber}");

                // Create default ProductConfiguration 
                ProductConfiguration productConfiguration = await _dataServicesManager.ProductService.CreateProductConfiguration(productVersionId: productVersion.Id, argumentJson, forgeZipKey: objectKey, isDefault: true);
                Debug.WriteLine($@"Created new product configuration: {productConfiguration.Id}");

                return Ok(new { Result = "Product uploaded successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Exception occurred: {ex}" });
            }
        }


    }
}
