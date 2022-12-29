// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Inventor;
using Microsoft.Identity.Client;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CadflairInventorAddin.Helpers
{
    internal class AzureB2CHelper
    {
        private static readonly string TenantName = "cadflair";
        private static readonly string Tenant = $"{TenantName}.onmicrosoft.com";
        private static readonly string AzureAdB2CHostname = $"{TenantName}.b2clogin.com";
        private static readonly string ClientId = "7f344a2b-d9ed-4ed7-af25-0f3b381f855c";
        private static readonly string RedirectUri = $"https://{TenantName}.b2clogin.com/oauth2/nativeclient";
        private static readonly string PolicySignUpSignIn = "b2c_1_susi";
        private static readonly string PolicyEditProfile = "b2c_1_edit";
        private static readonly string PolicyResetPassword = "b2c_1_reset";
        private static readonly string AuthorityBase = $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";
        private static readonly string AuthoritySignUpSignIn = $"{AuthorityBase}{PolicySignUpSignIn}";
        private static readonly string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        private static readonly string AuthorityResetPassword = $"{AuthorityBase}{PolicyResetPassword}";
        //public static string[] ApiScopes = { $"https://{Tenant}/helloapi/Data.Read", $"https://{Tenant}/helloapi/Data.Write" };
        //public static string ApiEndpoint = "https://fabrikamb2chello.azurewebsites.net/hello";

        private static AuthenticationResult _authState;
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
                if (!value) _authState = null;
                _signedIn = value;
                SignInButton.Enabled = !value;
                SignOutButton.Enabled = value;
            }
        }

        public static ButtonDefinition SignInButton { get; set; }
        public static ButtonDefinition SignOutButton { get; set; }

        public static void InitializeAzureB2C()
        {
            _publicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithB2CAuthority(AuthoritySignUpSignIn)
                .WithRedirectUri(RedirectUri)
                .Build();

            TokenCacheHelper.Bind(_publicClientApp.UserTokenCache);
        }

        private static async Task SignIn()
        {
            try
            {
                _authState = await _publicClientApp.AcquireTokenInteractive(null)
                                                   .WithParentActivityOrWindow(new InventorMainFrame(Globals.InventorApplication.MainFrameHWND))
                                                   .ExecuteAsync();

                SignedIn = true;
            }
            catch (MsalException ex)
            {
                SignedIn = false;

                if (ex.Message.Contains("AADB2C90118"))
                {
                    await ResetPassword();
                }
                else
                {
                    MessageBox.Show($"Sign in failed:\n{ex}", "Cadflair", MessageBoxButton.OK, MessageBoxImage.Error);
                    Trace.TraceError($"SignIn failed: \n{ex}\n");
                }
            }
            catch (Exception ex)
            {
                SignedIn = false;
                MessageBox.Show($"Sign in failed:\n{ex}", "Cadflair", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.TraceError($"SignIn failed: \n{ex}\n");
            }
        }

        private static async Task ResetPassword()
        {
            try
            {
                _authState = await _publicClientApp.AcquireTokenInteractive(null)
                                                   .WithParentActivityOrWindow(new InventorMainFrame(Globals.InventorApplication.MainFrameHWND))
                                                   .WithPrompt(Prompt.SelectAccount)
                                                   .WithB2CAuthority(AuthorityResetPassword)
                                                   .ExecuteAsync();

                SignedIn = true;
            }
            catch (Exception ex)
            {
                SignedIn = false;
                MessageBox.Show($"Password reset failed:\n{ex}", "Cadflair", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.TraceError($"ResetPassword failed: \n{ex}\n");
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
                Trace.TraceError($"SignOut failed: \n{ex}\n");
            }
        }

        public static async Task<string> GetAccessToken()
        {
            try
            {
                if (_authState == null || DateTimeOffset.Now > _authState.ExpiresOn)
                {
                    var accounts = await _publicClientApp.GetAccountsAsync(PolicySignUpSignIn);
                    _authState = await _publicClientApp.AcquireTokenSilent(null, accounts.FirstOrDefault()).ExecuteAsync();
                    SignedIn = true;
                }

                return _authState.AccessToken;
            }
            catch (MsalUiRequiredException)
            {
                // User will need to sign in interactively.
                SignedIn = false;
                MessageBox.Show($"User not signed in!", "Cadflair", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            catch (Exception ex)
            {
                SignedIn = false;
                MessageBox.Show($"Authentication failed:\n{ex}", "Cadflair", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.TraceError($"GetAccessToken failed: \n{ex}\n");
                return null;
            }
        }


        //private void DisplayUserInfo(AuthenticationResult authResult)
        //{
        //    if (authResult != null)
        //    {
        //        JObject user = ParseIdToken(authResult.IdToken);
        //        ConnectionRichTextBox.Document.Blocks.Clear();
        //        ConnectionRichTextBox.AppendText($"Name: {user["name"]?.ToString()}\n");
        //        ConnectionRichTextBox.AppendText($"User Identifier: {user["oid"]?.ToString()}\n");
        //        ConnectionRichTextBox.AppendText($"Identity Provider: {user["iss"]?.ToString()}\n");
        //    }
        //}

        //JObject ParseIdToken(string idToken)
        //{
        //    // Parse the idToken to get user info
        //    idToken = idToken.Split('.')[1];
        //    idToken = Base64UrlDecode(idToken);
        //    return JObject.Parse(idToken);
        //}

        //private string Base64UrlDecode(string s)
        //{
        //    s = s.Replace('-', '+').Replace('_', '/');
        //    s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
        //    var byteArray = Convert.FromBase64String(s);
        //    var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
        //    return decoded;
        //}

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
