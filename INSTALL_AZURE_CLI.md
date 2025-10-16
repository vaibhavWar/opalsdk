# Install Azure CLI on Windows

## Option 1: MSI Installer (Recommended)

1. Download the Azure CLI installer:
   https://aka.ms/installazurecliwindows

2. Run the downloaded MSI file

3. Follow the installation wizard

4. Restart your terminal/PowerShell

5. Verify installation:
   ```powershell
   az --version
   ```

## Option 2: Winget (Windows Package Manager)

If you have Winget installed:

```powershell
winget install -e --id Microsoft.AzureCLI
```

## Option 3: PowerShell (One Command)

```powershell
Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi
Start-Process msiexec.exe -ArgumentList '/I AzureCLI.msi /quiet' -Wait
Remove-Item .\AzureCLI.msi
```

Then restart your terminal.

## After Installation

1. Close and reopen your terminal
2. Verify Azure CLI is installed:
   ```powershell
   az --version
   ```
3. Run the deployment script:
   ```powershell
   .\deploy-azure-simple.ps1
   ```

## Alternative: Deploy via Azure Portal

If you prefer not to install Azure CLI, you can deploy manually:

### Step 1: Create App Service in Azure Portal

1. Go to https://portal.azure.com
2. Click "Create a resource"
3. Search for "Web App"
4. Fill in:
   - **Resource Group**: Create new "opal-tools-rg"
   - **Name**: product-desc-gen-[random]
   - **Publish**: Code
   - **Runtime stack**: .NET 8 (LTS)
   - **Operating System**: Linux
   - **Region**: East US
   - **Pricing Plan**: Basic B1
5. Click "Review + Create" → "Create"

### Step 2: Deploy Your Code

#### Option A: GitHub Integration (Easiest)

1. In your Web App, go to "Deployment Center"
2. Select "GitHub"
3. Authorize and select your repository
4. Select branch: main
5. Click "Save"

Azure will automatically build and deploy from GitHub!

#### Option B: ZIP Deploy

1. Build your application locally:
   ```powershell
   cd csharp
   dotnet publish -c Release -o ./publish
   Compress-Archive -Path ./publish/* -DestinationPath ../deploy.zip -Force
   cd ..
   ```

2. In Azure Portal, go to your Web App
3. Go to "Advanced Tools" → "Go"
4. In Kudu, go to "Tools" → "ZIP Push Deploy"
5. Drag and drop your `deploy.zip` file

#### Option C: VS Code Extension

1. Install "Azure App Service" extension in VS Code
2. Sign in to Azure
3. Right-click on `csharp` folder
4. Select "Deploy to Web App"
5. Select your Web App

### Step 3: Configure Settings

In Azure Portal, go to your Web App → Configuration → Application settings:

Add these settings:
- `ASPNETCORE_ENVIRONMENT` = `Production`
- `WEBSITES_PORT` = `8080`

Click "Save"

### Step 4: Test Your Deployment

1. Get your app URL from the Overview page
2. Test discovery: `https://your-app-name.azurewebsites.net/discovery`
3. Configure in Optimizely Opal with the discovery URL

## Need Help?

- Azure CLI Documentation: https://docs.microsoft.com/cli/azure/
- Azure App Service Documentation: https://docs.microsoft.com/azure/app-service/

