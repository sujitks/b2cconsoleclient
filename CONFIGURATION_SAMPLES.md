# Sample configuration for different environments

## Development Environment
```
TenantName: mycompany-dev
ClientId: 12345678-1234-1234-1234-123456789012
PolicySignUpSignIn: b2c_1_signupsignin_dev
PolicyResetPassword: b2c_1_passwordreset_dev
ApiScopes: https://mycompany-dev.onmicrosoft.com/api/access_as_user
ApiEndpoint: https://dev-api.mycompany.com/api/test
ApiSubscriptionKey: dev-subscription-key-here
```

## Staging Environment
```
TenantName: mycompany-staging
ClientId: 87654321-4321-4321-4321-210987654321
PolicySignUpSignIn: b2c_1_signupsignin_staging
PolicyResetPassword: b2c_1_passwordreset_staging
ApiScopes: https://mycompany-staging.onmicrosoft.com/api/access_as_user
ApiEndpoint: https://staging-api.mycompany.com/api/test
ApiSubscriptionKey: staging-subscription-key-here
```

## Production Environment
```
TenantName: mycompany
ClientId: abcdef12-3456-7890-abcd-ef1234567890
PolicySignUpSignIn: b2c_1_signupsignin
PolicyResetPassword: b2c_1_passwordreset
ApiScopes: https://mycompany.onmicrosoft.com/api/access_as_user
ApiEndpoint: https://api.mycompany.com/api/test
ApiSubscriptionKey: production-subscription-key-here
```

## Usage Examples

### Development Testing
```bash
B2CConsoleClient.exe -userid testuser@mycompany.com -password TestPassword123!
```

### Staging with Custom Endpoint
```bash
B2CConsoleClient.exe -userid staginguser@mycompany.com -password StagingPass456! -apiendpoints https://staging-api.mycompany.com/api/custom-test
```

### Production with Full Override
```bash
B2CConsoleClient.exe -userid produser@mycompany.com -password ProductionPass789! -tenantname mycompany -clientid abcdef12-3456-7890-abcd-ef1234567890 -apiscopes https://mycompany.onmicrosoft.com/api/access_as_user
```
