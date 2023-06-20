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

        public static async Task<Product> CreateProduct(int userId, int subscriptionId, int productFolderId, string displayName, string rootFileName, string iLogicFormJson, string argumentJson, bool isPublic, bool isConfigurable, string inventorZipName, string stpFileName, string svfZipName)
        {
            try
            {
                string uri = $"api/v1/products";

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
                using (FileStream inventorZipStream = File.Open(inventorZipName, FileMode.Open))
                using (FileStream svfZipStream = File.Open(svfZipName, FileMode.Open))
                using (FileStream stpStream = File.Open(stpFileName, FileMode.Open))
                using (StreamContent inventorContent = new StreamContent(inventorZipStream))
                using (StreamContent svfContent = new StreamContent(svfZipStream))
                using (StreamContent stpContent = new StreamContent(stpStream))
                {
                    // add product data to request
                    productDataContent.Headers.Add("Content-Disposition", "form-data; name=\"ProductData\"");
                    formContent.Add(productDataContent, "ProductData");

                    // add inventor zip file the form as a stream content
                    inventorContent.Headers.Add("Content-Type", "application/octet-stream");
                    inventorContent.Headers.Add("Content-Disposition", $"form-data; name=\"InventorZipFile\"; filename=\"{Path.GetFileName(inventorZipName)}\"");
                    formContent.Add(inventorContent, "InventorZipFile", Path.GetFileName(inventorZipName));

                    // add stp file the form as a stream content
                    stpContent.Headers.Add("Content-Type", "application/octet-stream");
                    stpContent.Headers.Add("Content-Disposition", $"form-data; name=\"StpFile\"; filename=\"{Path.GetFileName(stpFileName)}\"");
                    formContent.Add(stpContent, "StpFile", Path.GetFileName(svfZipName));

                    // add svf zip file the form as a stream content
                    svfContent.Headers.Add("Content-Type", "application/octet-stream");
                    svfContent.Headers.Add("Content-Disposition", $"form-data; name=\"SvfZipFile\"; filename=\"{Path.GetFileName(svfZipName)}\"");
                    formContent.Add(svfContent, "SvfZipFile", Path.GetFileName(svfZipName));

                    string result = await Client.Post(uri, formContent);
                    Product product = JsonConvert.DeserializeObject<Product>(result);

                    return product;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UploadProductToCadflair", userId, subscriptionId, displayName, rootFileName, iLogicFormJson, argumentJson, isPublic, isConfigurable, inventorZipName, svfZipName);
                return null;
            }
        }

        #endregion

        #region "ProductFolder"

        public static async Task<List<ProductFolder>> GetProductFoldersBySubscriptionIdAndParentId(int subscriptionId, int? parentId)
        {
            try
            {
                string uri = $"api/v1/productfolders/{subscriptionId}/{parentId}";
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
                string uri = $"api/v1/productfolders/{subscriptionId}/{createdById}/{displayName}/{parentId}";
                string result = await Client.Post(uri);
                ProductFolder folder = JsonConvert.DeserializeObject<ProductFolder>(result);
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
