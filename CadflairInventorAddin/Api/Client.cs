﻿using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CadflairInventorAddin.Api
{
    internal static class Client
    {

        private static readonly string BaseUrl = "https://localhost:7269";

        private static async Task<string> CallApi(HttpMethod method, string endPoint, HttpContent content = null)
        {
            using (HttpClient client = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage(method, $"{BaseUrl}{endPoint}"))
            {
                // add authorization token to request header
                AuthenticationResult auth = await Authentication.GetAuthenticationResult();
                if (auth == null) throw new Exception("User not signed in!");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

                // add content
                if (content != null) request.Content = content;

                // send the request
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public static Task<string> Get(string endPoint, HttpContent content = null)
        {
            return CallApi(HttpMethod.Get, endPoint, content);
        }

        public static Task<string> Post(string endPoint, HttpContent content = null)
        {
            return CallApi(HttpMethod.Post, endPoint, content);
        }

        public static Task<string> Put(string endPoint, HttpContent content = null)
        {
            return CallApi(HttpMethod.Put, endPoint, content);
        }

    }
}
