using CadflairDataAccess.Models;
using CadflairInventorAddin.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CadflairInventorAddin.Api
{
    internal static class ProductApi
    {

        #region "Product"

        public static async Task<Product> CreateProduct(int userId, int subscriptionId, int productFolderId, string displayName, string rootFileName, string iLogicFormJson, string argumentJson, bool isPublic, bool isConfigurable, string zipFileName)
        {
            try
            {
                string uri = $"/api/product/create";

                dynamic productData = new
                {
                    UserId = userId,
                    SubscriptionId = subscriptionId,
                    ProductFolderId = productFolderId,
                    DisplayName = displayName,
                    RootFileName = rootFileName,
                    ILogicFormJson = iLogicFormJson,
                    ArgumentJson = argumentJson,
                    IsPublic = isPublic,
                    IsConfigurable = isConfigurable,
                };

                using (MultipartFormDataContent formContent = new MultipartFormDataContent())
                using (StringContent productDataContent = new StringContent(JsonConvert.SerializeObject(productData)))
                using (FileStream stream = System.IO.File.Open(zipFileName, FileMode.Open))
                using (StreamContent streamContent = new StreamContent(stream))
                {
                    // add product data to request
                    productDataContent.Headers.Add("Content-Disposition", "form-data; name=\"ProductData\"");
                    formContent.Add(productDataContent, "ProductData");

                    // add file the form as a stream content
                    streamContent.Headers.Add("Content-Type", "application/octet-stream");
                    streamContent.Headers.Add("Content-Disposition", $"form-data; name=\"ZipFile\"; filename=\"{System.IO.Path.GetFileName(zipFileName)}\"");
                    formContent.Add(streamContent, "ZipFile", System.IO.Path.GetFileName(zipFileName));

                    string result = await Client.Post(uri, formContent);
                    Product product = JsonConvert.DeserializeObject<Product>(result);
                    return product;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UploadProductToCadflair");
                return null;
            }
        }

        #endregion

        #region "ProductFolder"

        public static async Task<List<ProductFolder>> GetProductFoldersBySubscriptionIdAndParentId(int subscriptionId, int? parentId)
        {
            try
            {
                string uri = $"/api/productfolder/get/{subscriptionId}/{parentId}";
                string result = await Client.Get(uri);
                List<ProductFolder> folders = JsonConvert.DeserializeObject<List<ProductFolder>>(result);
                return folders;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetProductFoldersBySubscriptionIdAndParentId", subscriptionId, parentId);
                return new List<ProductFolder>();
            }
        }

        public static async Task<ProductFolder> CreateProductFolder(int subscriptionId, int createdById, string displayName, int? parentId)
        {
            try
            {
                string uri = $"/api/productfolder/create/{subscriptionId}/{createdById}/{displayName}/{parentId}";
                string result = await Client.Post(uri);
                ProductFolder folder  = JsonConvert.DeserializeObject<ProductFolder>(result);
                return folder;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CreateProductFolder", subscriptionId, createdById, displayName, parentId);
                return null;
            }
        }

        #endregion

    }
}
