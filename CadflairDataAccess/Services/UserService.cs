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
            string sql = "select * from dbo.[User]";

            return _db.LoadDataAsync<User, dynamic>(sql, new { });
        }

        public Task<User> GetUserById(int userId)
        {
            string sql = "select * from dbo.[User] where Id = @Id";

            dynamic values = new
            {
                Id = userId,
            };

            return _db.LoadSingleAsync<User, dynamic>(sql, values);
        }

        public async Task<User> GetUserByEmailAddress(string emailAddress)
        {
            string sql = "select * from dbo.[User] where EmailAddress = @EmailAddress";

            dynamic values = new
            {
                EmailAddress = emailAddress,
            };

            User user = await _db.LoadSingleAsync<User, dynamic>(sql, values);

            return user;
        }

        public Task<List<UserRole>> GetUserRoles()
        {
            string sql = "select * from dbo.[UserRole]";

            return _db.LoadDataAsync<UserRole, dynamic>(sql, new { });
        }

        public Task<UserRole> GetUserRoleById(int userRoleId)
        {
            string sql = "select * from dbo.[UserRole] where Id = @UserRoleId";

            dynamic values = new
            {
                UserRoleId = userRoleId,
            };

            return _db.LoadSingleAsync<UserRole, dynamic>(sql, values);
        }

        public Task<List<User>> GetUsersByAccountId(int accountId)
        {
            string sql = "select * from dbo.[User] where AccountId = @AccountId";

            dynamic values = new
            {
                AccountId = accountId,
            };

            return _db.LoadDataAsync<User, dynamic>(sql, values);

        }

        public Task CreateUser(User newUser)
        {
            string sql = @"INSERT INTO [dbo].[User]
                           (UserRoleId
                           ,FirstName
                           ,LastName
                           ,PasswordHash)
                           VALUES
                           (@UserRoleId
                           ,@FirstName
                           ,@LastName
                           ,@PasswordHash)";

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
