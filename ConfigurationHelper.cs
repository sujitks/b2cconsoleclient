using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace B2CConsoleClient
{
    public class AuthConfig
    {
        public string UserId { get; set; }
        public string TenantName { get; set; }
        public string RedirectUri { get; set; }
        public string ClientId { get; set; }
        public string PolicySignUpSignInValue { get; set; }
        public string PolicyResetPassword { get; set; }
        public string ApiScopes { get; set; }
        public string ApiEndpoints { get; set; }
        public string ApiSubscriptionKeys { get; set; }
    }

    public static class ConfigurationHelper
    {
        private static readonly string ConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

        public static AuthConfig LoadFromConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var config = new AuthConfig
            {
                TenantName = configuration["AppSettings:TenantName"] ?? "",
                RedirectUri = configuration["AppSettings:RedirectUri"] ?? "http://localhost",
                ClientId = configuration["AppSettings:ClientId"] ?? "",
                PolicySignUpSignInValue = configuration["AppSettings:PolicySignUpSignIn"] ?? "",
                PolicyResetPassword = configuration["AppSettings:PolicyResetPassword"] ?? "",
                ApiScopes = configuration["AppSettings:ApiScopes"] ?? "",
                ApiEndpoints = configuration["AppSettings:APiEndpoint"] ?? "",
                ApiSubscriptionKeys = configuration["AppSettings:ApiSubscriptionKey"] ?? ""
            };

            // Check if required configuration is missing and prompt for it
            if (IsConfigurationMissing(config))
            {
                Console.WriteLine("Configuration is missing or incomplete. Let's set it up:");
                Console.WriteLine();
                
                config = PromptForConfiguration(config);
                SaveConfiguration(config);
                
                Console.WriteLine();
                Console.WriteLine("Configuration saved! You can now run the application.");
                Console.WriteLine();
            }

            return config;
        }

        private static bool IsConfigurationMissing(AuthConfig config)
        {
            return string.IsNullOrWhiteSpace(config.TenantName) ||
                   string.IsNullOrWhiteSpace(config.ClientId) ||
                   string.IsNullOrWhiteSpace(config.PolicySignUpSignInValue) ||
                   string.IsNullOrWhiteSpace(config.ApiScopes);
        }

        private static AuthConfig PromptForConfiguration(AuthConfig existingConfig)
        {
            var config = new AuthConfig();

            config.TenantName = PromptForValue("Azure B2C Tenant Name (e.g., myb2ctenant)", existingConfig.TenantName);
            config.ClientId = PromptForValue("Application (Client) ID", existingConfig.ClientId);
            config.PolicySignUpSignInValue = PromptForValue("Sign-up/Sign-in Policy (e.g., B2C_1_signupsignin1)", existingConfig.PolicySignUpSignInValue);
            config.PolicyResetPassword = PromptForValue("Password Reset Policy (optional)", existingConfig.PolicyResetPassword, true);
            config.ApiScopes = PromptForValue("API Scopes (e.g., https://yourtenant.onmicrosoft.com/api/read)", existingConfig.ApiScopes);
            config.ApiEndpoints = PromptForValue("API Endpoint URL (optional)", existingConfig.ApiEndpoints, true);
            config.ApiSubscriptionKeys = PromptForValue("API Subscription Key (optional)", existingConfig.ApiSubscriptionKeys, true);
            
            // Set default redirect URI for interactive authentication
            config.RedirectUri = "http://localhost";

            return config;
        }

        private static string PromptForValue(string promptText, string existingValue, bool optional = false)
        {
            var optionalText = optional ? " (optional)" : "";
            var defaultText = !string.IsNullOrWhiteSpace(existingValue) ? $" [current: {existingValue}]" : "";
            
            Console.Write($"{promptText}{optionalText}{defaultText}: ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                return existingValue ?? "";
            }
            
            return input.Trim();
        }

        private static void SaveConfiguration(AuthConfig config)
        {
            try
            {
                var settings = new
                {
                    AppSettings = new
                    {
                        TenantName = config.TenantName,
                        RedirectUri = config.RedirectUri,
                        ClientId = config.ClientId,
                        PolicySignUpSignIn = config.PolicySignUpSignInValue,
                        PolicyResetPassword = config.PolicyResetPassword,
                        ApiScopes = config.ApiScopes,
                        APiEndpoint = config.ApiEndpoints,
                        ApiSubscriptionKey = config.ApiSubscriptionKeys
                    }
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(ConfigFilePath, json);
                
                Console.WriteLine($"Configuration saved to {ConfigFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
            }
        }
    }
}
