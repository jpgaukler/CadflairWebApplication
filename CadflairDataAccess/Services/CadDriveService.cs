using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CadflairDataAccess.Services
{
    public class CadDriveService
    {

        private readonly DataAccess _db;

        public CadDriveService(DataAccess db)
        {
            _db = db;
        }

        #region "DriveFolder"

        public Task<DriveFolder> CreateDriveFolder(int subscriptionId, int createdById, string displayName, int? parentId)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                CreatedById = createdById,
                DisplayName = displayName,
                ParentId = parentId,
            };

            return _db.SaveSingleAsync<DriveFolder, dynamic>("[dbo].[spDriveFolder_Insert]", values);
        }

        public Task<DriveFolder> GetDriveFolderById(int id)
        {
            return _db.LoadSingleAsync<DriveFolder, dynamic>("[dbo].[spDriveFolder_GetById]", new { Id = id });
        }

        public async Task<List<DriveFolder>> GetDriveFoldersBySubscriptionId(int subscriptionId)
        {
            var folders = await _db.LoadDataAsync<DriveFolder, dynamic>("[dbo].[spDriveFolder_GetBySubscriptionId]", new { SubscriptionId = subscriptionId });

            if (folders == null || folders.Any() == false)
                return new List<DriveFolder>();

            foreach (DriveFolder folder in folders)
            {
                folder.ChildFolders = folders.Where(child => child.ParentId == folder.Id).ToList();
                folder.ParentFolder = folders.FirstOrDefault(parent => parent.Id == folder.ParentId);
            }

            return folders.Where(i => i.ParentId == null).ToList();
        }

        public Task UpdateDriveFolder(DriveFolder folder)
        {
            dynamic values = new
            {
                folder.Id,
                folder.ParentId,
                folder.DisplayName,
            };

            return _db.SaveDataAsync("[dbo].[spDriveFolder_UpdateById]", values);
        }

        public Task DeleteDriveFolderById(int folderId)
        {
            return _db.SaveDataAsync("[dbo].[spDriveFolder_DeleteById]", new { Id = folderId });
        }

        #endregion

        #region "DriveDocument"

        public Task<DriveDocument> CreateDriveDocument(int subscriptionId, int folderId, int createdById, string displayName, string description, string bucketKey, string objectKey, bool isZip = false, string rootFileName = null)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                DriveFolderId = folderId,
                CreatedById = createdById,
                DisplayName = displayName,
                Description = description,
                BucketKey = bucketKey,
                ObjectKey = objectKey,
                IsZip = isZip,
                RootFileName = rootFileName
            };

            return _db.SaveSingleAsync<DriveDocument, dynamic>("[dbo].[spDriveDocument_Insert]", values);
        }

        public Task<DriveDocument> GetDriveDocumentByGuid(Guid guid)
        {
            return _db.LoadSingleAsync<DriveDocument, dynamic>("[dbo].[spDriveDocument_GetByGuid]", new { Guid = guid });
        }

        public Task<List<DriveDocument>> GetDriveDocumentsByDriveFolderId(int folderId)
        {
            return _db.LoadDataAsync<DriveDocument, dynamic>("[dbo].[spDriveDocument_GetByDriveFolderId]", new { DriveFolderId = folderId });
        }

        public Task DeleteDriveDocumentById(int id)
        {
            return _db.SaveDataAsync("[dbo].[spDriveDocument_DeleteById]", new { Id = id });
        }

        public Task UpdateDriveDocument(DriveDocument document)
        {
            dynamic values = new
            {
                document.Id,
                document.SubscriptionId,
                document.DriveFolderId,
                document.DisplayName,
                document.Description,
                document.BucketKey,
                document.ObjectKey,
                document.IsZip,
                document.RootFileName,
            };

            return _db.SaveDataAsync("[dbo].[spDriveDocument_UpdateById]", values);
        }


        #endregion

    }
}

