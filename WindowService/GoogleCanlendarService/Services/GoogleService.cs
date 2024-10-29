using Google.Apis.Auth.OAuth2;
using System.Configuration;
using System.IO;

namespace GoogleCanlendarService.Services
{
    public class GoogleService
    {
        readonly string credentialJson = ConfigurationManager.AppSettings["GoogleCredentialsJson"];
        public GoogleCredential GetCredential(params string[] scopes)
        {
            // Load the credentials from the service account key file
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(credentialJson)))
            {
                var credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(scopes);
                return credential;
            }
        }
    }
}
