
            // Replace with your Azure AD application ID and certificate thumbprint
            string clientId = "your_client_id";
            string certificateThumbprint = "your_certificate_thumbprint";

            // Load the certificate
            X509Certificate2 certificate = new X509Certificate2(certificateThumbprint);

            // Create a ClientCredentialProvider using the certificate
            ClientCredentialProvider authProvider = new ClientCredentialProvider(clientId, certificate);

            // Create a GraphServiceClient
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            // Get the first 10 emails
            var messages = await graphClient.Me.Messages
                .Request()
                .Top(10)
                .GetAsync();

            foreach (var message in messages)
            {
                Console.WriteLine($"Subject: {message.Subject}");
                Console.WriteLine($"Sender: {message.Sender.EmailAddress.Address}");
                // ... other email properties
            }
        }