using Microsoft.Identity.Client;

public async Task<string> GetAccessToken(string clientId, X509Certificate2 certificate)
{
    var clientApp = PublicClientApplicationBuilder.Create(clientId)
        .WithRedirectUri(new Uri("http://localhost")) // Can be any URI
        .Build();

    var authResult = await clientApp.AcquireTokenByCertificateAsync(new[] { "https://graph.microsoft.com/.default" }, certificate);
    return authResult.AccessToken;
}