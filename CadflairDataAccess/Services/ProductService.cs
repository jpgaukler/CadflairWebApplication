using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
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

        public Task<List<Product>> GetProducts()
        {
            string sql = "select * from dbo.Product";

            return _db.LoadDataAsync<Product, dynamic>(sql, new { });
        }

        public async Task<Product> GetProductById(int productId)
        {
            string sql = "select * from dbo.Product where Id = @Id";

            dynamic values = new
            {
                Id = productId,
            };

            List<Product> accounts = await _db.LoadDataAsync<Product, dynamic>(sql, values);

            return accounts.First();
        }

        public Task<List<Product>> GetProductsByProductFamilyId(int productFamilyId)
        {
            string sql = "select * from dbo.Product where ProductFamilyId = @ProductFamilyId";

            dynamic values = new
            {
                ProductFamilyId = productFamilyId,
            };

            return _db.LoadDataAsync<Product, dynamic>(sql, values);
        }

        public Task CreateProduct(Product newProduct)
        {
            string sql = @"INSERT INTO [dbo].[Product]
                           (ProductFamilyId
                           ,DisplayName
                           ,ParameterJson
                           ,ForgeBucketKey
                           ,ForgeObjectKey
                           ,CreatedById
                           ,IsPublic
                           ,IsConfigurable)
                           VALUES
                           (@ProductFamilyId
                           ,@DisplayName
                           ,@ParameterJson
                           ,@ForgeBucketKey
                           ,@ForgeObjectKey
                           ,@CreatedById
                           ,@IsPublic
                           ,@IsConfigurable)";


            return _db.SaveDataAsync(sql, newProduct);
        }

        public Task DeleteProduct(Product product)
        {
            string sql = "DELETE FROM [dbo].[Product] WHERE Id = @Id";

            dynamic values = new
            {
                Id = product.Id,
            };

            return _db.SaveDataAsync(sql, values);
        }

        public Task<List<ProductFamily>> GetProductFamilies()
        {
            string sql = "select * from dbo.ProductFamily";

            return _db.LoadDataAsync<ProductFamily, dynamic>(sql, new { });
        }

        public async Task<ProductFamily> GetProductFamilyById(int productId)
        {
            string sql = "select * from dbo.ProductFamily where Id = @Id";

            dynamic values = new
            {
                Id = productId,
            };

            List<ProductFamily> families = await _db.LoadDataAsync<ProductFamily, dynamic>(sql, values);

            return families.First();
        }

        public Task<List<ProductFamily>> GetProductFamiliesByAccountId(int accountId)
        {
            string sql = "select * from dbo.ProductFamily where AccountId = @AccountId";

            dynamic values = new
            {
                AccountId = accountId,
            };

            return _db.LoadDataAsync<ProductFamily, dynamic>(sql, values);
        }

        public Task CreateProductFamily(ProductFamily newProductFamily)
        {
            string sql = @"INSERT INTO [dbo].[ProductFamily]
                           (ParentId
                           ,AccountId
                           ,DisplayName
                           ,CreatedById)
                           VALUES
                           (@ParentId
                           ,@AccountId
                           ,@DisplayName
                           ,@CreatedById)";

            return _db.SaveDataAsync(sql, newProductFamily);
        }

        public Task DeleteProductFamily(Product productFamily)
        {
            string sql = "DELETE FROM [dbo].[ProductFamily] WHERE Id = @Id";

            dynamic values = new
            {
                Id = productFamily.Id,
            };

            return _db.SaveDataAsync(sql, values);
        }

        public Task<List<ProductConfiguration>> GetProductConfigurations()
        {
            string sql = "select * from dbo.ProductConfiguration";

            return _db.LoadDataAsync<ProductConfiguration, dynamic>(sql, new { });
        }

        public async Task<ProductConfiguration> GetProductConfigurationById(int productConfigurationId)
        {
            string sql = "select * from dbo.ProductConfiguration where Id = @Id";

            dynamic values = new
            {
                Id = productConfigurationId,
            };

            List<ProductConfiguration> accounts = await _db.LoadDataAsync<ProductConfiguration, dynamic>(sql, values);

            return accounts.First();
        }

        public Task<List<ProductConfiguration>> GetProductConfigurationsByProductId(int productId)
        {
            string sql = "select * from dbo.ProductConfiguration where ProductId = @ProductId";

            dynamic values = new
            {
                ProductId = productId,
            };

            return _db.LoadDataAsync<ProductConfiguration, dynamic>(sql, values);
        }

        public Task CreateProductConfiguration(ProductConfiguration newProductConfiguration)
        {
            string sql = @"INSERT INTO [dbo].[ProductConfiguration]
                           (ProductId
                           ,ArgumentJson
                           ,ForgeBucketKey
                           ,ForgeObjectKey)
                           VALUES
                           (@ProductId
                           ,@ArgumentJson
                           ,@ForgeBucketKey
                           ,@ForgeObjectKey)";

            return _db.SaveDataAsync(sql, newProductConfiguration);
        }

        public Task DeleteProductConfiguration(ProductConfiguration productConfiguration)
        {
            string sql = "DELETE FROM [dbo].[ProductConfiguration] WHERE Id = @Id";

            dynamic values = new
            {
                Id = productConfiguration.Id,
            };

            return _db.SaveDataAsync(sql, values);
        }

        public Task<List<ProductQuoteRequest>> GetProductQuoteRequests()
        {
            string sql = "select * from dbo.ProductQuoteRequest";

            return _db.LoadDataAsync<ProductQuoteRequest, dynamic>(sql, new { });
        }

        //public Task<List<ProductQuoteRequest>> GetProductQuoteRequestsByProductId()
        //{
        //    string sql = "select * from dbo.ProductQuoteRequest";

        //    return _db.LoadDataAsync<ProductQuoteRequest, dynamic>(sql, new { });
        //}

        //public async Task<Product> GetProductById(int productId)
        //{
        //    string sql = "select * from dbo.Product where Id = @Id";

        //    dynamic values = new
        //    {
        //        Id = productId,
        //    };

        //    List<Product> accounts = await _db.LoadDataAsync<Product, dynamic>(sql, values);

        //    return accounts.First();
        //}

        //public Task<List<Product>> GetProductsByProductFamilyId(int productFamilyId)
        //{
        //    string sql = "select * from dbo.Product where ProductFamilyId = @ProductFamilyId";

        //    dynamic values = new
        //    {
        //        ProductFamilyId = productFamilyId,
        //    };

        //    return _db.LoadDataAsync<Product, dynamic>(sql, values);
        //}

        //public Task CreateProduct(Product newProduct)
        //{
        //    string sql = @"INSERT INTO [dbo].[Product]
        //                   (ProductFamilyId
        //                   ,DisplayName
        //                   ,ParameterJson
        //                   ,ForgeBucketKey
        //                   ,ForgeObjectKey
        //                   ,CreatedById
        //                   ,IsPublic
        //                   ,IsConfigurable)
        //                   VALUES
        //                   (@ProductFamilyId
        //                   ,@DisplayName
        //                   ,@ParameterJson
        //                   ,@ForgeBucketKey
        //                   ,@ForgeObjectKey
        //                   ,@CreatedById
        //                   ,@IsPublic
        //                   ,@IsConfigurable)";


        //    return _db.SaveDataAsync(sql, newProduct);
        //}

        //public Task DeleteProduct(Product product)
        //{
        //    string sql = "DELETE FROM [dbo].[Product] WHERE Id = @Id";

        //    dynamic values = new
        //    {
        //        Id = product.Id,
        //    };

        //    return _db.SaveDataAsync(sql, values);
        //}
    }
}
