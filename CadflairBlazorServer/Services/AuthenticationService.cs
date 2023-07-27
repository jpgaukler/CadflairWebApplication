using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CadflairBlazorServer.Services
{
    public class AuthenticationService
    {
        // services
        private readonly DataServicesManager _dataServicesManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILogger<AuthenticationService> _logger;


        public AuthenticationService(AuthenticationStateProvider authenticationStateProvider, DataServicesManager dataServicesManager, ProtectedLocalStorage protectedLocalStorage, ILogger<AuthenticationService> logger)
        {
            _dataServicesManager = dataServicesManager;
            _authenticationStateProvider = authenticationStateProvider;
            _logger = logger;
        }

        public async Task<User?> GetUser()
        {
            try
            {
                // get claims from auth provider
                AuthenticationState authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                string objectId = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value ?? string.Empty;
                string firstName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value ?? string.Empty;
                string lastName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("surname"))?.Value ?? string.Empty;
                string emailAddress = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value ?? string.Empty;

                // exit if no user is logged in
                if (string.IsNullOrWhiteSpace(objectId))
                    return null;

                // get user from db
                User? user = await _dataServicesManager.UserService.GetUserByObjectIdentifier(objectId);

                //create new user if no match is found
                if (user == null)
                {
                    user = await _dataServicesManager.UserService.CreateUser(Guid.Parse(objectId), firstName, lastName, emailAddress);
                }
                else
                {
                    // if match is found, compare claim values with user values in db and update record if needed
                    bool isDirty = false;

                    if (!objectId.Equals(user.ObjectIdentifier.ToString()))
                    {
                        isDirty = true;
                        user.ObjectIdentifier = Guid.Parse(objectId);
                    }

                    if (!firstName.Equals(user.FirstName))
                    {
                        isDirty = true;
                        user.FirstName = firstName;
                    }

                    if (!lastName.Equals(user.LastName))
                    {
                        isDirty = true;
                        user.LastName = lastName;
                    }

                    if (!emailAddress.Equals(user.EmailAddress))
                    {
                        isDirty = true;
                        user.EmailAddress = emailAddress;
                    }

                    if (isDirty)
                        await _dataServicesManager.UserService.UpdateUser(user);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unknown error occurred!");
                return null;
            }
        }
    }
}
