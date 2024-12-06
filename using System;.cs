using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MicrosoftGraphApiExample
{
    class Program
    {
        static async Task Main()
        {
            // Replace with your Azure AD application credentials
            string clientId = "your_client_id";
            string clientSecret = "your_client_secret";
            string tenantId = "your_tenant_id";

            // Endpoint for getting an access token
            string tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            // Endpoint for fetching emails
            string graphEndpoint = "https://graph.microsoft.com/v1.0/me/messages";

            // Get the access token
            using (var httpClient = new HttpClient())
            {
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));
                tokenRequest.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default")
                });

                var tokenResponse = await httpClient.SendAsync(tokenRequest);
                tokenResponse.EnsureSuccessStatusCode();
                var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();
                var token = JsonDocument.Parse(tokenResponseContent).RootElement.GetProperty("access_token").GetString();

                // Fetch the emails
                using (var graphClient = new HttpClient())
                {
                    graphClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var graphResponse = await graphClient.GetAsync(graphEndpoint);
                    graphResponse.EnsureSuccessStatusCode();
                    var graphResponseContent = await graphResponse.Content.ReadAsStringAsync();

                    // Parse the email list
                    var emailList = JsonDocument.Parse(graphResponseContent).RootElement.EnumerateArray();

                    foreach (var email in emailList)
                    {
                        var senderEmail = email.GetProperty("sender").GetProperty("emailAddress").GetProperty("address").GetString();
                        Console.WriteLine(senderEmail);
                    }
                }
            }
        }
    }
}