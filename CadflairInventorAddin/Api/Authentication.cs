// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CadflairInventorAddin.Helpers;
using CadflairInventorLibrary.Helpers;
using Inventor;
using Microsoft.Identity.Client;

namespace CadflairInventorAddin.Api
{
    internal class Authentication
    {
        private static readonly string TenantName = "cadflair";
        private static readonly string Tenant = $"{TenantName}.onmicrosoft.com";
        private static readonly string AzureAdB2CHostname = $"{TenantName}.b2clogin.com";
        private static readonly string ClientId = "7f344a2b-d9ed-4ed7-af25-0f3b381f855c";
        private static readonly string RedirectUri = $"https://{TenantName}.b2clogin.com/oauth2/nativeclient";
        private static readonly string PolicySignIn = "b2c_1_signin";
        private static readonly string PolicyResetPassword = "b2c_1_reset";
        private static readonly string AuthorityBase = $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";
        private static readonly string AuthoritySignIn = $"{AuthorityBase}{PolicySignIn}";
        private static readonly string AuthorityResetPassword = $"{AuthorityBase}{PolicyResetPassword}";
        public static string[] ApiScopes = { $"https://{Tenant}/api/User.InventorAddin" };

        private static AuthenticationResult _authResult;
        private static IPublicClientApplication _publicClientApp;

        private static bool _signedIn;
        public static bool SignedIn
        {
            get
            {
                return _signedIn;
            }
            set
            {
                if (!value) _authResult = null;
                _signedIn = value;
                SignInButton.Enabled = !value;
                SignOutButton.Enabled = value;
            }
        }

        public static ButtonDefinition SignInButton { get; set; }
        public static ButtonDefinition SignOutButton { get; set; }

        public static async void InitializeAzureB2C()
        {
            _publicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                                                             .WithB2CAuthority(AuthoritySignIn)
                                                             .WithB2CAuthority(AuthorityResetPassword)
                                                             .WithRedirectUri(RedirectUri)
                                                             .Build();

            TokenCacheHelper.Bind(_publicClientApp.UserTokenCache);

            // try to sign in using existing token in the cache
            try
            {
                var accounts = await _publicClientApp.GetAccountsAsync(PolicySignIn);
                _authResult = await _publicClientApp.AcquireTokenSilent(ApiScopes, accounts.FirstOrDefault()).ExecuteAsync();
                SignedIn = true;
            }
            catch
            {
                SignedIn = false;
            }
        }

        private static async Task SignIn()
        {
            try
            {
                _authResult = await _publicClientApp.AcquireTokenInteractive(ApiScopes)
                                                    .WithB2CAuthority(AuthoritySignIn)
                                                    .WithParentActivityOrWindow(new InventorMainFrame(Globals.InventorApplication.MainFrameHWND))
                                                    .ExecuteAsync();

                SignedIn = true;
            }
            catch (MsalException ex)
            {
                SignedIn = false;

                if (ex.Message.Contains("User canceled authentication.")) return;

                if (ex.Message.Contains("user has forgotten their password"))
                {
                    // open user flow to reset password through the browser
                    Process.Start("https://cadflair.b2clogin.com/cadflair.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_reset&client_id=7f344a2b-d9ed-4ed7-af25-0f3b381f855c&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fcadflair.b2clogin.com%2Foauth2%2Fnativeclient&scope=openid&response_type=code&prompt=login");
                    return;
                }

                MessageBox.Show($"Sign in failed:\n{ex}", "Cadflair", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "SignIn");
            }
            catch (Exception ex)
            {
                SignedIn = false;
                MessageBox.Show($"Sign in failed:\n{ex}", "Cadflair", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "SignIn");
            }
        }

        private static async Task SignOut()
        {
            try
            {
                // SignOut will remove tokens from the token cache from ALL accounts, irrespective of user flow
                IEnumerable<IAccount> accounts = await _publicClientApp.GetAccountsAsync();

                while (accounts.Any())
                {
                    await _publicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await _publicClientApp.GetAccountsAsync();
                }

                SignedIn = false;
            }
            catch (MsalException ex)
            {
                MessageBox.Show($"Sign out failed:\n{ex}", "Cadflair", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "SignOut");
            }
        }

        public static async Task<AuthenticationResult> GetAuthenticationResult()
        {
            try
            {
                if (_authResult == null || DateTimeOffset.Now > _authResult.ExpiresOn)
                {
                    var accounts = await _publicClientApp.GetAccountsAsync(PolicySignIn);
                    _authResult = await _publicClientApp.AcquireTokenSilent(ApiScopes, accounts.FirstOrDefault()).ExecuteAsync();
                    SignedIn = true;
                }

                return _authResult;
            }
            catch (MsalUiRequiredException)
            {
                // User will need to sign in interactively.
                SignedIn = false;
                MessageBox.Show($"Please sign in to continue.", "Cadflair", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            catch (Exception ex)
            {
                SignedIn = false;
                MessageBox.Show($"An unknown error occurred!", "Cadflair", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "GetAccessToken");
                return null;
            }
        }

        #region "UI"

        public static async void SignInButton_OnExecute(NameValueMap Context)
        {
            await SignIn();
        }

        public static async void SignOutButton_OnExecute(NameValueMap Context)
        {
            await SignOut();
        }

        #endregion
    }
}

//private static void DisplayUserInfo(AuthenticationResult authResult)
//{
//    if (authResult != null)
//    {
//        JObject user = ParseIdToken(authResult.IdToken);
//        Log.Info(user.ToString());
//    }
//}

//private static JObject ParseIdToken(string idToken)
//{
//    // Parse the idToken to get user info
//    idToken = idToken.Split('.')[1];
//    idToken = Base64UrlDecode(idToken);
//    return JObject.Parse(idToken);
//}

//private static string Base64UrlDecode(string s)
//{
//    s = s.Replace('-', '+').Replace('_', '/');
//    s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
//    var byteArray = Convert.FromBase64String(s);
//    var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
//    return decoded;
//}

