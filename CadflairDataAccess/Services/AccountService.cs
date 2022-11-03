using CadflairDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadflairDataAccess.Services
{
    public class AccountService
    {

        private readonly DataAccess _db;

        public AccountService(DataAccess db)
        {
            _db = db;
        }

        public Task<List<Account>> GetAccounts()
        {
            string sql = "select * from dbo.Account";
            return _db.LoadDataAsync<Account, dynamic>(sql, new { });
        }

        public async Task<Account> GetAccountById(int accountId)
        {
            string sql = "select * from dbo.Account where Id = @Id";

            dynamic values = new
            {
                Id = accountId,
            };

            List<Account> accounts = await _db.LoadDataAsync<Account, dynamic>(sql, values);
            return accounts.First();
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

        public Task CreateAccount(Account newAccount)
        {
            string sql = @"INSERT INTO [dbo].[Account]
                           (CompanyName
                           ,SubDirectory
                           ,CreatedById
                           ,OwnerId
                           ,AccountTypeId
                           ,SubscriptionExpiresOn)
                           VALUES
                           (@CompanyName
                           ,@SubDirectory
                           ,@CreatedById
                           ,@OwnerId
                           ,@AccountTypeId
                           ,@SubscriptionExpiresOn)";

            return _db.SaveDataAsync<Account>(sql, newAccount);
        }

    }
}
