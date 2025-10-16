# Deploy to Azure Without CLI

Since Azure CLI needs a shell restart, here are easier deployment options:

## ✅ Option 1: GitHub Actions (Easiest - No CLI Needed!)

Your code is already on GitHub. Let's use GitHub Actions to deploy automatically:

### Step 1: Create Azure Resources in Portal

1. Go to https://portal.azure.com
2. Log in with: **prftindia2@gmail.com**
3. Click **"+ Create a resource"**
4. Search for **"Web App"** and click **Create**

Fill in the form:
- **Subscription**: Select your subscription
- **Resource Group**: Click "Create new" → Name: `opal-tools-rg`
- **Name**: `product-desc-gen-<YOUR_INITIALS>` (must be globally unique)
- **Publish**: `Code`
- **Runtime stack**: `.NET 8 (LTS)`
- **Operating System**: `Linux`
- **Region**: `East US`

Click **"Review + create"** → **"Create"** (wait 1-2 minutes)

### Step 2: Enable GitHub Deployment

1. Go to your new Web App in Azure Portal
2. In the left menu, find **"Deployment Center"**
3. Select **Source**: `GitHub`
4. Click **"Authorize"** and sign in to GitHub
5. Select:
   - **Organization**: Your GitHub username
   - **Repository**: `opalsdk`
   - **Branch**: `main`
6. **Build Provider**: Select `GitHub Actions`
7. **Runtime stack**: `.NET 8`
8. **Working Directory**: `/csharp`
9. Click **"Save"**

Azure will automatically:
- Create a GitHub Actions workflow
- Build your code
- Deploy to Azure
- Redeploy on every push to main

### Step 3: Configure App Settings

1. In your Web App, go to **"Configuration"**
2. Click **"+ New application setting"**
3. Add these settings:
   - Name: `ASPNETCORE_ENVIRONMENT`, Value: `Production`
   - Name: `WEBSITES_PORT`, Value: `8080`
4. Click **"Save"** → **"Continue"**

### Step 4: Test Your Deployment

1. Get your URL from the Web App **"Overview"** page
2. Test discovery: `https://your-app-name.azurewebsites.net/discovery`
3. Add to Optimizely Opal!

---

## ✅ Option 2: Visual Studio Code Extension

### Install VS Code Extension

1. Open VS Code
2. Go to Extensions (Ctrl+Shift+X)
3. Search for **"Azure App Service"**
4. Click **"Install"**

### Deploy

1. Click the Azure icon in the sidebar
2. Sign in with: **prftindia2@gmail.com**
3. Right-click on **"App Services"**
4. Select **"Create New Web App... (Advanced)"**
5. Follow the prompts:
   - Name: `product-desc-gen-<YOUR_INITIALS>`
   - Resource Group: Create new `opal-tools-rg`
   - Runtime: `.NET 8 (LTS)`
   - OS: `Linux`
   - Location: `East US`
   - App Service Plan: Create new, B1 tier
6. Right-click your `csharp` folder
7. Select **"Deploy to Web App..."**
8. Select your newly created app
9. Click **"Deploy"**

---

## ✅ Option 3: ZIP Deploy (Manual)

### Step 1: Build Deployment Package

Run this in your current terminal:

```powershell
cd csharp
dotnet publish -c Release -o ./publish
Compress-Archive -Path ./publish/* -DestinationPath ../deploy.zip -Force
cd ..
```

### Step 2: Deploy via Azure Portal

1. Go to https://portal.azure.com
2. Navigate to your Web App (create one if needed using Option 1, Step 1)
3. Go to **"Advanced Tools"** → Click **"Go"**
4. In Kudu console, go to **"Tools"** → **"ZIP Push Deploy"**
5. Drag and drop your `deploy.zip` file
6. Wait for deployment to complete

---

## ✅ Option 4: Direct Azure Portal Upload

### Using Azure Cloud Shell

1. Go to https://portal.azure.com
2. Click the Cloud Shell icon (>_) at the top
3. Select **"PowerShell"**
4. Run these commands:

```bash
# Clone your repo
git clone https://github.com/vaibhavWar/opalsdk.git
cd opalsdk/csharp

# Create resources
az group create --name opal-tools-rg --location eastus
az appservice plan create --name opal-tools-plan --resource-group opal-tools-rg --is-linux --sku B1
az webapp create --resource-group opal-tools-rg --plan opal-tools-plan --name product-desc-gen-$RANDOM --runtime "DOTNET:8.0"

# Deploy
az webapp up --name <YOUR-WEBAPP-NAME> --resource-group opal-tools-rg
```

---

## 🎯 Recommended: Option 1 (GitHub Actions)

This is the best option because:
- ✅ No CLI installation needed
- ✅ Automatic deployments on git push
- ✅ Built-in CI/CD pipeline
- ✅ Easy to manage in Azure Portal
- ✅ Free for public repositories

---

## After Deployment

Once deployed, you'll get a URL like:
`https://product-desc-gen-xxxx.azurewebsites.net`

### Configure in Optimizely Opal:
1. Go to Opal settings
2. Add custom tool
3. Discovery URL: `https://your-app-name.azurewebsites.net/discovery`
4. Save and test!

### View Logs:
- Azure Portal → Your Web App → **"Log stream"**
- Or **"Diagnose and solve problems"**

---

## Need Help?

All these methods work without Azure CLI. The GitHub Actions approach (Option 1) is the most reliable and requires no local tools!

