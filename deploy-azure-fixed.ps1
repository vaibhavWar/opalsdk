# Fixed Azure Deployment Script for .NET 8
# Deploys to Linux App Service (recommended for .NET 8)

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
Write-Host "  App Service Plan: $AppServicePlan (Linux)"
Write-Host "  SKU: $Sku"
Write-Host "  Web App Name: $webAppName"
Write-Host ""

# Check Azure CLI
Write-Host "Step 1/6: Checking Azure CLI..." -ForegroundColor Yellow
try {
    $azVersion = az version 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Azure CLI not found"
    }
    Write-Host "OK: Azure CLI ready" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Azure CLI not found or not in PATH" -ForegroundColor Red
    Write-Host "Please close and reopen your terminal, then try again" -ForegroundColor Yellow
    exit 1
}

# Check login
Write-Host ""
Write-Host "Step 2/6: Checking Azure login..." -ForegroundColor Yellow
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

# Confirm
Write-Host ""
Write-Host "Ready to deploy. Continue? (y/n): " -NoNewline -ForegroundColor Yellow
$confirm = Read-Host
if ($confirm -ne "y" -and $confirm -ne "Y") {
    Write-Host "Cancelled" -ForegroundColor Yellow
    exit 0
}

# Create Resource Group
Write-Host ""
Write-Host "Step 3/6: Creating resource group..." -ForegroundColor Yellow
az group create --name $ResourceGroup --location $Location --output none 2>$null
Write-Host "OK: Resource group ready" -ForegroundColor Green

# Create App Service Plan (Linux)
Write-Host ""
Write-Host "Step 4/6: Creating Linux App Service Plan..." -ForegroundColor Yellow
az appservice plan create `
    --name $AppServicePlan `
    --resource-group $ResourceGroup `
    --is-linux `
    --sku $Sku `
    --output none 2>$null
Write-Host "OK: App Service Plan ready" -ForegroundColor Green

# Create Web App with .NET 8 runtime
Write-Host ""
Write-Host "Step 5/6: Creating Web App with .NET 8..." -ForegroundColor Yellow
az webapp create `
    --resource-group $ResourceGroup `
    --plan $AppServicePlan `
    --name $webAppName `
    --runtime "DOTNET:8.0" `
    --output none

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to create Web App" -ForegroundColor Red
    Write-Host ""
    Write-Host "Trying alternative runtime format..." -ForegroundColor Yellow
    az webapp create `
        --resource-group $ResourceGroup `
        --plan $AppServicePlan `
        --name $webAppName `
        --runtime "DOTNETCORE:8.0" `
        --output none
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to create Web App with both runtime formats" -ForegroundColor Red
        Write-Host ""
        Write-Host "Available Linux runtimes:" -ForegroundColor Yellow
        az webapp list-runtimes --os-type linux --output table
        exit 1
    }
}
Write-Host "OK: Web App created" -ForegroundColor Green

# Configure app settings BEFORE deployment
Write-Host ""
Write-Host "Configuring application settings..." -ForegroundColor Yellow
az webapp config appsettings set `
    --resource-group $ResourceGroup `
    --name $webAppName `
    --settings ASPNETCORE_ENVIRONMENT=Production WEBSITES_PORT=8080 `
    --output none

# Deploy Application
Write-Host ""
Write-Host "Step 6/6: Deploying application (2-3 minutes)..." -ForegroundColor Yellow

# Build and publish
Push-Location csharp
Write-Host "  Building application..." -ForegroundColor Gray
dotnet publish -c Release -o ./publish 2>&1 | Out-Null

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    Pop-Location
    exit 1
}

# Create ZIP
Write-Host "  Creating deployment package..." -ForegroundColor Gray
$publishPath = (Get-Item "./publish").FullName
$zipPath = Join-Path $publishPath ".." "deploy.zip"
Compress-Archive -Path "$publishPath\*" -DestinationPath $zipPath -Force

# Deploy ZIP
Write-Host "  Uploading to Azure..." -ForegroundColor Gray
az webapp deployment source config-zip `
    --resource-group $ResourceGroup `
    --name $webAppName `
    --src $zipPath `
    --output none

$deployResult = $LASTEXITCODE
Pop-Location

if ($deployResult -ne 0) {
    Write-Host "ERROR: Deployment failed" -ForegroundColor Red
    exit 1
}

Write-Host "OK: Application deployed" -ForegroundColor Green

# Enable logging
Write-Host ""
Write-Host "Enabling logging..." -ForegroundColor Yellow
az webapp log config `
    --name $webAppName `
    --resource-group $ResourceGroup `
    --docker-container-logging filesystem `
    --output none

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

# Wait and test
Write-Host "Waiting for app to start (15 seconds)..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

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
    Write-Host "WARN: Endpoint not ready yet. Wait 1-2 minutes and test:" -ForegroundColor Yellow
    Write-Host "  $appUrl/discovery" -ForegroundColor White
}

Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Add to Optimizely Opal with discovery URL above"
Write-Host "  2. View logs: az webapp log tail --name $webAppName --resource-group $ResourceGroup"
Write-Host "  3. Manage: https://portal.azure.com"
Write-Host ""
Write-Host "To delete resources: az group delete --name $ResourceGroup --yes" -ForegroundColor Gray
Write-Host ""

# Save deployment info
$deployInfo = @"
========================================
DEPLOYMENT INFORMATION
========================================
Deployed: $(Get-Date)
Resource Group: $ResourceGroup
Web App Name: $webAppName
App URL: $appUrl
Discovery URL: $appUrl/discovery
Tool Endpoint: $appUrl/tools/product-description-generator

To view logs:
  az webapp log tail --name $webAppName --resource-group $ResourceGroup

To restart:
  az webapp restart --name $webAppName --resource-group $ResourceGroup

To delete:
  az group delete --name $ResourceGroup --yes
========================================
"@

$deployInfo | Out-File -FilePath "DEPLOYMENT_INFO.txt" -Encoding UTF8
Write-Host "Deployment info saved to: DEPLOYMENT_INFO.txt" -ForegroundColor Gray

