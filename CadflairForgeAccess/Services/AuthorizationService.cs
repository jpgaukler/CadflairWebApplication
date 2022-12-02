using Autodesk.Forge;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CadflairForgeAccess.Services
{
    public class AuthorizationService 
    {
        private dynamic? _internalToken;
        private dynamic? _publicToken;

        public string ClientId { get; }
        public string ClientSecret { get; }

        /// <summary>
        /// Service for providing forge access token to webapp. Forge credentials must be stored in the Environment variables in the form shown below: <br/><br/>
        /// <b>"FORGE_CLIENT_ID"</b> = your client id <br/>
        /// <b>"FORGE_CLIENT_SECRET"</b> = your client secret
        /// </summary>
        public AuthorizationService(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary>
        public dynamic GetPublic()
        {
            if (_publicToken == null || _publicToken?.ExpiresAt < DateTime.UtcNow)
            {
                Scope[] scopes = new Scope[]
                {
                    Scope.ViewablesRead
                };

                _publicToken = Get2LeggedToken(scopes);
                _publicToken.ExpiresAt = DateTime.UtcNow.AddSeconds(_publicToken.expires_in);
            }

            if (_publicToken == null) return new { Error = "Unable to acquire Public Forge token!" };

            return _publicToken;
        }


        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary>
        public async Task<dynamic> GetPublicAsync()
        {
            if (_publicToken == null || _publicToken?.ExpiresAt < DateTime.UtcNow)
            {
                Scope[] scopes = new Scope[]
                {
                    Scope.ViewablesRead
                };

                _publicToken = await Get2LeggedTokenAsync(scopes);
                _publicToken.ExpiresAt = DateTime.UtcNow.AddSeconds(_publicToken.expires_in);
            }

            if (_publicToken == null) return new { Error = "Unable to acquire Public Forge token!" };

            return _publicToken;
        }


        /// <summary>
        /// Get access token with internal (write) scope
        /// </summary>
        public dynamic GetInternal()
        {
            if (_internalToken == null || _internalToken?.ExpiresAt < DateTime.UtcNow)
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

                _internalToken = Get2LeggedToken(scopes);
                _internalToken.ExpiresAt = DateTime.UtcNow.AddSeconds(_internalToken.expires_in);
            }

            if (_internalToken == null) throw new Exception("Unable to acquire Internal Forge token!");

            return _internalToken;
        }

        /// <summary>
        /// Get access token with internal (write) scope
        /// </summary>
        public async Task<dynamic> GetInternalAsync()
        {
            if (_internalToken == null || _internalToken?.ExpiresAt < DateTime.UtcNow)
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

                _internalToken = await Get2LeggedTokenAsync(scopes);
                _internalToken.ExpiresAt = DateTime.UtcNow.AddSeconds(_internalToken.expires_in);
            }

            if (_internalToken == null) throw new Exception("Unable to acquire Internal Forge token!");

            return _internalToken;
        }

        /// <summary>
        /// Get the access token from Autodesk
        /// </summary>
        private dynamic Get2LeggedToken(Scope[] scopes)
        {
            TwoLeggedApi oauth = new();
            dynamic bearer = oauth.Authenticate(clientId: ClientId,
                                                clientSecret: ClientSecret,
                                                grantType: "client_credentials",
                                                scope: scopes);

            return bearer;
        }

        /// <summary>
        /// Get the access token from Autodesk
        /// </summary>
        private async Task<dynamic> Get2LeggedTokenAsync(Scope[] scopes)
        {
            TwoLeggedApi oauth = new();
            dynamic bearer = await oauth.AuthenticateAsync(clientId: ClientId,
                                                           clientSecret: ClientSecret,
                                                           grantType: "client_credentials",
                                                           scope: scopes);

            return bearer;
        }

    }
}
