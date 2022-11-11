using CadflairDataAccess.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace CadflairBlazorServer.Authentication
{
    public class CadflairAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        //private readonly ProtectedLocalStorage _localStorage;
        private readonly ClaimsPrincipal _anonymousUser;

        public CadflairAuthenticationStateProvider(ProtectedSessionStorage sessionStorageService)
        {
            _sessionStorage = sessionStorageService;
            _anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

                if (userSession == null)
                {
                    return await Task.FromResult(new AuthenticationState(_anonymousUser));
                }

                Claim[] claims = new[]
                {
                    new Claim(ClaimTypes.Name, userSession.Name),
                    new Claim(ClaimTypes.Email, userSession.EmailAddress),
                    new Claim(ClaimTypes.Role, userSession.Role)
                };

                ClaimsIdentity identity = new(claims, "CadflairAuth");
                ClaimsPrincipal user = new(identity);

                return await Task.FromResult(new AuthenticationState(user));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymousUser));
            }
        }


        public async Task UpdateAuthenticationState(UserSession? userSession = null)
        {
            ClaimsPrincipal user;

            if(userSession != null)
            {
                await _sessionStorage.SetAsync("UserSession", userSession);

                Claim[] claims = new[]
                {
                    new Claim(ClaimTypes.Name, userSession.Name),
                    new Claim(ClaimTypes.Email, userSession.EmailAddress),
                    new Claim(ClaimTypes.Role, userSession.Role)
                };

                ClaimsIdentity identity = new(claims, "CadflairAuth");
                user = new ClaimsPrincipal(identity);
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");
                user = _anonymousUser;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }


    }
}
