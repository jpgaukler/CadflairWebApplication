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

        #region "Users"

        public Task<User> GetUserByObjectIdentifier(string objectIdentifier)
        {
            return _db.LoadSingleAsync<User, dynamic>("[dbo].[spUser_GetByObjectIdentifier]", new { ObjectIdentifier = objectIdentifier });
        }

        public Task<List<User>> GetUsersBySubscriptionId(int subscriptionId)
        {
            return _db.LoadDataAsync<User, dynamic>("[dbo].[spUser_GetBySubscriptionId]", new { SubscriptionId = subscriptionId });
        }

        public Task<User> CreateUser(Guid objectIdentifier, string firstName, string lastName, string emailAddress)
        {
            dynamic values = new
            {
                ObjectIdentifier = objectIdentifier,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
            };

            return _db.SaveSingleAsync<User, dynamic>("[dbo].[spUser_Insert]", values);
        }

        public Task UpdateUser(User user)
        {
            dynamic values = new
            {
                user.Id,
                user.ObjectIdentifier,
                user.SubscriptionId,
                user.FirstName,
                user.LastName,
                user.EmailAddress,
            };

            return _db.SaveDataAsync("[dbo].[spUser_UpdateById]", values);
        }

        public Task DeleteUser(User user)
        {
            return _db.SaveDataAsync("[dbo].[spUser_DeleteById]", new { user.Id });
        }

        #endregion

    }
}
