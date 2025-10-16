# Azure Deployment Guide

This guide walks you through deploying the Product Description Generator to Azure App Service.

## Prerequisites

- Azure account ([Sign up for free](https://azure.microsoft.com/free/))
- Azure CLI installed ([Download](https://docs.microsoft.com/cli/azure/install-azure-cli))
- Git installed
- Access to your GitHub repository

## Deployment Options

### Option 1: Automated GitHub Actions Deployment (Recommended)

This option automatically deploys your application whenever you push to the `main` branch.

#### Step 1: Create Azure Resources

```powershell
# Login to Azure
az login

# Set variables (customize these)
$RESOURCE_GROUP = "opal-tools-rg"
$LOCATION = "eastus"
$APP_SERVICE_PLAN = "opal-tools-plan"
$WEBAPP_NAME = "product-description-generator-$(Get-Random -Minimum 1000 -Maximum 9999)"
$ACR_NAME = "opaltoolsacr$(Get-Random -Minimum 1000 -Maximum 9999)"

# Create resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create Azure Container Registry (ACR)
az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic --admin-enabled true

# Get ACR credentials
$ACR_LOGIN_SERVER = az acr show --name $ACR_NAME --query loginServer --output tsv
$ACR_USERNAME = az acr credential show --name $ACR_NAME --query username --output tsv
$ACR_PASSWORD = az acr credential show --name $ACR_NAME --query "passwords[0].value" --output tsv

# Create App Service Plan (Linux)
az appservice plan create --name $APP_SERVICE_PLAN --resource-group $RESOURCE_GROUP --is-linux --sku B1

# Create Web App with container
az webapp create --resource-group $RESOURCE_GROUP --plan $APP_SERVICE_PLAN --name $WEBAPP_NAME --deployment-container-image-name $ACR_LOGIN_SERVER/product-description-generator:latest

# Configure Web App
az webapp config appsettings set --resource-group $RESOURCE_GROUP --name $WEBAPP_NAME --settings WEBSITES_PORT=8080

# Enable container logging
az webapp log config --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP --docker-container-logging filesystem

# Get publish profile
az webapp deployment list-publishing-profiles --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP --xml
```

#### Step 2: Configure GitHub Secrets

Go to your GitHub repository → Settings → Secrets and variables → Actions, and add:

1. **AZURE_REGISTRY_LOGIN_SERVER**: `$ACR_LOGIN_SERVER` (from above)
2. **AZURE_REGISTRY_USERNAME**: `$ACR_USERNAME` (from above)
3. **AZURE_REGISTRY_PASSWORD**: `$ACR_PASSWORD` (from above)
4. **AZURE_WEBAPP_PUBLISH_PROFILE**: Copy the entire XML output from the last command above

#### Step 3: Update Workflow File

Edit `.github/workflows/azure-deploy.yml` and update the `AZURE_WEBAPP_NAME` to match your `$WEBAPP_NAME`.

#### Step 4: Deploy

```powershell
git add .
git commit -m "Configure Azure deployment"
git push origin main
```

The GitHub Action will automatically build and deploy your application!

#### Step 5: Verify Deployment

```powershell
# Get your app URL
$APP_URL = "https://$WEBAPP_NAME.azurewebsites.net"
Write-Host "App URL: $APP_URL"

# Test the discovery endpoint
Invoke-RestMethod -Uri "$APP_URL/discovery" -Method GET

# View logs
az webapp log tail --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP
```

---

### Option 2: Manual Azure CLI Deployment

For quick one-time deployment without setting up CI/CD:

```powershell
# Login to Azure
az login

# Set variables
$RESOURCE_GROUP = "opal-tools-rg"
$LOCATION = "eastus"
$APP_SERVICE_PLAN = "opal-tools-plan"
$WEBAPP_NAME = "product-description-generator-$(Get-Random -Minimum 1000 -Maximum 9999)"

# Create resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create App Service Plan
az appservice plan create --name $APP_SERVICE_PLAN --resource-group $RESOURCE_GROUP --is-linux --sku B1

# Deploy from local Docker build
cd csharp
az webapp up --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP --plan $APP_SERVICE_PLAN --os-type Linux --runtime "DOTNETCORE:8.0"

# Configure the app
az webapp config appsettings set --resource-group $RESOURCE_GROUP --name $WEBAPP_NAME --settings ASPNETCORE_URLS=http://+:8080

# View the URL
Write-Host "App deployed to: https://$WEBAPP_NAME.azurewebsites.net"
```

---

### Option 3: Docker Container Deployment

Deploy using Docker directly:

```powershell
# Build and push to Azure Container Registry
cd csharp

# Build Docker image
docker build -t product-description-generator:latest .

# Tag for ACR (use your ACR name from Option 1)
docker tag product-description-generator:latest $ACR_LOGIN_SERVER/product-description-generator:latest

# Login to ACR
az acr login --name $ACR_NAME

# Push image
docker push $ACR_LOGIN_SERVER/product-description-generator:latest

# Deploy to Web App
az webapp config container set --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP --docker-custom-image-name $ACR_LOGIN_SERVER/product-description-generator:latest
```

---

## Testing Your Deployment

Once deployed, test the endpoints:

```powershell
$APP_URL = "https://YOUR_WEBAPP_NAME.azurewebsites.net"

# Test discovery endpoint
Invoke-RestMethod -Uri "$APP_URL/discovery" -Method GET | ConvertTo-Json -Depth 10

# Test execution endpoint
$body = @{
    productName = "DEWALT 20V Acrylic Dispenser"
    partNumber = "211DCE595D1"
    attributes = @(
        "Brand: DEWALT",
        "Battery Voltage (V): 20",
        "Capacity: 28 oz."
    )
    type = "ecommerce"
    tone = "professional"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$APP_URL/tools/product-description-generator" -Method POST -Body $body -ContentType "application/json" | ConvertTo-Json -Depth 10
```

---

## Monitoring and Troubleshooting

### View Application Logs

```powershell
# Stream live logs
az webapp log tail --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP

# Download logs
az webapp log download --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP --log-file logs.zip
```

### Check Application Health

```powershell
# Get app status
az webapp show --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP --query state

# Restart the app
az webapp restart --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP
```

### Common Issues

1. **Container fails to start**
   - Check that `WEBSITES_PORT=8080` is set in app settings
   - Verify Dockerfile exposes port 8080
   - Check logs: `az webapp log tail --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP`

2. **403 Forbidden or CORS errors**
   - The app has CORS configured to allow all origins
   - If issues persist, add specific CORS configuration in Azure Portal

3. **Slow first request (cold start)**
   - Azure App Service has a warm-up period
   - Consider upgrading to a higher-tier plan for always-on functionality

---

## Configuring for Optimizely Opal

Once deployed, configure your Azure Web App URL in Optimizely Opal:

1. Copy your app URL: `https://YOUR_WEBAPP_NAME.azurewebsites.net`
2. In Optimizely Opal settings, add custom tool:
   - **Discovery URL**: `https://YOUR_WEBAPP_NAME.azurewebsites.net/discovery`
3. Save and test the integration

---

## Cost Optimization

- **B1 Plan**: ~$13/month - Good for development/testing
- **F1 Free Plan**: Available for testing (limited resources)
- **P1V2 Plan**: ~$73/month - Production with auto-scaling

To use the Free tier:

```powershell
az appservice plan create --name $APP_SERVICE_PLAN --resource-group $RESOURCE_GROUP --is-linux --sku F1
```

---

## Updating Your Deployment

### With GitHub Actions
Just push changes to the `main` branch:

```powershell
git add .
git commit -m "Your changes"
git push origin main
```

### Manual Update
```powershell
cd csharp
az webapp up --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP
```

---

## Clean Up Resources

When you're done, delete all Azure resources:

```powershell
az group delete --name $RESOURCE_GROUP --yes --no-wait
```

---

## Next Steps

- Configure custom domain: [Azure Docs](https://docs.microsoft.com/azure/app-service/app-service-web-tutorial-custom-domain)
- Enable HTTPS: [Azure Docs](https://docs.microsoft.com/azure/app-service/configure-ssl-certificate)
- Set up monitoring: [Application Insights](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- Configure auto-scaling: [Azure Docs](https://docs.microsoft.com/azure/app-service/manage-scale-up)

