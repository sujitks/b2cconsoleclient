using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2CConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Azure B2C Console Client");
                Console.WriteLine("========================");

                var config = ParseCommandLineArguments(args);

                var authService = new AuthenticationService(config);
                
                if (!string.IsNullOrEmpty(config.UserId))
                {
                    Console.WriteLine($"Login hint: {config.UserId}");
                }
                Console.WriteLine($"Tenant: {config.TenantName}");
                Console.WriteLine();

                var token = await authService.AuthenticateAsync();
                
                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Authentication successful!");
                    Console.WriteLine($"JWT Token: {token}");
                    Console.WriteLine();

                    // Test API call if endpoint is provided
                    if (!string.IsNullOrEmpty(config.ApiEndpoints))
                    {
                        Console.WriteLine("Testing API call...");
                        var apiResult = await authService.CallApiAsync(token);
                        Console.WriteLine($"API Response: {apiResult}");
                    }
                }
                else
                {
                    Console.WriteLine("Authentication failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static AuthConfig ParseCommandLineArguments(string[] args)
        {
            var config = ConfigurationHelper.LoadFromConfig();

            if (args.Length == 0)
            {
                Console.WriteLine("No command line arguments provided. Using configuration file values.");
                Console.WriteLine("Interactive authentication will be used.");
                return config;
            }

            var argDict = new Dictionary<string, string>();
            
            for (int i = 0; i < args.Length; i += 2)
            {
                if (i + 1 < args.Length)
                {
                    string key = args[i].TrimStart('-', '/').ToLower();
                    string value = args[i + 1];
                    argDict[key] = value;
                }
            }

            // Override config values with command line arguments
            if (argDict.ContainsKey("userid"))
                config.UserId = argDict["userid"];
            
            if (argDict.ContainsKey("tenantname"))
                config.TenantName = argDict["tenantname"];
            
            if (argDict.ContainsKey("redirecturi"))
                config.RedirectUri = argDict["redirecturi"];
            
            if (argDict.ContainsKey("clientid"))
                config.ClientId = argDict["clientid"];
            
            if (argDict.ContainsKey("policysignupsingin"))
                config.PolicySignUpSignInValue = argDict["policysignupsingin"];
            
            if (argDict.ContainsKey("policyresetpassword"))
                config.PolicyResetPassword = argDict["policyresetpassword"];
            
            if (argDict.ContainsKey("apiscopes"))
                config.ApiScopes = argDict["apiscopes"];
            
            if (argDict.ContainsKey("apiendpoints"))
                config.ApiEndpoints = argDict["apiendpoints"];
            
            if (argDict.ContainsKey("apisubscriptionkey"))
                config.ApiSubscriptionKeys = argDict["apisubscriptionkey"];

            // UserId is optional for interactive authentication (will be used as login hint)

            return config;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("b2c-client [options]");
            Console.WriteLine();
            Console.WriteLine("Optional parameters (will use config file values if not specified):");
            Console.WriteLine("  -userid <value>              User ID hint for interactive authentication");
            Console.WriteLine("  -tenantname <value>          Azure AD B2C tenant name");
            Console.WriteLine("  -redirecturi <value>         Redirect URI for authentication");
            Console.WriteLine("  -clientid <value>            Client ID of the application");
            Console.WriteLine("  -policysignupsingin <value>  Sign up/sign in policy name");
            Console.WriteLine("  -policyresetpassword <value> Password reset policy name");
            Console.WriteLine("  -apiscopes <value>           API scopes (comma-separated)");
            Console.WriteLine("  -apiendpoints <value>        API endpoint URL");
            Console.WriteLine("  -apisubscriptionkey <value>  API subscription key");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("b2c-client -userid user@example.com");
            Console.WriteLine("b2c-client (will use config file values)");
            Console.WriteLine();
            Console.WriteLine("Note: This application uses interactive authentication (opens browser).");
            Console.WriteLine("      All configuration values will be loaded from appsettings.json");
        }
    }
}
