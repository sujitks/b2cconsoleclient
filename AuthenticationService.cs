using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace B2CConsoleClient
{
    public class AuthenticationService
    {
        private readonly AuthConfig _config;
        private readonly IPublicClientApplication _app;
        private readonly string[] _scopes;

        public AuthenticationService(AuthConfig config)
        {
            _config = config;
            _scopes = new[] { config.ApiScopes };

            // Build the authority URLs
            var tenant = $"{config.TenantName}.onmicrosoft.com";
            var azureAdB2CHostname = $"{config.TenantName}.b2clogin.com";
            var authorityBase = $"https://{azureAdB2CHostname}/tfp/{tenant}/";
            var authoritySignUpSignIn = $"{authorityBase}{config.PolicySignUpSignInValue}";

            // Create the public client application with verbose logging
            _app = PublicClientApplicationBuilder.Create(config.ClientId)
                .WithB2CAuthority(authoritySignUpSignIn)
                .WithRedirectUri(config.RedirectUri)
                .WithLogging(Log, LogLevel.Verbose, true) // Enable PII logging and verbose level
                .Build();

            // Bind token cache
            TokenCacheHelper.Bind(_app.UserTokenCache);
        }

        public async Task<string> AuthenticateAsync()
        {
            try
            {
                // First try silent authentication
                var accounts = await _app.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();

                AuthenticationResult result = null;

                if (firstAccount != null)
                {
                    try
                    {
                        result = await _app.AcquireTokenSilent(_scopes, firstAccount)
                            .ExecuteAsync();
                        
                        Console.WriteLine("Silent authentication successful.");
                        return result.AccessToken;
                    }
                    catch (MsalUiRequiredException)
                    {
                        // Silent authentication failed, need interactive authentication
                        Console.WriteLine("Silent authentication failed. Performing interactive authentication...");
                    }
                }

                // Interactive authentication - opens browser
                Console.WriteLine("Opening browser for interactive authentication...");
                Console.WriteLine("Please complete the sign-in process in your browser.");
                
                var interactiveRequest = _app.AcquireTokenInteractive(_scopes);
                
                // If userId is provided, use it as a login hint
                if (!string.IsNullOrEmpty(_config.UserId))
                {
                    interactiveRequest = interactiveRequest.WithLoginHint(_config.UserId);
                }
                
                result = await interactiveRequest.ExecuteAsync();

                if (result != null)
                {
                    DisplayUserInfo(result);
                    return result.AccessToken;
                }
            }
            catch (MsalException ex)
            {
                Console.WriteLine($"MSAL Exception Details:");
                Console.WriteLine($"  Error Code: {ex.ErrorCode}");
                Console.WriteLine($"  Message: {ex.Message}");
                
                if (ex is MsalServiceException serviceEx)
                {
                    Console.WriteLine($"  HTTP Status Code: {serviceEx.StatusCode}");
                    Console.WriteLine($"  Correlation ID: {serviceEx.CorrelationId}");
                    Console.WriteLine($"  Response Body: {serviceEx.ResponseBody}");
                    Console.WriteLine($"  Claims: {serviceEx.Claims}");
                }
                
                if (ex.Message.Contains("AADB2C90118")) // Password reset required
                {
                    Console.WriteLine("Password reset may be required. Please reset your password through the web interface.");
                }
                else if (ex.ErrorCode == "access_denied")
                {
                    Console.WriteLine("User cancelled the authentication or access was denied.");
                }
                else if (ex.ErrorCode == "unauthorized_client")
                {
                    Console.WriteLine("The application may not be configured correctly for interactive authentication.");
                    Console.WriteLine("Check Azure B2C application settings and redirect URIs.");
                }
                
                Console.WriteLine($"Full Exception: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Authentication error: {ex.Message}");
                Console.WriteLine($"Full Exception: {ex}");
            }

            return null;
        }

        public async Task<string> CallApiAsync(string accessToken)
        {
            if (string.IsNullOrEmpty(_config.ApiEndpoints))
            {
                return "No API endpoint configured.";
            }

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, _config.ApiEndpoints);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    
                    if (!string.IsNullOrEmpty(_config.ApiSubscriptionKeys))
                    {
                        request.Headers.Add("Ocp-Apim-Subscription-Key", _config.ApiSubscriptionKeys);
                    }

                    var response = await httpClient.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();
                    
                    return $"Status: {response.StatusCode}\nContent: {content}";
                }
                catch (Exception ex)
                {
                    return $"API call failed: {ex.Message}";
                }
            }
        }

        private void DisplayUserInfo(AuthenticationResult authResult)
        {
            try
            {
                if (authResult?.IdToken != null)
                {
                    var user = ParseIdToken(authResult.IdToken);

                    Console.WriteLine("User Information:");
                    Console.WriteLine($"  Name: {GetClaimValue(user, "name")}");
                    Console.WriteLine($"  User Identifier: {GetClaimValue(user, "oid")}");
                    Console.WriteLine($"  Street Address: {GetClaimValue(user, "streetAddress")}");
                    Console.WriteLine($"  City: {GetClaimValue(user, "city")}");
                    Console.WriteLine($"  State: {GetClaimValue(user, "state")}");
                    Console.WriteLine($"  Country: {GetClaimValue(user, "country")}");
                    Console.WriteLine($"  Job Title: {GetClaimValue(user, "jobTitle")}");

                    var emails = GetClaimArrayValue(user, "emails");
                    if (emails != null && emails.Count > 0)
                    {
                        Console.WriteLine($"  Email: {emails[0]}");
                    }
                    Console.WriteLine($"  Identity Provider: {GetClaimValue(user, "iss")}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing user info: {ex.Message}");
            }
        }

        private Dictionary<string, object> ParseIdToken(string idToken)
        {
            try
            {
                // Parse the idToken to get user info
                var tokenParts = idToken.Split('.');
                if (tokenParts.Length > 1)
                {
                    var payload = tokenParts[1];
                    payload = Base64UrlDecode(payload);
                    var jsonDocument = JsonDocument.Parse(payload);
                    return JsonElementToDictionary(jsonDocument.RootElement);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing ID token: {ex.Message}");
            }
            return new Dictionary<string, object>();
        }

        private Dictionary<string, object> JsonElementToDictionary(JsonElement element)
        {
            var dictionary = new Dictionary<string, object>();
            
            foreach (var property in element.EnumerateObject())
            {
                dictionary[property.Name] = property.Value.ValueKind switch
                {
                    JsonValueKind.String => property.Value.GetString(),
                    JsonValueKind.Number => property.Value.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Array => property.Value.EnumerateArray().Select(x => x.GetString()).ToList(),
                    _ => property.Value.ToString()
                };
            }
            
            return dictionary;
        }

        private string GetClaimValue(Dictionary<string, object> claims, string claimName)
        {
            return claims.TryGetValue(claimName, out var value) ? value?.ToString() : null;
        }

        private List<string> GetClaimArrayValue(Dictionary<string, object> claims, string claimName)
        {
            if (claims.TryGetValue(claimName, out var value) && value is List<string> list)
            {
                return list;
            }
            return null;
        }

        private string Base64UrlDecode(string input)
        {
            var output = input.Replace('-', '+').Replace('_', '/');
            output = output.PadRight(output.Length + (4 - output.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(output);
            return Encoding.UTF8.GetString(byteArray);
        }

        private void Log(LogLevel level, string message, bool containsPii)
        {
            // Enhanced console logging for debugging
            var piiIndicator = containsPii ? "[PII]" : "";
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            
            // Log all levels for debugging
            Console.WriteLine($"[{timestamp}] MSAL {level}{piiIndicator}: {message}");
        }
    }
}
