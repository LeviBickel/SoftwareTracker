using akeyless.Client;
using akeyless.Api;
using akeyless.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SoftwareTracker.Data
{
    public sealed class AkeylessHelper
    {
        //string token = "";
        //V2Api instance = null;

        public static string RetrieveEncryptionKey()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.akeyless.io";
            var instance = new V2Api(config);

            var authBody = new Auth(accessId: Environment.GetEnvironmentVariable("STAccessID"), accessKey: Environment.GetEnvironmentVariable("STAccessKey"));
            AuthOutput authResult = instance.Auth(authBody);
            string token = authResult.Token;
            List<String> secrets = new List<String>();
            secrets.Add("SoftwareTracker/EncryptionKey");
            var getSecretValueBody = new GetSecretValue(names: secrets, token: token);
            Dictionary<string, string> getSecretValueResult = instance.GetSecretValue(getSecretValueBody);
            //Console.WriteLine(getSecretValueResult["netcore"]);
            return getSecretValueResult["SoftwareTracker/EncryptionKey"];
        }

        public static string RetrieveConnectionString()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.akeyless.io";
            var instance = new V2Api(config);

            var authBody = new Auth(accessId: Environment.GetEnvironmentVariable("STAccessID"), accessKey: Environment.GetEnvironmentVariable("STAccessKey"));
            AuthOutput authResult = instance.Auth(authBody);
            string token = authResult.Token;
            List<String> secrets = new List<String>();
            secrets.Add("SoftwareTracker/ConnectionString");
            var getSecretValueBody = new GetSecretValue(names: secrets, token: token);
            Dictionary<string, string> getSecretValueResult = instance.GetSecretValue(getSecretValueBody);
            //Console.WriteLine(getSecretValueResult["netcore"]); 
            return getSecretValueResult["SoftwareTracker/ConnectionString"];
        }
    }
}
