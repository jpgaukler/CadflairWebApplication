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
                if (string.IsNullOrWhiteSpace(form.ProductData)) return ValidationProblem("Parameter 'ArgumentJson' was not provided!");
                if (form.ZipFile == null || form.ZipFile.Length == 0) return ValidationProblem("No zip file was provided!");
                if (Path.GetExtension(form.ZipFile.FileName) != ".zip") return ValidationProblem("Invalid file type!");

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

                if (!uploadSuccessful) return BadRequest(new { Error = $"Unable to upload to Autodesk Forge OSS." });

                // Create new Product record in the database
                dynamic productData = JsonConvert.DeserializeObject<dynamic>(form.ProductData)!;

                Debug.WriteLine($@"UserId: {productData.UserId}");
                Debug.WriteLine($@"SubscriptionId: {productData.SubscriptionId}");
                Debug.WriteLine($@"ProductFolderId: {productData.ProductFolderId}");
                Debug.WriteLine($@"DisplayName: {productData.DisplayName}");
                Debug.WriteLine($@"ParameterJson: {productData.ParameterJson}");
                Debug.WriteLine($@"ArgumentJson: {productData.ArgumentJson}");
                Debug.WriteLine($@"IsPublic: {productData.IsPublic}");
                Debug.WriteLine($@"IsConfigurable: {productData.IsConfigurable}");

                Product newProduct = await _dataServicesManager.ProductService.CreateProduct(subscriptionId: (int)productData.SubscriptionId,
                                                                                             productFolderId: (int)productData.ProductFolderId,
                                                                                             displayName: (string)productData.DisplayName,
                                                                                             iLogicFormJson: (string)productData.ILogicFormJson,
                                                                                             forgeBucketKey: bucketKey,
                                                                                             createdById: (int)productData.UserId,
                                                                                             isPublic: (bool)productData.IsPublic,
                                                                                             isConfigurable: (bool)productData.IsConfigurable);

                // Create new ProductConfiguration record in the database for master configuration
                ProductConfiguration masterConfiguration = await _dataServicesManager.ProductService.CreateProductConfiguration(productId: newProduct.Id,
                                                                                                                                argumentJson: (string)productData.ArgumentJson,
                                                                                                                                forgeZipKey: objectKey,
                                                                                                                                isDefault: true);

                return Ok(new { Result = "Product uploaded successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Exception occurred: {ex}" });
            }
        }


    }
}
