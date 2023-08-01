using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CadflairDataAccess.Services
{
    public class CatalogService
    {

        private readonly DataAccess _db;

        public CatalogService(DataAccess db)
        {
            _db = db;
        }

        #region "CatalogFolder"

        public Task<CatalogFolder> CreateCatalogFolder(int subscriptionId, int createdById, string displayName, int? parentId)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                CreatedById = createdById,
                DisplayName = displayName,
                ParentId = parentId,
            };

            return _db.SaveSingleAsync<CatalogFolder, dynamic>("[dbo].[spCatalogFolder_Insert]", values);
        }

        public Task<CatalogFolder> GetCatalogFolderById(int id)
        {
            return _db.LoadSingleAsync<CatalogFolder, dynamic>("[dbo].[spCatalogFolder_GetById]", new { Id = id });
        }

        public async Task<List<CatalogFolder>> GetCatalogFoldersBySubscriptionId(int subscriptionId)
        {
            var folders = await _db.LoadDataAsync<CatalogFolder, dynamic>("[dbo].[spCatalogFolder_GetBySubscriptionId]", new { SubscriptionId = subscriptionId });

            if (folders == null || folders.Any() == false)
                return new List<CatalogFolder>();

            foreach (CatalogFolder folder in folders)
            {
                folder.ChildFolders = folders.Where(child => child.ParentId == folder.Id).ToList();
                folder.ParentFolder = folders.FirstOrDefault(parent => parent.Id == folder.ParentId);
            }

            return folders.Where(i => i.ParentId == null).ToList();
        }

        public Task UpdateCatalogFolder(CatalogFolder catalogFolder)
        {
            dynamic values = new
            {
                catalogFolder.Id,
                catalogFolder.ParentId,
                catalogFolder.DisplayName,
            };

            return _db.SaveDataAsync("[dbo].[spCatalogFolder_UpdateById]", values);
        }

        public Task DeleteCatalogFolderById(int catalogFolderId)
        {
            return _db.SaveDataAsync("[dbo].[spCatalogFolder_DeleteById]", new { Id = catalogFolderId });
        }

        #endregion

        #region "CatalogModel"

        public Task<CatalogModel> CreateCatalogModel(int subscriptionId, int catalogFolderId, int createdById, string displayName, string description, string bucketKey, string objectKey, bool isZip = false, string rootFileName = null)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                CatalogFolderId = catalogFolderId,
                CreatedById = createdById,
                DisplayName = displayName,
                Description = description,
                BucketKey = bucketKey,
                ObjectKey = objectKey,
                IsZip = isZip,
                RootFileName = rootFileName
            };

            return _db.SaveSingleAsync<CatalogModel, dynamic>("[dbo].[spCatalogModel_Insert]", values);
        }

        public Task<CatalogModel> GetCatalogModelByGuid(Guid guid)
        {
            return _db.LoadSingleAsync<CatalogModel, dynamic>("[dbo].[spCatalogModel_GetByGuid]", new { Guid = guid });
        }

        public Task<List<CatalogModel>> GetCatalogModelsByCatalogFolderId(int catalogFolderId)
        {
            return _db.LoadDataAsync<CatalogModel, dynamic>("[dbo].[spCatalogModel_GetByCatalogFolderId]", new { CatalogFolderId = catalogFolderId });
        }

        public Task DeleteCatalogModelById(int id)
        {
            return _db.SaveDataAsync("[dbo].[spCatalogModel_DeleteById]", new { Id = id });
        }

        public Task UpdateCatalogModel(CatalogModel catalogModel)
        {
            dynamic values = new
            {
                catalogModel.Id,
                catalogModel.SubscriptionId,
                catalogModel.CatalogFolderId,
                catalogModel.DisplayName,
                catalogModel.Description,
                catalogModel.BucketKey,
                catalogModel.ObjectKey,
                catalogModel.IsZip,
                catalogModel.RootFileName,
            };

            return _db.SaveDataAsync("[dbo].[spCatalogModel_UpdateById]", values);
        }


        #endregion

    }
}

