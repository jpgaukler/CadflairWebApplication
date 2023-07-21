using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CadflairDataAccess.Services
{
    public class CadModelService
    {

        private readonly DataAccess _db;

        public CadModelService(DataAccess db)
        {
            _db = db;
        }

        #region "CadModel"

        public Task<CadModel> CreateCadModel(int subscriptionId, int productFolderId, int createdById, string displayName, string description, string bucketKey, string objectKey, bool isZip = false, string rootFileName = null)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                ProductFolderId = productFolderId,
                CreatedById = createdById,
                DisplayName = displayName,
                Description = description,
                BucketKey = bucketKey,
                ObjectKey = objectKey,
                IsZip = isZip,
                RootFileName = rootFileName
            };

            return _db.SaveSingleAsync<CadModel, dynamic>("[dbo].[spCadModel_Insert]", values);
        }

        public Task<List<CadModel>> GetCadModelsByProductFolderId(int productFolderId)
        {
            return _db.LoadDataAsync<CadModel, dynamic>("[dbo].[spCadModel_GetByProductFolderId]", new { ProductFolderId = productFolderId });
        }

        #endregion

    }
}

