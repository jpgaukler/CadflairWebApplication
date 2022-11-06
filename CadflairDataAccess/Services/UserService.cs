using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadflairDataAccess.Services
{
    public class UserService
    {

        private readonly DataAccess _db;

        public UserService(DataAccess db)
        {
            _db = db;
        }

        public Task<List<ApplicationUser>> GetUsers()
        {
            string sql = "select * from dbo.[User]";

            return _db.LoadDataAsync<ApplicationUser, dynamic>(sql, new { });
        }

        public Task<List<ApplicationRole>> GetUserTypes()
        {
            string sql = "select * from dbo.UserType";

            return _db.LoadDataAsync<ApplicationRole, dynamic>(sql, new { });
        }

        public async Task<ApplicationUser> GetUserById(int userId)
        {
            string sql = "select * from dbo.User where Id = @Id";

            dynamic values = new
            {
                Id = userId,
            };

            List<ApplicationUser> users = await _db.LoadDataAsync<ApplicationUser, dynamic>(sql, values);

            return users.First();
        }

        public async Task<ApplicationRole> GetUserTypeById(int userTypeId)
        {
            string sql = "select * from dbo.UserType where Id = @UserTypeId";

            dynamic values = new
            {
                UserTypeId = userTypeId,
            };

            List<ApplicationRole> users = await _db.LoadDataAsync<ApplicationRole, dynamic>(sql, values);

            return users.First();
        }

        public Task<List<ApplicationUser>> GetUsersByAccountId(int accountId)
        {
            string sql = "select * from dbo.User where AccountId = @AccountId";

            dynamic values = new
            {
                AccountId = accountId,
            };

            return _db.LoadDataAsync<ApplicationUser, dynamic>(sql, values);

        }

        public Task CreateUser(ApplicationUser newUser)
        {
            string sql = @"INSERT INTO [dbo].[User]
                           (UserTypeId
                           ,PasswordHash
                           ,FirstName
                           ,LastName
                           ,UserName)
                           VALUES
                           (@UserTypeId
                           ,@PasswordHash
                           ,@FirstName
                           ,@LastName
                           ,@UserName)";

            return _db.SaveDataAsync(sql, newUser);
        }

        public Task DeleteUser(ApplicationUser user)
        {
            string sql = "DELETE FROM [dbo].[User] WHERE Id = @Id";

            dynamic values = new
            {
                Id = user.Id,
            };

            return _db.SaveDataAsync(sql, values);
        }

    }
}
