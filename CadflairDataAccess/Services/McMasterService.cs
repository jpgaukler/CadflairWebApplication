using CadflairDataAccess.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CadflairDataAccess.Services
{
    public class McMasterService
    {

        private readonly DataAccess _db;

        public McMasterService(DataAccess db)
        {
            _db = db;
        }

        #region "Category"

        public Task<Category> CreateCategory(int subscriptionId, int? parentId, string name, string description, string thumbnailUri, int createdById)
        {
            dynamic values = new
            {
                ParentId = parentId,
                SubscriptionId = subscriptionId,
                Name = name,
                Description = description,
                ThumbnailUri = thumbnailUri,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<Category, dynamic>("[dbo].[spCategory_Insert]", values);
        }

        public Task<Category> GetCategoryById(int id)
        {
            return _db.LoadSingleAsync<Category, dynamic>("[dbo].[spCategory_GetById]", new { Id = id });
        }

        public async Task<List<Category>> GetCategoriesBySubscriptionId(int subscriptionId)
        {
            var categories = await _db.LoadDataAsync<Category, dynamic>("[dbo].[spCategory_GetBySubscriptionId]", new { SubscriptionId = subscriptionId });

            if (categories == null || categories.Any() == false)
                return new List<Category>();

            foreach (Category category in categories)
            {
                category.ChildCategories = categories.Where(child => child.ParentId == category.Id).ToList();
                category.ParentCategory = categories.FirstOrDefault(parent => parent.Id == category.ParentId);
            }

            return categories.Where(i => i.ParentId == null).ToList();
        }

        public Task UpdateCategory(Category category)
        {
            dynamic values = new
            {
                category.Id,
                category.ParentId,
                category.Name,
                category.Description,
                category.ThumbnailUri,
            };

            return _db.SaveDataAsync("[dbo].[spCategory_UpdateById]", values);
        }

        public Task DeleteCategoryById(int categoryId)
        {
            return _db.SaveDataAsync("[dbo].[spCategory_DeleteById]", new { Id = categoryId });
        }

        #endregion

        #region "ProductDefinition"

        public async Task<ProductDefinition> CreateProductDefinition(int subscriptionId, int? categoryId, string name, string description, string thumbnailUri, string forgeBucketKey, int createdById)
        {
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                CategoryId = categoryId,
                Name = name,
                Description = description,
                ThumbnailUri = thumbnailUri,
                ForgeBucketKey = forgeBucketKey,
                CreatedById = createdById,
            };

            ProductDefinition newProductDefinition = await _db.SaveSingleAsync<ProductDefinition, dynamic>("[dbo].[spProductDefinition_Insert]", values);

            await CreateProductTable(newProductDefinition.Id, createdById);

            return newProductDefinition;
        }

        public Task<ProductDefinition> GetProductDefinitionByNameAndSubscriptionId(string name, int subscriptionId)
        { 
            dynamic values = new
            {
                SubscriptionId = subscriptionId,
                Name = name,
            };

            return _db.LoadSingleAsync<ProductDefinition, dynamic>("[dbo].[spProductDefinition_GetByNameAndSubscriptionId]", values);
        }

        public Task<List<ProductDefinition>> GetProductDefinitionsBySubscriptionId(int subscriptionId)
        {
            return _db.LoadDataAsync<ProductDefinition, dynamic>("[dbo].[spProductDefinition_GetBySubscriptionId]", new { SubscriptionId = subscriptionId });
        }

        public Task UpdateProductDefinition(ProductDefinition productDefinition)
        {
            dynamic values = new
            {
                productDefinition.Id,
                productDefinition.CategoryId,
                productDefinition.Name,
                productDefinition.Description,
                productDefinition.ThumbnailUri,
                productDefinition.ForgeBucketKey,
            };

            return _db.SaveDataAsync("[dbo].[spProductDefinition_UpdateById]", values);
        }

        public Task DeleteProductDefinitionById(int productDefinitionId)
        {
            // TO DO: delete product table

            return _db.SaveDataAsync("[dbo].[spProductDefinition_DeleteById]", new { Id = productDefinitionId });
        }

        #endregion

        #region "ProductTable"

        public Task<ProductTable> CreateProductTable(int productDefinitionId, int createdById)
        {
            dynamic values = new
            {
                ProductDefinitionId = productDefinitionId,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<ProductTable, dynamic>("[dbo].[spProductTable_Insert]", values);
        }

        public async Task<ProductTable> GetProductTableByProductDefinitionId(int productDefinitionId)
        {
            ProductTable productTable =  await _db.LoadSingleAsync<ProductTable, dynamic>("[dbo].[spProductTable_GetByProductDefinitionId]", new { ProductDefinitionId = productDefinitionId });
            productTable.Rows = await GetRowsByProductTableId(productTable.Id);
            productTable.Columns = await GetColumnsByProductTableId(productTable.Id);
            List<TableValue> values = await GetTableValuesByProductTableId(productTable.Id);
            List<Attachment> attachments = await GetAttachmentsByProductTableId(productTable.Id);

            foreach(var column in productTable.Columns)
            {
                column.TableValues = values.Where(i => i.ColumnId == column.Id).ToList();
            }

            foreach(var row in productTable.Rows)
            {
                row.TableValues = values.Where(i => i.RowId == row.Id).ToList();
                row.Attachments = attachments.Where(i => i.RowId == row.Id).ToList();
            }

            return productTable;
        }

        #endregion


        #region "Column"

        public Task<Column> CreateColumn(int productTableId, string header, int sortOrder, int createdById)
        {
            dynamic values = new
            {
                ProductTableId = productTableId,
                Header = header,
                SortOrder = sortOrder,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<Column, dynamic>("[dbo].[spColumn_Insert]", values);
        }

        private Task<List<Column>> GetColumnsByProductTableId(int productTableId)
        {
            return _db.LoadDataAsync<Column, dynamic>("[dbo].[spColumn_GetByProductTableId]", new { ProductTableId  = productTableId });
        }

        public Task UpdateColumn(Column column)
        {
            dynamic values = new
            {
                column.Id,
                column.Header,
                column.SortOrder,
            };

            return _db.SaveDataAsync("[dbo].[spColumn_UpdateById]", values);
        }

        public Task DeleteColumnById(int columnId)
        {
            return _db.SaveDataAsync("[dbo].[spColumn_DeleteById]", new { Id = columnId });
        }

        #endregion

        #region "Row"

        public Task<Row> CreateRow(int productTableId, string partNumber, int createdById)
        {
            dynamic values = new
            {
                ProductTableId = productTableId,
                PartNumber = partNumber,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<Row, dynamic>("[dbo].[spRow_Insert]", values);
        }

        private Task<List<Row>> GetRowsByProductTableId(int productTableId)
        {
            return _db.LoadDataAsync<Row, dynamic>("[dbo].[spRow_GetByProductTableId]", new { ProductTableId  = productTableId });
        }

        public async Task UpdateRow(Row row)
        {
            foreach (var tableValue in row.TableValues)
                await UpdateTableValue(tableValue);

            dynamic values = new
            {
                row.Id,
                row.PartNumber,
            };

            await _db.SaveDataAsync("[dbo].[spRow_UpdateById]", values);
        }

        public Task DeleteRowById(int rowId)
        {
            return _db.SaveDataAsync("[dbo].[spRow_DeleteById]", new { Id = rowId });
        }


        #endregion

        #region "TableValue"

        public Task<TableValue> CreateTableValue(int productTableId, int columnId, int rowId, string value, int createdById)
        {
            dynamic values = new
            {
                ProductTableId = productTableId,
                ColumnId = columnId,
                RowId = rowId,
                Value = value,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<TableValue, dynamic>("[dbo].[spTableValue_Insert]", values);
        }

        private Task UpdateTableValue(TableValue tableValue)
        {
            dynamic values = new
            {
                tableValue.Id,
                tableValue.RowId,
                tableValue.ColumnId,
                tableValue.Value,
            };

            return _db.SaveDataAsync("[dbo].[spTableValue_UpdateById]", values);
        }

        private Task<List<TableValue>> GetTableValuesByProductTableId(int productTableId)
        {
            return _db.LoadDataAsync<TableValue, dynamic>("[dbo].[spTableValue_GetByProductTableId]", new { ProductTableId  = productTableId });
        }


        #endregion

        #region "Attachment"

        public Task<Attachment> CreateAttachment(int rowId, string forgeObjectKey, int createdById)
        {
            dynamic values = new
            {
                RowId = rowId,
                ForgeObjectKey = forgeObjectKey,
                CreatedById = createdById,
            };

            return _db.SaveSingleAsync<Attachment, dynamic>("[dbo].[spAttachment_Insert]", values);
        }

        private Task<List<Attachment>> GetAttachmentsByProductTableId(int productTableId)
        {
            return _db.LoadDataAsync<Attachment, dynamic>("[dbo].[spAttachment_GetByProductTableId]", new { ProductTableId  = productTableId });
        }


        #endregion

    }
}

