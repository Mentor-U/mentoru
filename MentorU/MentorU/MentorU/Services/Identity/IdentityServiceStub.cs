using MentorU.Views;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.Services.Identity
{
    /// <summary>
    /// Implementation of the IIdentityService Interface
    /// </summary>
    class IdentityServiceStub : IIdentityService
    {

        /// <summary>
        /// Driver for authentication v1. If successful, move the user to the main shell pages
        /// </summary>
        /// <returns></returns>
        public async Task<bool> VerifyRegistration()
        {
            await Authenticate();
            if (App.userSignedIn)
            {
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                return true;
            }
            else return false;
        }

        private async Task Authenticate()
        {

            AuthenticationResult authResult = null;
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync();

            try
            {
                // User not logged in yet... 
                if (!App.userSignedIn)
                {
                    try
                    {
                        IAccount firstAccount = accounts.FirstOrDefault();
                        authResult = await App.PCA.AcquireTokenSilent(Constants.Scopes, firstAccount)
                                              .ExecuteAsync();
                    }
                    catch (MsalUiRequiredException)
                    {
                        try
                        {
                            authResult = await App.PCA.AcquireTokenInteractive(Constants.Scopes)
                                                      .WithParentActivityOrWindow(App.UIParent)
                                                      .ExecuteAsync();
                        }
                        catch (Exception ex2)
                        {
                            await App.Current.MainPage.DisplayAlert("Acquire token interactive failed. See exception message for details: ", ex2.Message, "Dismiss");
                        }
                    }

                    if (authResult != null)
                    {
                        var content = await GetHttpContentWithTokenAsync(authResult.AccessToken);
                        UpdateUserContent(content);

                    }
                }
                else
                {
                    while (accounts.Any())
                    {
                        await App.PCA.RemoveAsync(accounts.FirstOrDefault());
                        accounts = await App.PCA.GetAccountsAsync();
                    }

                    App.userSignedIn = false;
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Authentication failed. See exception message for details: ", ex.Message, "Dismiss");
            }
        }


        /// <summary>
        /// Parse the token for MS Auth/Graph use
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<string> GetHttpContentWithTokenAsync(string token)
        {
            try
            {
                //get data from API
                HttpClient client = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("API call to graph failed: ", ex.Message, "Dismiss");
                return ex.ToString();
            }
        }

        /// <summary>
        /// Using a token from MS Graph, parse it and extract/update the apps active user profile.
        /// </summary>
        /// <param name="content"></param>
        private void UpdateUserContent(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                JObject user = JObject.Parse(content);

                App.userSignedIn = true;

                // This is unique. Could be used as our PK for users database as well, that would help us be concise 
                App.ActiveUser.id = user["id"].ToString();
                App.ActiveUser.DisplayName = user["displayName"].ToString();
                App.ActiveUser.FirstName = user["givenName"].ToString();
                App.ActiveUser.LastName = user["surname"].ToString();
                // This is the email... 
                App.ActiveUser.PrincipalName = user["userPrincipalName"].ToString();


            }
        }
    }
}
