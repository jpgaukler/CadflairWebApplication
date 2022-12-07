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
        private static readonly string[] _uploadFileExtensions = { ".ipt", "iam", ".idw", ".dwg", ".zip"};

        public ProductController(ForgeServicesManager forgeServicesManager, DataServicesManager dataServicesManager)
        {
            _forgeServicesManager = forgeServicesManager;
            _dataServicesManager = dataServicesManager;
        }

        public class ProductUploadData
        {
            public string ProductSpec { get; set; } = string.Empty;
            public IFormFile? ZipFile { get; set; }
        }

        /// <summary>
        /// Receive a file from the client and upload to the bucket
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/product/create")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductUploadData productUploadData)
        {
            try
            {
                //validate data
                if (string.IsNullOrWhiteSpace(productUploadData.ProductSpec)) return ValidationProblem("No product information provided.");
                if (productUploadData.ZipFile == null) return ValidationProblem("No zip file was not provided.");
                if (productUploadData.ZipFile.Length == 0) return ValidationProblem("File does not contain any data.");
                if (Path.GetExtension(productUploadData.ZipFile.FileName) != ".zip") return ValidationProblem("Invalid file type provided.");
                //if (!_uploadFileExtensions.Any(i => i == Path.GetExtension(productUploadData.ZipFile.FileName))) return ValidationProblem("Invalid file type provided.");

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
                    await productUploadData.ZipFile.CopyToAsync(stream);
                }

                // Upload file to Autodesk Forge OSS 
                bool uploadSuccessful = await _forgeServicesManager.ObjectStorageService.UploadFileAsync(bucketKey.ToString(), objectKey.ToString(), tempFileName);

                // Delete the temp file from the server
                System.IO.File.Delete(tempFileName);

                if (!uploadSuccessful) return BadRequest(new { Error = $"Unable to upload to Autodesk Forge OSS." });

                // Create new Product record in the database
                Product newProduct = JsonSerializer.Deserialize<Product>(productUploadData.ProductSpec)!;
                newProduct.ForgeBucketKey = bucketKey;
                newProduct.Id = await _dataServicesManager.ProductService.CreateProduct(newProduct);

                // Create new ProductConfiguration record in the database for master configuration
                ProductConfiguration masterConfiguration = new()
                {
                    ProductId = newProduct.Id,
                    ArgumentJson = "",
                    ForgeObjectKey = objectKey,
                };

                masterConfiguration.Id = await _dataServicesManager.ProductService.CreateProductConfiguration(masterConfiguration);

                return Ok(new { Result = "Product uploaded successfully!"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"Exception occurred: {ex}" });
            }
        }


    }
}
