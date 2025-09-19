# B2C Console Client - Interactive Configuration Setup

This .NET 9 console application demonstrates how to interact with an Azure B2C protected API using interactive authentication.

## Features

- **Interactive Configuration Setup**: On first run, the application will prompt you to enter all required configuration values
- **Cross-Platform Token Caching**: Securely stores authentication tokens using platform-specific encryption (Windows DPAPI or file permissions on macOS/Linux)
- **Interactive Authentication**: Uses browser-based authentication flow that works with Azure B2C
- **Modern .NET**: Built with .NET 9 and modern configuration patterns

## First Time Setup

When you run the application for the first time with blank configuration values, it will automatically prompt you to enter:

1. **Azure B2C Tenant Name** (required) - Your B2C tenant name (e.g., "myb2ctenant")
2. **Application (Client) ID** (required) - Your Azure AD application client ID
3. **Sign-up/Sign-in Policy** (required) - Your B2C policy name (e.g., "B2C_1_signupsignin1")
4. **Password Reset Policy** (optional) - Your B2C password reset policy
5. **API Scopes** (required) - The scopes for your API (e.g., "https://yourtenant.onmicrosoft.com/api/read")
6. **API Endpoint URL** (optional) - Your API endpoint URL
7. **API Subscription Key** (optional) - If your API requires a subscription key

### Example First Run

```
B2C Console Client
==============================
Configuration is missing or incomplete. Let's set it up:

Azure B2C Tenant Name (e.g., myb2ctenant): mycompany
Application (Client) ID: 12345678-1234-1234-1234-123456789012
Sign-up/Sign-in Policy (e.g., B2C_1_signupsignin1): B2C_1_signupsignin1
Password Reset Policy (optional): B2C_1_passwordreset
API Scopes (e.g., https://yourtenant.onmicrosoft.com/api/read): https://mycompany.onmicrosoft.com/api/read
API Endpoint URL (optional): https://api.mycompany.com/api
API Subscription Key (optional): your-key-here

Configuration saved to /path/to/appsettings.json
Configuration saved! You can now run the application.
```

After the initial setup, the configuration is saved to `appsettings.json` and subsequent runs will use the saved values.

## Running the Application

1. **Build the project:**
   ```bash
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

## Configuration File Structure

The application uses `appsettings.json` with the following structure:

```json
{
  "AppSettings": {
    "TenantName": "your-tenant-name",
    "RedirectUri": "http://localhost",
    "ClientId": "your-client-id",
    "PolicySignUpSignIn": "B2C_1_signupsignin1",
    "PolicyResetPassword": "B2C_1_passwordreset",
    "ApiScopes": "https://yourtenant.onmicrosoft.com/api/read",
    "APiEndpoint": "https://api.yourdomain.com/api",
    "ApiSubscriptionKey": "your-subscription-key"
  }
}
```

## Reconfiguring the Application

To reconfigure the application, you can:

1. **Delete the configuration file** and run the application to go through setup again
2. **Edit `appsettings.json`** directly with your new values
3. **Clear values** in `appsettings.json` (set them to empty strings) to trigger the interactive setup again

## Azure B2C Setup Requirements

Make sure your Azure B2C application is configured with:
- **Redirect URI**: `http://localhost`
- **Application Type**: Public client/native
- **Authentication flows**: Interactive authentication must be enabled

## Troubleshooting

- **Authentication errors**: Check your Azure B2C configuration and ensure redirect URIs match
- **API call errors**: Verify your API endpoint and subscription key configuration
- **Cross-platform issues**: The token cache uses different encryption methods on different platforms (Windows DPAPI vs. file permissions)

## Dependencies

- Microsoft.Identity.Client 4.67.1
- Microsoft.Extensions.Configuration 9.0.0
- System.Text.Json 9.0.0
- System.Security.Cryptography.ProtectedData 9.0.0 (Windows-specific encryption)
