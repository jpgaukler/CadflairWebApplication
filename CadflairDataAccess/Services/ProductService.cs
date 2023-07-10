﻿using CadflairDataAccess.Models;
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

        public Task<Product> CreateProduct(int subscriptionId, int productFolderId, string displayName, bool isPublic, int createdById)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                ProductFolderId = productFolderId,
                DisplayName = displayName,
                SubdirectoryName =  Regex.Replace(displayName, "[^a-zA-Z0-9_.]+", string.Empty).ToLower(),
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

        public Task<ProductFolder> CreateProductFolder(int subscriptionId, int createdById, string displayName, int? parentId)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                CreatedById = createdById,
                DisplayName = displayName,
                ParentId = parentId,
            };

            return _db.SaveSingleAsync<ProductFolder, dynamic>("[dbo].[spProductFolder_Insert]", values);
        }

        public Task<ProductFolder> GetProductFolderById(int id)
        {
            return _db.LoadSingleAsync<ProductFolder, dynamic>("[dbo].[spProductFolder_GetById]", new { Id = id });
        }

        public Task<List<ProductFolder>> GetProductFoldersBySubscriptionId(int subscriptionId)
        {
            return _db.LoadDataAsync<ProductFolder, dynamic>("[dbo].[spProductFolder_GetBySubscriptionId]", new { SubscriptionId = subscriptionId });
        }

        /// <summary>
        /// NEED TO DELETE THIS METHOD AND REPLACE IT WITH NEW TECHNIQUE OF GETTING ALL FOLDERS IN ONE CALL. SEE PRODUCT CATALOG PAGE. 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public Task<List<ProductFolder>> GetProductFoldersBySubscriptionIdAndParentId(int subscriptionId, int? parentId)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                ParentId = parentId,
            };

            return _db.LoadDataAsync<ProductFolder, dynamic>("[dbo].[spProductFolder_GetBySubscriptionIdAndParentId]", values);
        }

        public Task DeleteProductFolder(ProductFolder productFolder)
        {
            return _db.SaveDataAsync("[dbo].[spProductFolder_DeleteById]", new { productFolder.Id });
        }

        #endregion

        #region "ProductConfiguration"

        public Task<ProductConfiguration> CreateProductConfiguration(int productVersionId, string argumentJson, bool isDefault)
        {
            dynamic values = new
            {
                ProductVersionId = productVersionId,
                IsDefault = isDefault,
                ArgumentJson = argumentJson,
            };

            return _db.SaveSingleAsync<ProductConfiguration, dynamic>("[dbo].[spProductConfiguration_Insert]", values);
        }

        public Task<ProductConfiguration> GetProductConfigurationById(int id)
        {
            return _db.LoadSingleAsync<ProductConfiguration, dynamic>("[dbo].[spProductConfiguration_GetById]", new { Id = id });
        }

        public Task<List<ProductConfiguration>> GetProductsConfigurationsByProductVersionId(int productVersionId)
        {
            return _db.LoadDataAsync<ProductConfiguration, dynamic>("[dbo].[spProductConfiguration_GetByProductVersionId]", new { ProductVersionId = productVersionId });
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
                productConfiguration.BucketKey,
                productConfiguration.ZipObjectKey,
                productConfiguration.StpObjectKey,
            };

            return _db.SaveDataAsync("[dbo].[spProductConfiguration_UpdateById]", values);
        }


        #endregion

        #region "ProductQuoteRequest"

        public Task<ProductQuoteRequest> CreateProductQuoteRequest(int productConfigurationId, string firstName, string lastName, string emailAddress, string phoneNumber, string phoneExtension, string messageText)
        {
            dynamic values = new
            {
                ProductConfigurationId = productConfigurationId,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                PhoneNumber = phoneNumber,
                PhoneExtension = phoneExtension,
                MessageText = messageText
            };

            return _db.SaveSingleAsync<ProductQuoteRequest, dynamic>("[dbo].[spProductQuoteRequest_Insert]", values);
        }

        public Task<List<ProductQuoteRequest>> GetProductQuoteRequestsBySubscriptionId(int subscriptionId)
        {
            return _db.LoadDataAsync<ProductQuoteRequest, dynamic>("[dbo].[spProductQuoteRequest_GetBySubscriptionId]", new { SubscriptionId = subscriptionId });
        }


        #endregion

    }
}

