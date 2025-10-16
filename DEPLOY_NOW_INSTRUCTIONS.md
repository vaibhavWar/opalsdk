# 🚀 Deploy to Azure NOW - Fixed Scripts

## ✅ What's Fixed

I've created **two deployment scripts** to fix the runtime error:

1. **`deploy-azure-fixed.ps1`** - For Linux App Service (recommended)
2. **`deploy-azure-windows.ps1`** - For Windows App Service

## 🔧 The Problem

The original script was mixing Linux and Windows runtime formats. This is now fixed!

---

## 📋 **OPTION 1: Deploy with Azure CLI (After Terminal Restart)**

### Step 1: Close and Reopen PowerShell

Azure CLI was just installed and needs a fresh terminal session.

### Step 2: Run Deployment

**For Linux (Recommended):**
```powershell
cd C:\Projects\Opal\productdescriptiongenerator
.\deploy-azure-fixed.ps1
```

**For Windows (Since you mentioned you have Windows .NET 8):**
```powershell
cd C:\Projects\Opal\productdescriptiongenerator
.\deploy-azure-windows.ps1
```

The script will:
- ✅ Create Azure resources
- ✅ Build your application
- ✅ Deploy to Azure
- ✅ Configure settings
- ✅ Test the deployment
- ✅ Give you the discovery URL

---

## 📋 **OPTION 2: Deploy via Azure Portal (No CLI Needed)**

If you don't want to restart the terminal, use the Azure Portal method:

### Step 1: Create Web App in Azure Portal

1. Go to: https://portal.azure.com
2. Login with: **prftindia2@gmail.com**
3. Click **"+ Create a resource"** → Search **"Web App"**
4. Fill in:
   - **Resource Group**: Create new → `opal-tools-rg`
   - **Name**: `product-desc-gen-yourname` (globally unique)
   - **Publish**: `Code`
   - **Runtime stack**: `.NET 8 (LTS)`
   - **Operating System**: Choose:
     - **Linux** (recommended for .NET 8)
     - **Windows** (if you prefer Windows)
   - **Region**: `East US`
   - **App Service Plan**: Create new, B1 tier
5. Click **"Review + create"** → **"Create"**

### Step 2: Deploy Your Code

**Method A: GitHub Actions (Easiest)**

1. In your Web App, go to **"Deployment Center"**
2. **Source**: `GitHub` → Authorize
3. **Repository**: `opalsdk`
4. **Branch**: `main`
5. **Build Provider**: `GitHub Actions`
6. **Runtime**: `.NET 8`
7. **Working Directory**: `/csharp`
8. Click **"Save"**

**Method B: ZIP Deploy**

1. Build locally:
   ```powershell
   cd csharp
   dotnet publish -c Release -o ./publish
   Compress-Archive -Path ./publish/* -DestinationPath ../deploy.zip -Force
   ```

2. In Azure Portal:
   - Go to your Web App → **"Advanced Tools"** → **"Go"**
   - **Tools** → **"ZIP Push Deploy"**
   - Drag and drop `deploy.zip`

### Step 3: Configure Settings

In Azure Portal, go to your Web App → **"Configuration"**:

Add these application settings:
- **Linux**: 
  - `ASPNETCORE_ENVIRONMENT` = `Production`
  - `WEBSITES_PORT` = `8080`
- **Windows**:
  - `ASPNETCORE_ENVIRONMENT` = `Production`

Click **"Save"** → **"Continue"**

### Step 4: Test

Your URL: `https://your-app-name.azurewebsites.net/discovery`

---

## 🎯 Recommended Approach

### **Easiest**: GitHub Actions (Option 2, Method A)
- No local tools needed
- Auto-deploys on git push
- 5 minutes setup

### **Fastest**: CLI Script (Option 1)
- Automated deployment
- Just restart terminal and run script
- 2 minutes after restart

---

## 📝 What the Fixed Scripts Do

### Linux Script (`deploy-azure-fixed.ps1`):
- Creates Linux App Service Plan
- Uses `DOTNET:8.0` runtime (with fallback to `DOTNETCORE:8.0`)
- Deploys via ZIP
- Configures CORS and port 8080
- Tests deployment

### Windows Script (`deploy-azure-windows.ps1`):
- Creates Windows App Service Plan
- No runtime flag needed (Windows detects .NET 8 automatically)
- Deploys via ZIP
- Configures application logging
- Tests deployment

---

## 🆘 Troubleshooting

### "Azure CLI not found"
- Close and reopen PowerShell
- Or use Option 2 (Azure Portal)

### "Runtime not supported"
- Use `deploy-azure-fixed.ps1` (Linux) - this is fixed
- Or use `deploy-azure-windows.ps1` (Windows)

### Deployment takes too long
- First deployment: 3-5 minutes
- Check "Deployment Center" → "Logs" in Azure Portal

### Discovery returns 404
- Wait 1-2 minutes for cold start
- Check logs: Azure Portal → Log Stream
- Restart app if needed

---

## ✨ After Deployment

You'll get a URL like:
```
https://product-desc-gen-XXXX.azurewebsites.net
```

### Add to Optimizely Opal:
1. Discovery URL: `https://your-app.azurewebsites.net/discovery`
2. In Opal settings → Add custom tool
3. Paste discovery URL
4. Save and test!

---

**Your Azure Account**: prftindia2@gmail.com

**Next Step**: Choose Option 1 (restart terminal + run script) OR Option 2 (Azure Portal)

