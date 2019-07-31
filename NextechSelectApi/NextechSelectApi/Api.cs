using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NextechSelectApi
{
    // Perhaps this should be in its own file?
    public interface IApi
    {
        string GetPatients();
    }
    // .........................
    
    public class Api : IApi
    {
        private static HttpClient httpClient;

        public Api(string practiceId, string clientId, string username, string password)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://select.nextech-api.com/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
            httpClient.DefaultRequestHeaders.Add("nx-practice-id", practiceId);

            var tokens = GetAuthenticationToken(clientId, username, password);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        }

        private TokenResult GetAuthenticationToken(string clientId, string username, string password)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://login.microsoftonline.com/nextech-api.com/oauth2/token");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var resource = "https://select.nextech-api.com/api";

                var credentials = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", clientId },
                    { "username", username },
                    { "password", password },
                    { "resource", resource }
                };

                var qs = $"?grant_type=password&client_id={clientId}&username={username}&password={password}&resource={resource}";

                HttpResponseMessage response = client.PostAsync(qs, new FormUrlEncodedContent(credentials)).Result;

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error retrieving access token");
                }

                var result = JsonConvert.DeserializeObject<TokenResult>(response.Content.ReadAsStringAsync().Result);

                return result;
            }
        }

        public string GetPatients()
        {
            var apiUrl = $"api/Patient?_getpageoffset=0&_count=50";

            var result = httpClient.GetAsync(apiUrl).Result;

            return result.Content.ReadAsStringAsync().Result;
        }
    }
}
