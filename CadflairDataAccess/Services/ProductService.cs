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

        public Task<int> CreateProduct(Product newProduct)
        {
            dynamic values = new
            {
                newProduct.SubscriptionId,
                newProduct.ProductFamilyId,
                newProduct.DisplayName,
                newProduct.ParameterJson,
                newProduct.ForgeBucketKey,
                newProduct.CreatedById,
                newProduct.IsPublic,
                newProduct.IsConfigurable
            };

            return _db.SaveSingleAsync("[dbo].[spProduct_Insert]", values);
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

        public Task<int> CreateProductFamily(ProductFamily newProductFamily)
        {
            dynamic values = new
            {
                newProductFamily.SubscriptionId,
                newProductFamily.ParentId,
                newProductFamily.DisplayName,
                newProductFamily.CreatedById,
            };

            return _db.SaveSingleAsync("[dbo].[spProductFamily_Insert]", values);
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

        public Task<int> CreateProductConfiguration(ProductConfiguration newProductConfiguration)
        {
            dynamic values = new
            {
                newProductConfiguration.ProductId,
                newProductConfiguration.ArgumentJson,
                newProductConfiguration.ForgeObjectKey
            };

            return _db.SaveSingleAsync("[dbo].[spProductConfiguration_Insert]", values);
        }

        public Task DeleteProductConfiguration(ProductConfiguration productConfiguration)
        {
            return _db.SaveDataAsync("[dbo].[spProductConfiguration_DeleteById]", new { productConfiguration.Id });
        }

        #endregion

    }
}
