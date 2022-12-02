using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairForgeAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace CadflairBlazorServer.Controllers
{
    [ApiController]
    public class UploadProductController : ControllerBase
    {
        private readonly ForgeServicesManager _forgeServicesManager;
        private readonly DataServicesManager _dataServicesManager;
        private static readonly string[] _uploadFileExtensions = { ".ipt", "iam", ".idw", ".dwg", ".zip"};

        public UploadProductController(ForgeServicesManager forgeServicesManager, DataServicesManager dataServicesManager)
        {
            _forgeServicesManager = forgeServicesManager;
            _dataServicesManager = dataServicesManager;
        }

        public class ProductUploadData
        {
            public int UserId { get; set; } 
            public int ProductFamilyId { get; set; } 
            public string DisplayName { get; set; } = string.Empty;
            public string ParameterJson { get; set; } = string.Empty;
            public IFormFile? ZipFile { get; set; }
        }

        /// <summary>
        /// Receive a file from the client and upload to the bucket
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/forge/product/upload")]
        public async Task<IActionResult> UploadProduct([FromForm] ProductUploadData productUploadData)
        {
            try
            {
                //validate data
                if (productUploadData.ZipFile == null) throw new Exception("No zip file was not provided.");
                if (Path.GetExtension(productUploadData.ZipFile.FileName) != ".zip") throw new Exception("Invalid file type provided.");
                //if (!_uploadFileExtensions.Any(i => i == Path.GetExtension(productUploadData.ZipFile.FileName))) throw new Exception("Invalid file type provided.");
                if (productUploadData.ZipFile.Length == 0) throw new Exception("File does not contain any data.");

                //string bucketKey = fileUploadData.bucketKey.ToLower();
                //string objectKey = fileUploadData.objectName;
                string bucketKey = Guid.NewGuid().ToString();
                string objectKey = Guid.NewGuid().ToString();

                Debug.WriteLine($"BucketKey: {bucketKey} ObjectKey: {objectKey}");

                //// Upload check if less than 2mb!
                //if (memoryStream.Length < 2097152)
                //{
                //}
                //else
                //{
                //}

                // Create bucket for file (in case is does not exist) 
                // I actaully think i will want to get the bucket key from the product family record
                await _forgeServicesManager.ObjectStorageService.CreateBucketAsync(bucketKey);

                // Temporarily save the file to server
                string tempFileName = Path.GetTempFileName();

                using (FileStream stream = System.IO.File.Create(tempFileName))
                {
                    await productUploadData.ZipFile.CopyToAsync(stream);
                }

                // Upload file to forge oss
                await _forgeServicesManager.ObjectStorageService.UploadFileAsync(bucketKey, objectKey, tempFileName);

                // Delete the temp file from the server
                System.IO.File.Delete(tempFileName);

                // Create new record in the database
                Product newProduct = new()
                {
                    ProductFamilyId = productUploadData.ProductFamilyId,
                    DisplayName = productUploadData.DisplayName,
                    ParameterJson = productUploadData.ParameterJson,
                    ForgeBucketKey = Guid.NewGuid(),
                    ForgeObjectKey = Guid.NewGuid(),
                    CreatedById = 1,
                    IsPublic = true,
                    IsConfigurable = false,
                };

                Debug.WriteLine($"Display Name: {newProduct.DisplayName} BucketKey: {newProduct.ForgeBucketKey} ObjectKey: {newProduct.ForgeObjectKey}");

                await _dataServicesManager.ProductService.CreateProduct(newProduct);

                //return the result
                //return Ok(new { BucketKey = bucketKey, ObjectName = objectKey, Error = result.Error.ToString(), Response = result.completed.ToString() });
                //return Ok(new { Product = newProduct.DisplayName, ParameterJson = newProduct.ParameterJson });
                return Ok(new { Result = "Upload successful"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Failed to upload model to Cadflair: {ex}" });
            }
        }


    }
}
