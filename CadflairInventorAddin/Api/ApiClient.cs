using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CadflairInventorAddin.Api
{
    internal static class ApiClient
    {

        private static readonly string BaseUrl = "https://localhost:7269";

        private static async Task<string> CallApi(HttpMethod method, string uri, HttpContent content = null)
        {
            using (HttpClient client = new HttpClient())
            using (HttpRequestMessage request = new HttpRequestMessage(method, $"{BaseUrl}{uri}"))
            {
                // add authorization token to request header
                string token = await AuthenticationApi.GetAccessToken();
                if (string.IsNullOrWhiteSpace(token)) return default;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

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

        public static Task<string> Get(string uri, HttpContent content = null)
        {
            return CallApi(HttpMethod.Get, uri, content);
        }

        public static Task<string> Post(string uri, HttpContent content = null)
        {
            return CallApi(HttpMethod.Post, uri, content);
        }

        public static Task<string> Put(string uri, HttpContent content = null)
        {
            return CallApi(HttpMethod.Put, uri, content);
        }

    }
}
