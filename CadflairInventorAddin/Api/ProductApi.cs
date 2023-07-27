using CadflairDataAccess.Models;
using CadflairInventorAddin.Helpers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CadflairInventorAddin.Api
{
    internal static class ProductApi
    {

        #region "Product"

        public static async Task<Product> CreateProduct(int userId, int subscriptionId, string displayName, string rootFileName, string iLogicFormJson, string argumentJson, bool isPublic, bool isConfigurable, string inventorZipName, string stpFileName, string viewablesZipName)
        {
            try
            {
                string uri = $"api/v1/products";

                dynamic productData = new
                {
                    UserId = userId,
                    SubscriptionId = subscriptionId,
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
                using (FileStream viewablesZipStream = File.Open(viewablesZipName, FileMode.Open))
                using (FileStream stpStream = File.Open(stpFileName, FileMode.Open))
                using (StreamContent inventorContent = new StreamContent(inventorZipStream))
                using (StreamContent viewablesContent = new StreamContent(viewablesZipStream))
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
                    formContent.Add(stpContent, "StpFile", Path.GetFileName(stpFileName));

                    // add viewables zip file the form as a stream content
                    viewablesContent.Headers.Add("Content-Type", "application/octet-stream");
                    viewablesContent.Headers.Add("Content-Disposition", $"form-data; name=\"ViewablesZipFile\"; filename=\"{Path.GetFileName(viewablesZipName)}\"");
                    formContent.Add(viewablesContent, "ViewablesZipFile", Path.GetFileName(viewablesZipName));

                    string result = await Client.Post(uri, formContent);
                    Product product = JsonConvert.DeserializeObject<Product>(result);

                    return product;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UploadProductToCadflair", userId, subscriptionId, displayName, rootFileName, iLogicFormJson, argumentJson, isPublic, isConfigurable, inventorZipName, viewablesZipName);
                return null;
            }
        }

        public static async Task<Product> GetProductBySubscriptionIdAndDisplayName(int subscriptionId, string displayName)
        {
            try
            {
                string uri = $"api/v1/products/{subscriptionId}/{displayName}";
                string result = await Client.Get(uri);
                Product product = JsonConvert.DeserializeObject<Product>(result);
                return product;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetProductBySubscriptionIdAndDisplayName", subscriptionId, displayName);
                return null;
            }
        }

        #endregion

        //#region "CatalogFolder"

        //public static async Task<List<CatalogFolder>> GetCatalogFoldersBySubscriptionId(int subscriptionId)
        //{
        //    try
        //    {
        //        string uri = $"api/v1/catalogFolders/{subscriptionId}";
        //        string result = await Client.Get(uri);
        //        List<CatalogFolder> folders = JsonConvert.DeserializeObject<List<CatalogFolder>>(result);
        //        return folders;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "GetCatalogFoldersBySubscriptionId", subscriptionId);
        //        return new List<CatalogFolder>();
        //    }
        //}

        //public static async Task<CatalogFolder> CreateCatalogFolder(int subscriptionId, int createdById, string displayName, int? parentId)
        //{
        //    try
        //    {
        //        string uri = $"api/v1/catalogFolders/{subscriptionId}/{createdById}/{displayName}/{parentId}";
        //        string result = await Client.Post(uri);
        //        CatalogFolder folder = JsonConvert.DeserializeObject<CatalogFolder>(result);
        //        return folder;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "CreateCatalogFolder", subscriptionId, createdById, displayName, parentId);
        //        return null;
        //    }
        //}

        //#endregion

    }
}
