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

        public Task<List<User>> GetUsers()
        {
            string sql = "select * from dbo.User";

            return _db.LoadDataAsync<User, dynamic>(sql, new { });
        }

        public Task<List<UserType>> GetUserTypes()
        {
            string sql = "select * from dbo.UserType";

            return _db.LoadDataAsync<UserType, dynamic>(sql, new { });
        }

        public async Task<User> GetUserById(int userId)
        {
            string sql = "select * from dbo.User where Id = @Id";

            dynamic values = new
            {
                Id = userId,
            };

            List<User> users = await _db.LoadDataAsync<User, dynamic>(sql, values);

            return users.First();
        }

        public async Task<UserType> GetUserTypeById(int userTypeId)
        {
            string sql = "select * from dbo.UserType where Id = @UserTypeId";

            dynamic values = new
            {
                UserTypeId = userTypeId,
            };

            List<UserType> users = await _db.LoadDataAsync<UserType, dynamic>(sql, values);

            return users.First();
        }

        public Task<List<User>> GetUsersByAccountId(int accountId)
        {
            string sql = "select * from dbo.User where AccountId = @AccountId";

            dynamic values = new
            {
                AccountId = accountId,
            };

            return _db.LoadDataAsync<User, dynamic>(sql, values);

        }

        public Task CreateUser(User newUser)
        {
            string sql = @"INSERT INTO [dbo].[User]
                           (AccountId
                           ,RoleId
                           ,PasswordHash
                           ,FirstName
                           ,LastName
                           ,UserName)
                           VALUES
                           (@AccountId
                           ,@RoleId
                           ,@PasswordHash
                           ,@FirstName
                           ,@LastName
                           ,@UserName)";

            return _db.SaveDataAsync(sql, newUser);
        }

        public Task DeleteUser(User user)
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
