# Deploy to Azure - Step by Step Guide

## ✅ Azure CLI is Already Installed!

Your system is ready. Azure CLI version 2.78.0 is installed and working.

## 🚀 Deploy Your Application (3 Simple Steps)

### Step 1: Login to Azure

Open PowerShell in this project directory and run:

```powershell
az login
```

**What happens:**
- A browser window will open
- Login with: **prftindia2@gmail.com**
- After successful login, close the browser and return to PowerShell

### Step 2: Run the Deployment Script

```powershell
.\deploy-azure-simple.ps1
```

**What happens:**
- Script will ask "Continue? (y/n)" - Type **y** and press Enter
- It will create all Azure resources
- Deploy your application (takes 2-3 minutes)
- Test the deployment automatically

### Step 3: Get Your Discovery URL

After successful deployment, you'll see:

```
DEPLOYMENT SUCCESSFUL!
Application URL: https://product-desc-gen-XXXX.azurewebsites.net
Discovery URL: https://product-desc-gen-XXXX.azurewebsites.net/discovery
```

Copy the **Discovery URL** and add it to Optimizely Opal!

---

## 🎯 Quick Command (All in One)

If you prefer a single command sequence:

```powershell
# Login to Azure
az login

# Wait for browser login to complete, then run deployment
.\deploy-azure-simple.ps1
```

---

## 🔧 Alternative: Manual Azure Portal Deployment

If you prefer using the Azure Portal instead:

### 1. Go to Azure Portal
Visit: https://portal.azure.com

### 2. Create Web App
- Click "Create a resource"
- Search for "Web App"
- Click "Create"

### 3. Fill in Details
- **Resource Group**: Create new "opal-tools-rg"
- **Name**: `product-desc-gen-[any-number]` (must be unique)
- **Publish**: Code
- **Runtime stack**: .NET 8 (LTS)
- **Operating System**: Linux
- **Region**: East US (or your preferred region)
- **Pricing Plan**: Basic B1 (~$13/month)

### 4. Create and Wait
- Click "Review + Create"
- Click "Create"
- Wait 1-2 minutes for deployment

### 5. Deploy Code via GitHub

**Option A: GitHub Actions (Recommended)**

In Azure Portal → Your Web App → Deployment Center:
1. Select "GitHub"
2. Authorize GitHub
3. Select Repository: `vaibhavWar/opalsdk`
4. Select Branch: `main`
5. Click "Save"

Azure will automatically build and deploy! ✨

**Option B: Local ZIP Deploy**

```powershell
# Build the app
cd csharp
dotnet publish -c Release -o ./publish

# Create ZIP
Compress-Archive -Path ./publish/* -DestinationPath ../deploy.zip -Force
cd ..

# Deploy (replace YOUR-APP-NAME)
az webapp deployment source config-zip --resource-group opal-tools-rg --name YOUR-APP-NAME --src deploy.zip
```

### 6. Configure Settings

In Azure Portal → Your Web App → Configuration → Application settings:

Add these:
- `ASPNETCORE_ENVIRONMENT` = `Production`
- `WEBSITES_PORT` = `8080`

Click **Save**, then **Continue**

### 7. Test Your App

Get your app URL from the Overview page:
`https://YOUR-APP-NAME.azurewebsites.net`

Test discovery:
```powershell
Invoke-RestMethod -Uri "https://YOUR-APP-NAME.azurewebsites.net/discovery"
```

---

## 📊 Deployment Status

✅ Azure CLI Installed (v2.78.0)
✅ Application Code Ready
✅ Deployment Scripts Ready
⏳ Waiting for: Azure Login

---

## 💡 Tips

1. **First deployment takes 2-3 minutes** - be patient!
2. **App URL will be unique** - Azure generates a random name
3. **Free tier available** - Change `-Sku B1` to `-Sku F1` in script for free tier
4. **View logs anytime**:
   ```powershell
   az webapp log tail --name YOUR-APP-NAME --resource-group opal-tools-rg
   ```

---

## 🆘 Need Help?

### "Login keeps failing"
- Make sure you're using: **prftindia2@gmail.com**
- Try: `az logout` then `az login` again

### "App not responding after deployment"
- Wait 2 minutes for cold start
- Check logs: `az webapp log tail --name YOUR-APP-NAME --resource-group opal-tools-rg`

### "Deployment script fails"
- Make sure you're in the project root directory
- Check you have Azure permissions to create resources

---

## 🎉 After Successful Deployment

1. Copy your Discovery URL
2. Go to Optimizely Opal Settings
3. Add Custom Tool → Paste Discovery URL
4. Test it with a product description request!

---

**Ready to deploy? Run these two commands:**

```powershell
az login
.\deploy-azure-simple.ps1
```

Good luck! 🚀

