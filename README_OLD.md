# B2C Console Client

This console application provides command-line access to APIs using MSAL (Microsoft Authentication Library) for Azure AD B2C authentication. It's based on the existing WPF application but designed for automated testing and command-line usage.

## Features

- Command-line authentication with Azure AD B2C
- JWT token generation for API access
- Configuration file support with command-line overrides
- Silent token refresh when possible
- API testing capability
- User information display

## Setup

### Prerequisites

- .NET Framework 4.7.2 or higher
- Visual Studio 2017 or higher (for building)
- Azure AD B2C tenant configured
- API application registered in Azure AD B2C

### Configuration

1. Update the `App.config` file in the `B2CConsoleClient` folder with your Azure AD B2C settings:

```xml
<appSettings>
    <add key="TenantName" value="your-tenant-name" />
    <add key="RedirectUri" value="https://your-tenant-name.b2clogin.com/oauth2/nativeclient" />
    <add key="ClientId" value="your-client-id" />
    <add key="PolicySignUpSignIn" value="b2c_1_signupsignin" />
    <add key="PolicyResetPassword" value="b2c_1_passwordreset" />
    <add key="ApiScopes" value="https://your-tenant-name.onmicrosoft.com/your-api/access_as_user" />
    <add key="ApiEndpoint" value="https://your-api-endpoint.com/api/test" />
    <add key="ApiSubscriptionKey" value="your-subscription-key" />
</appSettings>
```

2. Build the solution:
   - Open `B2CConsoleClient.sln` in Visual Studio
   - Build the solution (Ctrl+Shift+B)
   - Or use MSBuild from command line: `msbuild B2CConsoleClient.sln`

## Usage

### Basic Usage

The application requires at minimum a user ID and password:

```bash
B2CConsoleClient.exe -userid user@example.com -password mypassword
```

### Full Command Line Options

```bash
B2CConsoleClient.exe -userid <userid> -password <password> [options]
```

#### Required Parameters

- `-userid <value>` - User ID for authentication
- `-password <value>` - Password for authentication

#### Optional Parameters

All optional parameters will use values from the configuration file if not specified:

- `-tenantname <value>` - Azure AD B2C tenant name
- `-redirecturi <value>` - Redirect URI for authentication
- `-clientid <value>` - Client ID of the application
- `-policysignupsingin <value>` - Sign up/sign in policy name
- `-policyresetpassword <value>` - Password reset policy name
- `-apiscopes <value>` - API scopes (comma-separated)
- `-apiendpoints <value>` - API endpoint URL
- `-apisubscriptionkey <value>` - API subscription key

### Examples

1. **Basic authentication using config file values:**
```bash
B2CConsoleClient.exe -userid john.doe@example.com -password MySecurePassword123
```

2. **Override specific configuration values:**
```bash
B2CConsoleClient.exe -userid john.doe@example.com -password MySecurePassword123 -tenantname mycompany -clientid abc123def456
```

3. **Full parameter specification:**
```bash
B2CConsoleClient.exe -userid john.doe@example.com -password MySecurePassword123 -tenantname mycompany -clientid abc123def456 -apiscopes "https://mycompany.onmicrosoft.com/api/read" -apiendpoints "https://api.mycompany.com/test"
```

## Output

The application will display:

1. Authentication status
2. User information (name, email, etc.)
3. JWT access token
4. API test results (if endpoint is configured)

Example output:
```
B2C Console Client
==============================
Authenticating user: john.doe@example.com
Tenant: mycompany

Authentication successful!
User Information:
  Name: John Doe
  User Identifier: 12345678-1234-1234-1234-123456789012
  Email: john.doe@example.com
  Identity Provider: https://mycompany.b2clogin.com/...

JWT Token: eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6...

Testing API call...
API Response: Status: OK
Content: {"message": "API call successful", "data": {...}}
```

## Token Caching

The application uses MSAL's token caching mechanism to store tokens securely on the local machine. Subsequent runs will attempt silent authentication first, falling back to interactive authentication if needed.

## Error Handling

Common error scenarios and their meanings:

- **Authentication failed**: Invalid credentials or configuration
- **AADB2C90118**: Password reset required - user needs to reset password
- **MsalUiRequiredException**: Interactive authentication required
- **API call failed**: Check API endpoint, subscription key, and token validity

## Security Considerations

- Passwords are passed via command line and should be handled securely in production
- Consider using environment variables or secure credential storage for automated scenarios
- Tokens are cached locally using Windows Data Protection API (DPAPI)
- Always use HTTPS endpoints in production

## Troubleshooting

1. **Check configuration values** in App.config
2. **Verify Azure AD B2C setup** - ensure policies and app registrations are correct
3. **Check network connectivity** to Azure AD B2C endpoints
4. **Review MSAL logs** - the application logs MSAL activities for debugging

## Building from Source

1. Clone or download the repository
2. Restore NuGet packages: `nuget restore`
3. Build the solution: `msbuild B2CConsoleClient.sln`
4. The executable will be in `B2CConsoleClient\bin\Debug\` or `B2CConsoleClient\bin\Release\`

## Dependencies

- Microsoft.Identity.Client 4.31.0
- Newtonsoft.Json 13.0.1
- .NET Framework 4.7.2

## License

See LICENSE file for license information.
