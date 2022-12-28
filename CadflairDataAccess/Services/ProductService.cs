using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CadflairDataAccess.Services
{
    public class ProductService
    {

        private readonly DataAccess _db;

        public ProductService(DataAccess db)
        {
            _db = db;
        }

        #region "Product"

        public Task<Product> CreateProduct(int subscriptionId, int productFolderId, string displayName, string forgeBucketKey, bool isPublic, int createdById)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                ProductFolderId = productFolderId,
                DisplayName = displayName,
                SubdirectoryName =  Regex.Replace(displayName, "[^a-zA-Z0-9_.]+", string.Empty).ToLower(),
                ForgeBucketKey = forgeBucketKey,
                IsPublic = isPublic,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<Product, dynamic>("[dbo].[spProduct_Insert]", values);
        }

        public Task<Product> GetProductBySubscriptionIdAndSubdirectoryName(int subscriptionId, string subdirectoryName)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                SubdirectoryName = subdirectoryName,
            };

            return _db.LoadSingleAsync<Product, dynamic>("[dbo].[spProduct_GetBySubscriptionIdAndSubdirectoryName]", values);
        }

        public Task<Product> GetProductBySubscriptionIdAndDisplayName(int subscriptionId, string displayName)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                DisplayName = displayName,
            };

            return _db.LoadSingleAsync<Product, dynamic>("[dbo].[spProduct_GetBySubscriptionIdAndDisplayName]", values);
        }

        public Task<List<Product>> GetProductsByProductFolderId(int productFolderId)
        {
            return _db.LoadDataAsync<Product, dynamic>("[dbo].[spProduct_GetByProductFolderId]", new { ProductFolderId = productFolderId });
        }


        #endregion

        #region "ProductVersion"
        
        public Task<ProductVersion> CreateProductVersion(int productId, string rootFileName, string iLogicFormJson, bool isConfigurable, int createdById)
        {
            dynamic values = new
            {
                ProductId = productId,
                RootFileName = rootFileName,
                ILogicFormJson = iLogicFormJson,
                IsConfigurable = isConfigurable,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<ProductVersion, dynamic>("[dbo].[spProductVersion_Insert]", values);
        }
        
        public Task<ProductVersion> GetLatestProductVersionByProductId(int productId)
        {
            return _db.LoadSingleAsync<ProductVersion, dynamic>("[dbo].[spProductVersion_GetLatestByProductId]", new { ProductId = productId });
        }

        #endregion

        #region "ProductFolder"

        public Task<ProductFolder> GetProductFolderById(int id)
        {
            return _db.LoadSingleAsync<ProductFolder, dynamic>("[dbo].[spProductFolder_GetById]", new { Id = id });
        }

        public Task<List<ProductFolder>> GetProductFoldersBySubscriptionId(int subscriptionId)
        {
            return _db.LoadDataAsync<ProductFolder, dynamic>("[dbo].[spProductFolder_GetBySubscriptionId]", new { SubscriptionId = subscriptionId });
        }

        public Task<List<ProductFolder>> GetProductFoldersBySubscriptionIdAndParentId(int subscriptionId, int? parentId)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                ParentId = parentId,
            };

            return _db.LoadDataAsync<ProductFolder, dynamic>("[dbo].[spProductFolder_GetBySubscriptionIdAndParentId]", values);
        }

        public Task<ProductFolder> CreateProductFolder(int subscriptionId, int? parentId, string displayName, int createdById)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                ParentId = parentId,
                DisplayName = displayName,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<ProductFolder, dynamic>("[dbo].[spProductFolder_Insert]", values);
        }

        public Task DeleteProductFolder(Product productFolder)
        {
            return _db.SaveDataAsync("[dbo].[spProductFolder_DeleteById]", new { productFolder.Id });
        }

        #endregion

        #region "ProductConfiguration"

        public Task<ProductConfiguration> GetProductConfigurationById(int id)
        {
            return _db.LoadSingleAsync<ProductConfiguration, dynamic>("[dbo].[spProductConfiguration_GetById]", new { Id = id });
        }

        public Task<ProductConfiguration> CreateProductConfiguration(int productVersionId, string argumentJson, string forgeZipKey, bool isDefault)
        {
            dynamic values = new
            {
                ProductVersionId = productVersionId,
                IsDefault = isDefault,
                ArgumentJson = argumentJson,
                ForgeZipKey = forgeZipKey,
            };

            return _db.SaveSingleAsync<ProductConfiguration, dynamic>("[dbo].[spProductConfiguration_Insert]", values);
        }


        public Task<ProductConfiguration> GetDefaultProductConfigurationByProductVersionId(int productVersionId)
        {
            return _db.LoadSingleAsync<ProductConfiguration, dynamic>("[dbo].[spProductConfiguration_GetDefaultByProductVersionId]", new { ProductVersionId = productVersionId });
        }

        public Task UpdateProductConfiguration(ProductConfiguration productConfiguration)
        {
            dynamic values = new
            {
                productConfiguration.Id,
                productConfiguration.ProductVersionId,
                productConfiguration.IsDefault,
                productConfiguration.ArgumentJson,
                productConfiguration.ForgeZipKey,
                productConfiguration.ForgePdfKey,
                productConfiguration.ForgeDwgKey,
                productConfiguration.ForgeStpKey,
            };

            return _db.SaveDataAsync("[dbo].[spProductConfiguration_UpdateById]", values);
        }


        #endregion

    }
}

