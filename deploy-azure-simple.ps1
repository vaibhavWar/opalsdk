# Simple Azure Deployment Script
# Deploys Product Description Generator to Azure App Service

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Product Description Generator - Azure Deployment" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$ResourceGroup = "opal-tools-rg"
$Location = "eastus"
$AppServicePlan = "opal-tools-plan"
$Sku = "B1"
$random = Get-Random -Minimum 1000 -Maximum 9999
$webAppName = "product-desc-gen-$random"

Write-Host "Configuration:" -ForegroundColor Cyan
Write-Host "  Resource Group: $ResourceGroup"
Write-Host "  Location: $Location"
Write-Host "  App Service Plan: $AppServicePlan"
Write-Host "  SKU: $Sku"
Write-Host "  Web App Name: $webAppName"
Write-Host ""

# Check Azure CLI
Write-Host "Checking Azure CLI..." -ForegroundColor Yellow
$azCheck = Get-Command az -ErrorAction SilentlyContinue
if (-not $azCheck) {
    Write-Host "ERROR: Azure CLI not found. Install from: https://aka.ms/installazurecliwindows" -ForegroundColor Red
    exit 1
}
Write-Host "OK: Azure CLI found" -ForegroundColor Green
Write-Host ""

# Check login status
Write-Host "Checking Azure login..." -ForegroundColor Yellow
az account show --output none 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Logging in to Azure..." -ForegroundColor Yellow
    az login
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to login" -ForegroundColor Red
        exit 1
    }
}

$subscription = az account show --query name -o tsv
Write-Host "OK: Using subscription: $subscription" -ForegroundColor Green
Write-Host ""

# Confirm
Write-Host "Ready to deploy. This will:" -ForegroundColor Yellow
Write-Host "  1. Create Azure resources"
Write-Host "  2. Deploy your application"
Write-Host "  3. Configure settings"
Write-Host ""
$confirm = Read-Host "Continue? (y/n)"
if ($confirm -ne "y" -and $confirm -ne "Y") {
    Write-Host "Cancelled" -ForegroundColor Yellow
    exit 0
}
Write-Host ""

# Create Resource Group
Write-Host "Step 1/5: Creating resource group..." -ForegroundColor Yellow
az group create --name $ResourceGroup --location $Location --output none 2>$null
Write-Host "OK: Resource group ready" -ForegroundColor Green

# Create App Service Plan
Write-Host "Step 2/5: Creating App Service Plan..." -ForegroundColor Yellow
az appservice plan create --name $AppServicePlan --resource-group $ResourceGroup --is-linux --sku $Sku --output none 2>$null
Write-Host "OK: App Service Plan ready" -ForegroundColor Green

# Create Web App
Write-Host "Step 3/5: Creating Web App..." -ForegroundColor Yellow
az webapp create --resource-group $ResourceGroup --plan $AppServicePlan --name $webAppName --runtime "DOTNET:8.0" --output none
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to create Web App" -ForegroundColor Red
    exit 1
}
Write-Host "OK: Web App created" -ForegroundColor Green

# Deploy Application
Write-Host "Step 4/5: Deploying application (this may take 2-3 minutes)..." -ForegroundColor Yellow
Push-Location csharp
az webapp up --name $webAppName --resource-group $ResourceGroup --plan $AppServicePlan --os-type Linux --runtime "DOTNETCORE:8.0" --output none
$deployResult = $LASTEXITCODE
Pop-Location

if ($deployResult -ne 0) {
    Write-Host "ERROR: Deployment failed" -ForegroundColor Red
    exit 1
}
Write-Host "OK: Application deployed" -ForegroundColor Green

# Configure Settings
Write-Host "Step 5/5: Configuring application..." -ForegroundColor Yellow
az webapp config appsettings set --resource-group $ResourceGroup --name $webAppName --settings ASPNETCORE_ENVIRONMENT=Production WEBSITES_PORT=8080 --output none
az webapp log config --name $webAppName --resource-group $ResourceGroup --docker-container-logging filesystem --output none
Write-Host "OK: Configuration complete" -ForegroundColor Green

# Get URL
$appUrl = "https://$webAppName.azurewebsites.net"

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "DEPLOYMENT SUCCESSFUL!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Application URL:" -ForegroundColor Cyan
Write-Host "  $appUrl" -ForegroundColor White
Write-Host ""
Write-Host "Optimizely Opal Discovery URL:" -ForegroundColor Cyan
Write-Host "  $appUrl/discovery" -ForegroundColor White
Write-Host ""
Write-Host "Tool Endpoint:" -ForegroundColor Cyan
Write-Host "  $appUrl/tools/product-description-generator" -ForegroundColor White
Write-Host ""

# Wait for app to warm up
Write-Host "Waiting for application to start (10 seconds)..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Test Discovery
Write-Host "Testing discovery endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$appUrl/discovery" -Method GET -TimeoutSec 30 -ErrorAction Stop
    Write-Host "OK: Discovery endpoint working!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Available tools:" -ForegroundColor Cyan
    foreach ($tool in $response.tools) {
        Write-Host "  - $($tool.name)" -ForegroundColor White
    }
} catch {
    Write-Host "WARN: Endpoint not ready yet. Try in 1-2 minutes:" -ForegroundColor Yellow
    Write-Host "  $appUrl/discovery" -ForegroundColor White
}

Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Add to Optimizely Opal with discovery URL above"
Write-Host "  2. View logs: az webapp log tail --name $webAppName --resource-group $ResourceGroup"
Write-Host "  3. Manage in Azure Portal: https://portal.azure.com"
Write-Host ""
Write-Host "To delete resources: az group delete --name $ResourceGroup --yes" -ForegroundColor Gray
Write-Host ""

