using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;
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

        public Task<List<Product>> GetProductsByProductFamilyId(int productFamilyId)
        {
            string sql = "select * from dbo.Product where ProductFamilyId = @ProductFamilyId";

            dynamic values = new
            {
                ProductFamilyId = productFamilyId,
            };

            return _db.LoadDataAsync<Product, dynamic>(sql, values);

        }


    }
}
