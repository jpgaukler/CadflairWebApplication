using Autodesk.Forge;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CadflairWebApplication.Controllers.Forge
{
    [ApiController]
    public class OAuthController : ControllerBase
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
        /// Get access token with internal (write) scope
        /// </summary>
        public static dynamic? GetInternal()
        {
            if (InternalToken == null || InternalToken?.ExpiresAt < DateTime.UtcNow)
            {
                InternalToken = Get2LeggedToken(new Scope[] { Scope.BucketCreate, Scope.BucketRead, Scope.BucketDelete, Scope.DataRead, Scope.DataWrite, Scope.DataCreate, Scope.CodeAll });
                InternalToken.ExpiresAt = DateTime.UtcNow.AddSeconds(InternalToken.expires_in);
            }

            return InternalToken;
        }

        /// <summary>
        /// Get access token with internal (write) scope
        /// </summary>
        public static async Task<dynamic?> GetInternalAsync()
        {
            if (InternalToken == null || InternalToken?.ExpiresAt < DateTime.UtcNow)
            {
                InternalToken = await Get2LeggedTokenAsync(new Scope[] { Scope.BucketCreate, Scope.BucketRead, Scope.BucketDelete, Scope.DataRead, Scope.DataWrite, Scope.DataCreate, Scope.CodeAll });
                InternalToken.ExpiresAt = DateTime.UtcNow.AddSeconds(InternalToken.expires_in);
            }

            return InternalToken;
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


        #endregion


        #region API

        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary>
        [HttpGet]
        [Route("api/forge/oauth/token")]
        public async Task<dynamic?> GetPublicAsync()
        {
            if (PublicToken == null || PublicToken?.ExpiresAt < DateTime.UtcNow)
            {
                PublicToken = await Get2LeggedTokenAsync(new Scope[] { Scope.ViewablesRead });
                PublicToken.ExpiresAt = DateTime.UtcNow.AddSeconds(PublicToken.expires_in);
            }

            return PublicToken;
        }

        #endregion

    }
}
