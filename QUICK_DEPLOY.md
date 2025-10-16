# Quick Deploy to Azure

The fastest way to deploy the Product Description Generator to Azure!

## Prerequisites

- Azure account ([Sign up for free](https://azure.microsoft.com/free/))
- Azure CLI installed ([Download](https://docs.microsoft.com/cli/azure/install-azure-cli))

## One-Command Deployment

Open PowerShell in the project root directory and run:

```powershell
.\deploy-azure.ps1
```

That's it! The script will:
1. ✓ Check Azure CLI installation
2. ✓ Log you into Azure (if needed)
3. ✓ Create all required Azure resources
4. ✓ Deploy your application
5. ✓ Configure settings
6. ✓ Test the deployment
7. ✓ Provide you with the discovery URL

## Custom Deployment Options

### Choose Different Pricing Tier

```powershell
# Free tier (limited resources, good for testing)
.\deploy-azure.ps1 -Sku F1

# Basic tier (recommended, ~$13/month)
.\deploy-azure.ps1 -Sku B1

# Production tier (~$73/month with auto-scaling)
.\deploy-azure.ps1 -Sku P1V2
```

### Choose Different Region

```powershell
.\deploy-azure.ps1 -Location westus2
```

### Specify Resource Group

```powershell
.\deploy-azure.ps1 -ResourceGroup my-custom-rg
```

### Combine Options

```powershell
.\deploy-azure.ps1 -ResourceGroup opal-prod -Location westus2 -Sku P1V2
```

## What You'll Get

After successful deployment:

- **Application URL**: `https://product-desc-gen-XXXX.azurewebsites.net`
- **Discovery Endpoint**: `https://product-desc-gen-XXXX.azurewebsites.net/discovery`
- **Tool Endpoint**: `https://product-desc-gen-XXXX.azurewebsites.net/tools/product-description-generator`

## Testing Your Deployment

```powershell
# Replace with your actual URL
$APP_URL = "https://product-desc-gen-XXXX.azurewebsites.net"

# Test discovery
Invoke-RestMethod -Uri "$APP_URL/discovery" -Method GET

# Test tool execution
$body = @{
    productName = "DEWALT 20V Acrylic Dispenser"
    partNumber = "211DCE595D1"
    attributes = @("Brand: DEWALT", "Voltage: 20V", "Capacity: 28 oz.")
    type = "ecommerce"
    tone = "professional"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$APP_URL/tools/product-description-generator" -Method POST -Body $body -ContentType "application/json"
```

## Configure in Optimizely Opal

1. Copy your discovery URL: `https://product-desc-gen-XXXX.azurewebsites.net/discovery`
2. In Optimizely Opal settings, add custom tool
3. Paste the discovery URL
4. Save and test!

## View Logs

```powershell
az webapp log tail --name product-desc-gen-XXXX --resource-group opal-tools-rg
```

## Update Your Application

After making code changes:

```powershell
.\deploy-azure.ps1
```

The script will redeploy to the existing resources.

## Clean Up

To delete all Azure resources:

```powershell
az group delete --name opal-tools-rg --yes
```

## Troubleshooting

### "Azure CLI not found"
Install from: https://docs.microsoft.com/cli/azure/install-azure-cli

### "Login failed"
Run: `az login` manually

### "App not responding"
- Wait 1-2 minutes for cold start
- Check logs: `az webapp log tail --name YOUR_APP_NAME --resource-group opal-tools-rg`

### "Deployment failed"
Check if you have sufficient permissions in your Azure subscription

## Need More Control?

For advanced deployment options, see [AZURE_DEPLOYMENT_GUIDE.md](AZURE_DEPLOYMENT_GUIDE.md)

