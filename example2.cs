using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace MicrosoftGraphApiExample
{
    class Program
    {
        static async Task Main()
        {
            // Replace with your Azure AD application ID and certificate thumbprint
            string clientId = "your_client_id";
            string certificateThumbprint = "your_certificate_thumbprint";

            // Endpoint for getting an access token
            string tokenEndpoint = $"https://login.microsoftonline.com/common/oauth2/v2.0/token";

            // Endpoint for fetching emails
            string graphEndpoint = "https://graph.microsoft.com/v1.0/me/messages";

            // Load the certificate
            X509Certificate2 certificate = new X509Certificate2(certificateThumbprint);

            // Get the access token
            using (var httpClient = new HttpClient())
            {
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken(clientId, certificate));

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

        private static string GetAccessToken(string clientId, X509Certificate2 certificate)
        {
            // Implement logic to obtain access token using certificate-based authentication
            // This might involve using libraries like ADAL or MSAL.NET

            // For simplicity, let's assume a hypothetical method to obtain the token:
            string accessToken = "your_access_token"; // Replace with actual token retrieval logic

            return accessToken;
        }
    }
}