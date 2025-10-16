# 🚀 Deploy to Azure - Start Here!

## 📌 Quick Summary

Azure CLI was installed but needs a terminal restart. Here are your **3 easiest options** to deploy right now:

---

## ⭐ **OPTION 1: GitHub Actions (RECOMMENDED - 5 Minutes)**

This is the **EASIEST** method - no local tools needed!

### Steps:

1. **Go to Azure Portal**: https://portal.azure.com
   - Login with: **prftindia2@gmail.com**

2. **Create Web App**:
   - Click **"+ Create a resource"**
   - Search **"Web App"** → **Create**
   - Fill in:
     - **Resource Group**: Create new → `opal-tools-rg`
     - **Name**: `product-desc-gen-yourname` (must be unique globally)
     - **Publish**: `Code`
     - **Runtime**: `.NET 8 (LTS)`
     - **OS**: `Linux`
     - **Region**: `East US`
     - **Pricing**: `Basic B1` (~$13/month)
   - Click **"Review + create"** → **"Create"**

3. **Enable GitHub Deployment** (in your new Web App):
   - Go to **"Deployment Center"** (left menu)
   - **Source**: `GitHub`
   - Click **"Authorize"** (sign in to GitHub)
   - **Repository**: `opalsdk`
   - **Branch**: `main`
   - **Build Provider**: `GitHub Actions`
   - **Runtime**: `.NET`
   - **Version**: `.NET 8`
   - **Working Directory**: `/csharp`
   - Click **"Save"**

4. **Configure Settings**:
   - Go to **"Configuration"** (left menu)
   - Click **"+ New application setting"**
   - Add setting: Name=`ASPNETCORE_ENVIRONMENT`, Value=`Production`
   - Add setting: Name=`WEBSITES_PORT`, Value=`8080`
   - Click **"Save"** → **"Continue"**

5. **Test** (wait 2-3 minutes for first deployment):
   - Your URL: `https://product-desc-gen-yourname.azurewebsites.net`
   - Test: `https://product-desc-gen-yourname.azurewebsites.net/discovery`

### ✅ Benefits:
- No CLI needed
- Auto-deploys on every git push
- Built-in CI/CD
- Easy to manage

---

## ⭐ **OPTION 2: VS Code Extension (3 Minutes)**

If you have Visual Studio Code:

1. **Install Extension**:
   - Open VS Code
   - Extensions (Ctrl+Shift+X)
   - Search: **"Azure App Service"**
   - Install it

2. **Deploy**:
   - Click Azure icon in sidebar
   - Sign in with: **prftindia2@gmail.com**
   - Right-click **"App Services"**
   - **"Create New Web App... (Advanced)"**
   - Follow prompts (same settings as Option 1)
   - Right-click `csharp` folder
   - **"Deploy to Web App..."**
   - Select your app

---

## ⭐ **OPTION 3: Azure CLI (After Restart)**

Close and reopen your terminal/PowerShell, then run:

```powershell
cd C:\Projects\Opal\productdescriptiongenerator
.\deploy-azure-simple.ps1
```

This will automatically deploy everything!

---

## 📋 After Deployment

Once deployed, you'll get a URL like:
```
https://product-desc-gen-yourname.azurewebsites.net
```

### Add to Optimizely Opal:

1. Copy your discovery URL:
   ```
   https://product-desc-gen-yourname.azurewebsites.net/discovery
   ```

2. In Optimizely Opal settings:
   - Add custom tool
   - Paste the discovery URL
   - Save

3. Test it in Opal:
   ```
   generate product description using below details
   Product name: DEWALT 20V Acrylic Dispenser
   Part#: 211DCE595D1
   Attributes: Brand: DEWALT, Voltage: 20V, Capacity: 28 oz.
   ```

---

## 🆘 Troubleshooting

### Can't find Web App creation?
- Make sure you're signed in to Azure Portal
- Try the search bar: "Web App"

### Deployment taking too long?
- First deployment takes 3-5 minutes
- Check deployment status in "Deployment Center" → "Logs"

### Discovery endpoint returns 404?
- Wait 1-2 minutes for app to fully start
- Check "Log stream" in Azure Portal
- Restart the app: Overview → Restart

### Need to view logs?
- Azure Portal → Your Web App → "Log stream"

---

## 💡 My Recommendation

**Use Option 1 (GitHub Actions)** because:
- ✅ No local installation needed
- ✅ You already have code on GitHub
- ✅ Automatic deployments
- ✅ Easiest to troubleshoot
- ✅ Professional CI/CD setup

Total time: **5 minutes**

---

## 📚 More Details

- **Full Guide**: See `DEPLOY_WITHOUT_CLI.md`
- **Azure Deployment**: See `AZURE_DEPLOYMENT_GUIDE.md`
- **Quick Deploy**: See `QUICK_DEPLOY.md`

---

**Your Azure Login**: prftindia2@gmail.com

**Your GitHub Repo**: https://github.com/vaibhavWar/opalsdk

🎯 **Start with Option 1 above!**

