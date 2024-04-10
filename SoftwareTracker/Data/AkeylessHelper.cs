using akeyless.Client;
using akeyless.Api;
using akeyless.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;

namespace SoftwareTracker.Data
{
    public sealed class AkeylessHelper
    {
        //string token = "";
        //V2Api instance = null;
        static V2Api instance;
        public static string RetrieveEncryptionKey()
        {
            string token = AuthenticateToAKeyless();
            List<String> secrets = new List<String>();
            secrets.Add("SoftwareTracker/EncryptionKey");
            var getSecretValueBody = new GetSecretValue(names: secrets, token: token);
            Dictionary<string, string> getSecretValueResult = instance.GetSecretValue(getSecretValueBody);
            return getSecretValueResult["SoftwareTracker/EncryptionKey"];
        }

        public static string RetrieveConnectionString()
        {
            string token = AuthenticateToAKeyless();
            List<String> secrets = new List<String>();
            secrets.Add("SoftwareTracker/ConnectionString");
            var getSecretValueBody = new GetSecretValue(names: secrets, token: token);
            Dictionary<string, string> getSecretValueResult = instance.GetSecretValue(getSecretValueBody);
            return getSecretValueResult["SoftwareTracker/ConnectionString"];
        }
        public static string RetrieveSendGridToken()
        {
            string token = AuthenticateToAKeyless();
            List<String> secrets = new List<String>();
            secrets.Add("SoftwareTracker/SendGridAPI");
            var getSecretValueBody = new GetSecretValue(names: secrets, token: token);
            Dictionary<string, string> getSecretValueResult = instance.GetSecretValue(getSecretValueBody);
            return getSecretValueResult["SoftwareTracker/SendGridAPI"];
        }
        private static string AuthenticateToAKeyless()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.akeyless.io";
            instance = new V2Api(config);

            var authBody = new Auth(accessId: Environment.GetEnvironmentVariable("STAccessID"), accessKey: Environment.GetEnvironmentVariable("STAccessKey"));
            AuthOutput authResult = instance.Auth(authBody);
            string token = authResult.Token;
            return token;
        }
    }
}
