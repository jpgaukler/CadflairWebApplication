using CadflairDataAccess.Models;
using CadflairDataAccess.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace CadflairBlazorServer.Helpers
{
    internal static class AuthenticationHelpers
    {
        public static async Task<User> GetUser(this AuthenticationStateProvider provider, UserService userService)
        {
            // get claims from auth provider
            AuthenticationState authState = await provider.GetAuthenticationStateAsync();
            string objectId = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value ?? string.Empty;
            string firstName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value ?? string.Empty;
            string lastName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("surname"))?.Value ?? string.Empty;
            string email = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value ?? string.Empty;

            // return empty user if not logged in
            if (string.IsNullOrWhiteSpace(objectId)) return new User();

            // get user from db
            User user = await userService.GetUserByObjectIdentifier(objectId) ?? new();

            // compare claim values with user values in db
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

            if (!email.Equals(user.EmailAddress))
            {
                isDirty = true;
                user.EmailAddress = email;
            }

            if (isDirty)
            {
                if (user.Id == 0)
                {
                    user.Id = await userService.CreateUser(user);
                }
                else
                {
                    await userService.UpdateUser(user);
                }
            }

            return user;
        }
    }
}
