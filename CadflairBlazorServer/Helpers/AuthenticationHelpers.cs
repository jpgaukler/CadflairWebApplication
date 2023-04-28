using Microsoft.AspNetCore.Components.Authorization;

namespace CadflairBlazorServer.Helpers
{
    internal static class AuthenticationHelpers
    {
        public static async Task<User> GetUser(this AuthenticationStateProvider provider, DataServicesManager dataServicesManager)
        {
            // get claims from auth provider
            AuthenticationState authState = await provider.GetAuthenticationStateAsync();
            string objectId = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value ?? string.Empty;
            string firstName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value ?? string.Empty;
            string lastName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("surname"))?.Value ?? string.Empty;
            string emailAddress = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value ?? string.Empty;

            // return empty user if not logged in
            if (string.IsNullOrWhiteSpace(objectId)) 
                return new User();

            // get user from db
            User user = await dataServicesManager.UserService.GetUserByObjectIdentifier(objectId) ?? new();

            //create new user if no match is found
            if (user.Id == 0)
            {
                User newUser = await dataServicesManager.UserService.CreateUser(Guid.Parse(objectId), firstName, lastName, emailAddress);
                return newUser;
            }

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
                await dataServicesManager.UserService.UpdateUser(user);

            return user;
        }
    }
}
