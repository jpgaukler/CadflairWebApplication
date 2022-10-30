using Autodesk.Forge;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CadflairBlazorServer.Forge
{
    public static class OAuth 
    {
        #region Fields

        /// <summary>
        /// Internal token for use by server side forge functions
        /// </summary>
        private static dynamic? InternalToken { get; set; }

        /// <summary>
        /// Public token for use by client side readonly functions
        /// </summary>
        private static dynamic? PublicToken { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary>
        public static dynamic GetPublic()
        {
            if (PublicToken == null || PublicToken?.ExpiresAt < DateTime.UtcNow)
            {
                Scope[] scopes = new Scope[]
                {
                    Scope.ViewablesRead
                };

                PublicToken = Get2LeggedToken(scopes);
                PublicToken.ExpiresAt = DateTime.UtcNow.AddSeconds(PublicToken.expires_in);
            }

            if (PublicToken == null) return new { Error = "Unable to acquire Public Forge token!" };

            return PublicToken;
        }


        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary>
        public static async Task<dynamic> GetPublicAsync()
        {
            if (PublicToken == null || PublicToken?.ExpiresAt < DateTime.UtcNow)
            {
                Scope[] scopes = new Scope[]
                {
                    Scope.ViewablesRead
                };

                PublicToken = await Get2LeggedTokenAsync(scopes);
                PublicToken.ExpiresAt = DateTime.UtcNow.AddSeconds(PublicToken.expires_in);
            }

            if (PublicToken == null) return new { Error = "Unable to acquire Public Forge token!" };

            return PublicToken;
        }


        /// <summary>
        /// Get access token with internal (write) scope
        /// </summary>
        public static dynamic GetInternal()
        {
            if (InternalToken == null || InternalToken?.ExpiresAt < DateTime.UtcNow)
            {
                Scope[] scopes = new Scope[]
                {
                    Scope.BucketCreate,
                    Scope.BucketRead,
                    Scope.BucketDelete,
                    Scope.DataRead,
                    Scope.DataWrite,
                    Scope.DataCreate,
                    Scope.CodeAll
                };

                InternalToken = Get2LeggedToken(scopes);
                InternalToken.ExpiresAt = DateTime.UtcNow.AddSeconds(InternalToken.expires_in);
            }

            if (InternalToken == null) throw new Exception("Unable to acquire Internal Forge token!");

            return InternalToken;
        }

        /// <summary>
        /// Get access token with internal (write) scope
        /// </summary>
        public static async Task<dynamic> GetInternalAsync()
        {
            if (InternalToken == null || InternalToken?.ExpiresAt < DateTime.UtcNow)
            {

                Scope[] scopes = new Scope[]
                {
                    Scope.BucketCreate,
                    Scope.BucketRead,
                    Scope.BucketDelete,
                    Scope.DataRead,
                    Scope.DataWrite,
                    Scope.DataCreate,
                    Scope.CodeAll
                };

                InternalToken = await Get2LeggedTokenAsync(scopes);
                InternalToken.ExpiresAt = DateTime.UtcNow.AddSeconds(InternalToken.expires_in);
            }

            if (InternalToken == null) throw new Exception("Unable to acquire Internal Forge token!");

            return InternalToken;
        }

        /// <summary>
        /// Get the access token from Autodesk
        /// </summary>
        private static dynamic Get2LeggedToken(Scope[] scopes)
        {
            TwoLeggedApi oauth = new();
            dynamic bearer = oauth.Authenticate(clientId: Utils.GetAppSetting("FORGE_CLIENT_ID"),
                                                clientSecret: Utils.GetAppSetting("FORGE_CLIENT_SECRET"),
                                                grantType: "client_credentials",
                                                scope: scopes);

            return bearer;
        }


        /// <summary>
        /// Get the access token from Autodesk
        /// </summary>
        private static async Task<dynamic> Get2LeggedTokenAsync(Scope[] scopes)
        {
            TwoLeggedApi oauth = new();
            dynamic bearer = await oauth.AuthenticateAsync(clientId: Utils.GetAppSetting("FORGE_CLIENT_ID"),
                                                           clientSecret: Utils.GetAppSetting("FORGE_CLIENT_SECRET"),
                                                           grantType: "client_credentials",
                                                           scope: scopes);

            return bearer;
        }

        #endregion


    }
}
