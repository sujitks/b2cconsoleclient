# Azure B2C Console Client

[![NuGet](https://img.shields.io/nuget/v/B2CConsoleClient.svg)](https://www.nuget.org/packages/B2CConsoleClient/)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET global tool for Azure B2C authentication and JWT token generation for API access. Features interactive configuration setup, cross-platform token caching, and browser-based authentication.

## 🚀 Quick Start

### Installation

Install as a global .NET tool:

```bash
dotnet tool install -g B2CConsoleClient
```

### Usage

Once installed, run from anywhere:

```bash
b2c-client
```

On first run, the tool will guide you through an interactive setup process to configure your Azure B2C settings.

## ✨ Features

- **🔧 Interactive Setup**: First-time configuration wizard with guided prompts
- **🌐 Cross-Platform**: Works on Windows, macOS, and Linux with platform-specific secure token caching
- **🔐 Azure B2C Authentication**: Browser-based interactive authentication with MSAL
- **📱 Modern .NET**: Built with .NET 9 using modern patterns and dependencies
- **🛡️ Secure Token Storage**: Encrypted token caching (Windows DPAPI / Unix file permissions)
- **🎯 Global Tool**: Install once, use anywhere via command line

## 📋 Configuration

### Interactive Setup (Recommended)

Run the tool and follow the prompts:

```bash
b2c-client
```

You'll be asked to provide:
- Azure B2C Tenant Name
- Application (Client) ID  
- Sign-up/Sign-in Policy
- API Scopes
- API Endpoint (optional)
- API Subscription Key (optional)

### Command Line Arguments

Override configuration values:

```bash
b2c-client -userid user@example.com -tenantname mytenant -clientid your-client-id
```

Available parameters:
- `-userid` - Login hint for authentication
- `-tenantname` - Azure B2C tenant name
- `-clientid` - Application client ID
- `-policysignupsingin` - Sign-up/sign-in policy
- `-apiscopes` - API scopes
- `-apiendpoints` - API endpoint URL
- `-apisubscriptionkey` - API subscription key

### Manual Configuration

Edit `appsettings.json` directly in the tool's working directory:

```json
{
  "AppSettings": {
    "TenantName": "your-tenant-name",
    "ClientId": "your-client-id",
    "PolicySignUpSignIn": "B2C_1_signupsignin1",
    "ApiScopes": "https://yourtenant.onmicrosoft.com/api/read",
    "APiEndpoint": "https://api.yourdomain.com/endpoint",
    "RedirectUri": "http://localhost"
  }
}
```

## 🔄 Update & Uninstall

Update to latest version:
```bash
dotnet tool update -g B2CConsoleClient
```

Uninstall:
```bash
dotnet tool uninstall -g B2CConsoleClient
```

## 🔐 Authentication Flow

1. **Silent Authentication**: Attempts to use cached tokens
2. **Interactive Authentication**: Opens browser for Azure B2C sign-in
3. **Token Caching**: Securely stores tokens for future use
4. **API Calls**: Uses access token to call configured endpoints

## 🌍 Cross-Platform Support

- **Windows**: DPAPI encryption for secure token storage
- **macOS/Linux**: File permission-based security
- **All Platforms**: Interactive browser authentication

## 📚 Examples

### Basic Usage
```bash
# First run - interactive setup
b2c-client

# Subsequent runs use saved configuration
b2c-client

# Override tenant for specific call
b2c-client -tenantname different-tenant
```

### API Testing
```bash
# Test with specific endpoint
b2c-client -apiendpoints https://api.example.com/test

# Include subscription key
b2c-client -apisubscriptionkey your-key-123
```

## 🛠️ Development

### Requirements
- .NET 9.0 SDK
- Azure B2C tenant configured for interactive authentication

### Building from Source

```bash
git clone https://github.com/sujitks/b2c-console-client
cd b2c-console-client
dotnet build
dotnet run
```

### Creating NuGet Package

```bash
dotnet pack --configuration Release
```

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License.

## 🆘 Support

For issues and questions, please use the project's issue tracker.

## 📦 Dependencies

- Microsoft.Identity.Client 4.67.1
- Microsoft.Extensions.Configuration 9.0.0
- System.Text.Json 9.0.0
- System.Security.Cryptography.ProtectedData 9.0.0
