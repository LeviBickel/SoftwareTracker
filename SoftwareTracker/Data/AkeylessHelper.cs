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
        static V2Api instance;

        public static string RetrieveSecret(string secret)
        {
            string token = AuthenticateToAKeyless();
            List<String> secrets = new List<String>();
            secrets.Add($"SoftwareTracker/{secret}");
            var getSecretValueBody = new GetSecretValue(names: secrets, token: token);
            Dictionary<string, string> getSecretValueResult = instance.GetSecretValue(getSecretValueBody);
            return getSecretValueResult[$"SoftwareTracker/{secret}"];
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
