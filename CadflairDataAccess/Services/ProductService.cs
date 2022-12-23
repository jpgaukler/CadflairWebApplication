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

        public Task<Product> GetProductById(int id)
        {
            return _db.LoadSingleAsync<Product, dynamic>("[dbo].[spProduct_GetById]", new { Id = id });
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

        public Task<List<Product>> GetProductsByProductFolderId(int productFolderId)
        {
            return _db.LoadDataAsync<Product, dynamic>("[dbo].[spProduct_GetByProductFolderId]", new { ProductFolderId = productFolderId });
        }

        public Task<Product> CreateProduct(int subscriptionId, int productFolderId, string displayName, string iLogicFormJson, Guid forgeBucketKey, int createdById, bool isPublic, bool isConfigurable)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                ProductFolderId = productFolderId,
                DisplayName = displayName,
                SubdirectoryName =  Regex.Replace(displayName, "[^a-zA-Z0-9_.]+", string.Empty).ToLower(),
                ILogicFormJson = iLogicFormJson,
                ForgeBucketKey = forgeBucketKey,
                CreatedById = createdById,
                IsPublic = isPublic,
                IsConfigurable = isConfigurable
            };

            return _db.SaveSingleAsync<Product, dynamic>("[dbo].[spProduct_Insert]", values);
        }

        public Task DeleteProduct(Product product)
        {
            return _db.SaveDataAsync("[dbo].[spProduct_DeleteById]", new { product.Id });
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

        public Task<ProductConfiguration> GetDefaultProductConfigurationByProductId(int productId)
        {
            return _db.LoadSingleAsync<ProductConfiguration, dynamic>("[dbo].[spProductConfiguration_GetDefaultByProductId]", new { ProductId = productId });
        }

        public Task<List<ProductConfiguration>> GetProductConfigurationsByProductId(int productId)
        {
            return _db.LoadDataAsync<ProductConfiguration, dynamic>("[dbo].[spProductConfiguration_GetByProductId]", new { ProductId = productId });
        }

        public Task<ProductConfiguration> CreateProductConfiguration(int productId, string argumentJson, Guid forgeZipKey, bool isDefault)
        {
            dynamic values = new
            {
                ProductId = productId,
                IsDefault = isDefault,
                ArgumentJson = argumentJson,
                ForgeZipKey = forgeZipKey,
            };

            return _db.SaveSingleAsync<ProductConfiguration, dynamic>("[dbo].[spProductConfiguration_Insert]", values);
        }

        public Task DeleteProductConfiguration(ProductConfiguration productConfiguration)
        {
            return _db.SaveDataAsync("[dbo].[spProductConfiguration_DeleteById]", new { productConfiguration.Id });
        }

        #endregion

    }
}
