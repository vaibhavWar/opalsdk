# Azure Deployment Script for Product Description Generator
# This script automates the deployment of the application to Azure App Service

param(
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroup = "opal-tools-rg",
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "eastus",
    
    [Parameter(Mandatory=$false)]
    [string]$AppServicePlan = "opal-tools-plan",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("F1", "B1", "B2", "S1", "P1V2")]
    [string]$Sku = "B1"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Product Description Generator - Azure Deployment" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if Azure CLI is installed
Write-Host "Checking Azure CLI installation..." -ForegroundColor Yellow
try {
    $azVersion = az version --output json | ConvertFrom-Json
    Write-Host "✓ Azure CLI version $($azVersion.'azure-cli') is installed" -ForegroundColor Green
} catch {
    Write-Host "✗ Azure CLI is not installed. Please install from: https://docs.microsoft.com/cli/azure/install-azure-cli" -ForegroundColor Red
    exit 1
}

# Login to Azure
Write-Host ""
Write-Host "Logging in to Azure..." -ForegroundColor Yellow
az account show --output none 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Please log in to Azure..." -ForegroundColor Yellow
    az login
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ Failed to log in to Azure" -ForegroundColor Red
        exit 1
    }
}
Write-Host "✓ Logged in to Azure" -ForegroundColor Green

# Get current subscription
$subscription = az account show --query name -o tsv
Write-Host "✓ Using subscription: $subscription" -ForegroundColor Green

# Generate unique names
$random = Get-Random -Minimum 1000 -Maximum 9999
$webAppName = "product-desc-gen-$random"

Write-Host ""
Write-Host "Deployment Configuration:" -ForegroundColor Cyan
Write-Host "  Resource Group: $ResourceGroup" -ForegroundColor White
Write-Host "  Location: $Location" -ForegroundColor White
Write-Host "  App Service Plan: $AppServicePlan" -ForegroundColor White
Write-Host "  SKU: $Sku" -ForegroundColor White
Write-Host "  Web App Name: $webAppName" -ForegroundColor White
Write-Host ""

# Confirm deployment
$confirm = Read-Host "Do you want to proceed with deployment? (y/n)"
if ($confirm -ne "y" -and $confirm -ne "Y") {
    Write-Host "Deployment cancelled" -ForegroundColor Yellow
    exit 0
}

# Create resource group
Write-Host ""
Write-Host "Creating resource group..." -ForegroundColor Yellow
az group create --name $ResourceGroup --location $Location --output none
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Resource group created: $ResourceGroup" -ForegroundColor Green
} else {
    Write-Host "✓ Using existing resource group: $ResourceGroup" -ForegroundColor Green
}

# Create App Service Plan
Write-Host ""
Write-Host "Creating App Service Plan..." -ForegroundColor Yellow
az appservice plan create `
    --name $AppServicePlan `
    --resource-group $ResourceGroup `
    --is-linux `
    --sku $Sku `
    --output none 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ App Service Plan created: $AppServicePlan" -ForegroundColor Green
} else {
    Write-Host "✓ Using existing App Service Plan: $AppServicePlan" -ForegroundColor Green
}

# Create Web App
Write-Host ""
Write-Host "Creating Web App..." -ForegroundColor Yellow
az webapp create `
    --resource-group $ResourceGroup `
    --plan $AppServicePlan `
    --name $webAppName `
    --runtime "DOTNET:8.0" `
    --output none

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Web App created: $webAppName" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to create Web App" -ForegroundColor Red
    exit 1
}

# Deploy the application
Write-Host ""
Write-Host "Deploying application from local directory..." -ForegroundColor Yellow
Write-Host "This may take several minutes..." -ForegroundColor Yellow

Push-Location csharp
az webapp up `
    --name $webAppName `
    --resource-group $ResourceGroup `
    --plan $AppServicePlan `
    --os-type Linux `
    --runtime "DOTNETCORE:8.0" `
    --output none

$deployExitCode = $LASTEXITCODE
Pop-Location

if ($deployExitCode -eq 0) {
    Write-Host "✓ Application deployed successfully!" -ForegroundColor Green
} else {
    Write-Host "✗ Deployment failed" -ForegroundColor Red
    exit 1
}

# Configure app settings
Write-Host ""
Write-Host "Configuring application settings..." -ForegroundColor Yellow
az webapp config appsettings set `
    --resource-group $ResourceGroup `
    --name $webAppName `
    --settings ASPNETCORE_ENVIRONMENT=Production WEBSITES_PORT=8080 `
    --output none

Write-Host "✓ Application configured" -ForegroundColor Green

# Enable logging
Write-Host ""
Write-Host "Enabling container logging..." -ForegroundColor Yellow
az webapp log config `
    --name $webAppName `
    --resource-group $ResourceGroup `
    --docker-container-logging filesystem `
    --output none

Write-Host "✓ Logging enabled" -ForegroundColor Green

# Get the app URL
$appUrl = "https://$webAppName.azurewebsites.net"

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Deployment Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Application URL: $appUrl" -ForegroundColor Cyan
Write-Host "Discovery Endpoint: $appUrl/discovery" -ForegroundColor Cyan
Write-Host "Tool Endpoint: $appUrl/tools/product-description-generator" -ForegroundColor Cyan
Write-Host ""
Write-Host "Testing endpoints..." -ForegroundColor Yellow

# Wait a bit for the app to start
Start-Sleep -Seconds 10

# Test discovery endpoint
Write-Host ""
Write-Host "Testing discovery endpoint..." -ForegroundColor Yellow
try {
    $discoveryResponse = Invoke-RestMethod -Uri "$appUrl/discovery" -Method GET -ErrorAction Stop
    Write-Host "✓ Discovery endpoint is working!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Available tools:" -ForegroundColor Cyan
    foreach ($tool in $discoveryResponse.tools) {
        Write-Host "  - $($tool.name): $($tool.description)" -ForegroundColor White
    }
} catch {
    Write-Host "✗ Discovery endpoint is not responding yet (this is normal, give it a minute)" -ForegroundColor Yellow
    Write-Host "  You can manually test: $appUrl/discovery" -ForegroundColor White
}

Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. Configure in Optimizely Opal with discovery URL: $appUrl/discovery" -ForegroundColor White
Write-Host "2. View logs: az webapp log tail --name $webAppName --resource-group $ResourceGroup" -ForegroundColor White
Write-Host "3. Manage app: https://portal.azure.com" -ForegroundColor White
Write-Host ""
Write-Host "To delete resources later: az group delete --name $ResourceGroup --yes" -ForegroundColor White
Write-Host ""

