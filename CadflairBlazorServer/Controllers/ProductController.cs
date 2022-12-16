using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairForgeAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Text.Json;

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

        public class ProductUploadForm
        {
            public int? UserId { get; set; } 
            public int? SubscriptionId { get; set; } 
            public int? ProductFolderId { get; set; }
            public string? DisplayName { get; set; }
            public string? ParameterJson { get; set; }
            public string? ArgumentJson { get; set; }
            public bool? IsPublic { get; set; } 
            public bool? IsConfigurable { get; set; } 
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
                if (form.SubscriptionId == null) return ValidationProblem("Parameter 'SubscriptionId' was not provided!");
                if (form.ProductFolderId == null) return ValidationProblem("Parameter 'ProductFolderId' was not provided!");
                if (form.UserId == null) return ValidationProblem("Parameter 'UserId' was not provided!");
                if (string.IsNullOrWhiteSpace(form.DisplayName)) return ValidationProblem("Parameter 'DisplayName' was not provided!");
                if (string.IsNullOrWhiteSpace(form.ParameterJson)) return ValidationProblem("Parameter 'ParameterJson' was not provided!");
                if (string.IsNullOrWhiteSpace(form.ArgumentJson)) return ValidationProblem("Parameter 'ArgumentJson' was not provided!");
                if (form.IsPublic == null) return ValidationProblem("Parameter 'IsPublic' was not provided!");
                if (form.IsConfigurable == null) return ValidationProblem("Parameter 'IsConfigurable' was not provided!");
                if (form.ZipFile == null || form.ZipFile.Length == 0) return ValidationProblem("No zip file was provided!");
                if (Path.GetExtension(form.ZipFile.FileName) != ".zip") return ValidationProblem("Invalid file type!");

                //// Upload check if greater than 2mb!
                //if (memoryStream.Length > 2097152)
                //{
                //}

                // Create bucket for product
                Guid bucketKey = Guid.NewGuid();
                Guid objectKey = Guid.NewGuid();
                await _forgeServicesManager.ObjectStorageService.CreateBucketAsync(bucketKey.ToString());

                // Temporarily save the file to server
                string tempFileName = Path.GetTempFileName();

                using (FileStream stream = System.IO.File.Create(tempFileName))
                {
                    await form.ZipFile.CopyToAsync(stream);
                }

                // Upload file to Autodesk Forge OSS 
                bool uploadSuccessful = await _forgeServicesManager.ObjectStorageService.UploadFileAsync(bucketKey.ToString(), objectKey.ToString(), tempFileName);

                // Delete the temp file from the server
                System.IO.File.Delete(tempFileName);

                if (!uploadSuccessful) return BadRequest(new { Error = $"Unable to upload to Autodesk Forge OSS." });

                // Create new Product record in the database
                Product newProduct = await _dataServicesManager.ProductService.CreateProduct(subscriptionId: (int)form.SubscriptionId,
                                                                                             productFolderId: (int)form.ProductFolderId,
                                                                                             displayName: form.DisplayName,
                                                                                             parameterJson: form.ParameterJson,
                                                                                             forgeBucketKey: bucketKey,
                                                                                             createdById: (int)form.UserId,
                                                                                             isPublic: (bool)form.IsPublic,
                                                                                             isConfigurable: (bool)form.IsConfigurable);

                // Create new ProductConfiguration record in the database for master configuration
                ProductConfiguration masterConfiguration = await _dataServicesManager.ProductService.CreateProductConfiguration(productId: newProduct.Id,
                                                                                                                                argumentJson: form.ArgumentJson,
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
