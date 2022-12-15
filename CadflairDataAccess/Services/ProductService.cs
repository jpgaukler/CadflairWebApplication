using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public Task<List<Product>> GetProductsByProductFamilyId(int productFamilyId)
        {
            return _db.LoadDataAsync<Product, dynamic>("[dbo].[spProduct_GetByProductFamilyId]", new { ProductFamilyId = productFamilyId });
        }

        public Task<Product> CreateProduct(int subscriptionId, int productFamilyId, string displayName, string parameterJson, Guid forgeBucketKey, int createdById, bool isPublic, bool isConfigurable)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                ProductFamilyId = productFamilyId,
                DisplayName = displayName,
                ParameterJson = parameterJson,
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

        #region "ProductFamily"

        public Task<ProductFamily> GetProductFamilyById(int id)
        {
            return _db.LoadSingleAsync<ProductFamily, dynamic>("[dbo].[spProductFamily_GetById]", new { Id = id });
        }

        public Task<List<ProductFamily>> GetProductFamiliesBySubscriptionId(int subscriptionId)
        {
            return _db.LoadDataAsync<ProductFamily, dynamic>("[dbo].[spProductFamily_GetBySubscriptionId]", new { SubscriptionId = subscriptionId });
        }

        public Task<ProductFamily> CreateProductFamily(int subscriptionId, int parentId, string displayName, int createdById)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                ParentId = parentId,
                DisplayName = displayName,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<ProductFamily, dynamic>("[dbo].[spProductFamily_Insert]", values);
        }

        public Task DeleteProductFamily(Product productFamily)
        {
            return _db.SaveDataAsync("[dbo].[spProductFamily_DeleteById]", new { productFamily.Id });
        }

        #endregion

        #region "ProductConfiguration"

        public Task<ProductConfiguration> GetProductConfigurationById(int id)
        {
            return _db.LoadSingleAsync<ProductConfiguration, dynamic>("[dbo].[spProductConfiguration_GetById]", new { Id = id });
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
                ArgumentJson = argumentJson,
                ForgeObjectKey = forgeZipKey,
                IsDefault = isDefault,
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
