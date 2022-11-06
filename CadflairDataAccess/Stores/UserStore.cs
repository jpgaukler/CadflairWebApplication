using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Dapper;
using CadflairDataAccess.Models;

namespace CadflairDataAccess.Stores
{
    public class UserStore : IUserStore<ApplicationUser> //, IUserPasswordStore<ApplicationUser>
    {

        private readonly string _connectionString;

        public UserStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Development");
        }

        public void Dispose()
        {
            //nothing to dispose
        }

        #region "IUserStore members"

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                user.Id = await connection.QuerySingleAsync<int>($@"INSERT INTO [ApplicationUser] 
                                                                    ([ApplicationRoleId]
                                                                    ,[UserName] 
                                                                    ,[NormalizedUserName]
                                                                    ,[PasswordHash]
                                                                    ,[FirstName]
                                                                    ,[LastName])
                                                                    VALUES 
                                                                    (@{nameof(ApplicationUser.ApplicationRoleId)}
                                                                    ,@{nameof(ApplicationUser.UserName)}
                                                                    ,@{nameof(ApplicationUser.NormalizedUserName)}
                                                                    ,@{nameof(ApplicationUser.PasswordHash)}
                                                                    ,@{nameof(ApplicationUser.FirstName)} 
                                                                    ,@{nameof(ApplicationUser.LastName)});

                                                                    SELECT CAST(SCOPE_IDENTITY() as int)", user);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($"DELETE FROM [ApplicationUser] WHERE [Id] = @{nameof(ApplicationUser.Id)}", user);
            }

            return IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser] WHERE [Id] = @{nameof(ApplicationUser.Id)}", new { userId });
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser] WHERE [NormalizedUserName] = @{nameof(ApplicationUser.NormalizedUserName)}", new { normalizedUserName });
            }
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                //    await connection.OpenAsync(cancellationToken);
                //    await connection.ExecuteAsync($@"UPDATE [ApplicationUser] SET
                //                                     [UserName] = @{nameof(ApplicationUser.UserName)},
                //                                     [NormalizedUserName] = @{nameof(ApplicationUser.NormalizedUserName)},
                //                                     [Email] = @{nameof(ApplicationUser.Email)},
                //                                     [NormalizedEmail] = @{nameof(ApplicationUser.NormalizedEmail)},
                //                                     [EmailConfirmed] = @{nameof(ApplicationUser.EmailConfirmed)},
                //                                     [PasswordHash] = @{nameof(ApplicationUser.PasswordHash)},
                //                                     [PhoneNumber] = @{nameof(ApplicationUser.PhoneNumber)},
                //                                     [PhoneNumberConfirmed] = @{nameof(ApplicationUser.PhoneNumberConfirmed)},
                //                                     [TwoFactorEnabled] = @{nameof(ApplicationUser.TwoFactorEnabled)}
                //                                     WHERE [Id] = @{nameof(ApplicationUser.Id)}", user);
                //}

                return IdentityResult.Success;
            }


            #endregion


            #region "IUserPasswordStore members"

            //public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
            //{
            //    user.PasswordHash = passwordHash;
            //    return Task.FromResult(0);
            //}

            //public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
            //{
            //    return Task.FromResult(user.PasswordHash);
            //}

            //public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
            //{
            //    return Task.FromResult(user.PasswordHash != null);
            //}

            #endregion

        }
    }

}
