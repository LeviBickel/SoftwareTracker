using akeyless.Client;
using akeyless.Api;
using akeyless.Model;

namespace SoftwareTracker.Data
{
    public class AkeylessHelper
    {
        //string token = "";
        //V2Api instance = null;

        public static string RetrieveEncryptionKey()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://api.akeyless.io";
            var instance = new V2Api(config);

            var authBody = new Auth(accessId: "p-gfpvtk2pxdkkam", accessKey: "wCjh3eWZ+BBmh+ZjyBXJjy834bkuq4RTQfdKq+HIYZ8=");
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

            var authBody = new Auth(accessId: "p-gfpvtk2pxdkkam", accessKey: "wCjh3eWZ+BBmh+ZjyBXJjy834bkuq4RTQfdKq+HIYZ8=");
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
