# Deploy to Cloudflare Workers - Quick Guide

Your code is **built and ready** to deploy! Choose the method that works best for you.

---

## ✅ **Option 1: Manual Upload (Easiest - 2 minutes)**

### Step 1: Go to Cloudflare Dashboard
Open in browser: **https://dash.cloudflare.com/**

### Step 2: Create Worker
1. Click **"Workers & Pages"** in left sidebar
2. Click **"Create application"**
3. Click **"Create Worker"**
4. Name it: `product-description-generator`
5. Click **"Deploy"**

### Step 3: Upload Your Code
1. Click **"Quick edit"** button
2. Delete all the default code in the editor
3. Open your local file: `dist/index.js`
4. Copy all contents (Ctrl+A, Ctrl+C)
5. Paste into Cloudflare editor (Ctrl+V)
6. Click **"Save and Deploy"**

### Step 4: Get Your URL
You'll see a URL like:
```
https://product-description-generator.your-subdomain.workers.dev
```

### Step 5: Test It
```
https://product-description-generator.your-subdomain.workers.dev/discovery
```

---

## ✅ **Option 2: API Token (For Automation)**

### Step 1: Create API Token
1. Go to: **https://dash.cloudflare.com/profile/api-tokens**
2. Click **"Create Token"**
3. Use template: **"Edit Cloudflare Workers"**
4. Click **"Continue to summary"**
5. Click **"Create Token"**
6. **Copy the token** (you won't see it again!)

### Step 2: Set Environment Variable
**PowerShell:**
```powershell
$env:CLOUDFLARE_API_TOKEN="your-token-here"
```

**Or set permanently:**
```powershell
[System.Environment]::SetEnvironmentVariable('CLOUDFLARE_API_TOKEN', 'your-token-here', 'User')
```

### Step 3: Deploy
```powershell
npm run deploy
```

---

## ✅ **Option 3: OAuth Login (Quick)**

### Step 1: Login
```powershell
npx wrangler login
```

- A browser window will open
- Click **"Allow"** to authorize Wrangler
- **Complete within 60 seconds**

### Step 2: Deploy
```powershell
npm run deploy
```

---

## 🎯 **Recommended: Use Option 1 (Manual Upload)**

This is the fastest and most reliable method for first-time deployment.

### Why Manual Upload?
- ✅ No authentication issues
- ✅ Works immediately
- ✅ Visual confirmation of deployment
- ✅ Can test in Cloudflare's editor
- ✅ Takes only 2 minutes

---

## 📋 **After Deployment**

### 1. Your Worker URL
You'll get a URL like:
```
https://product-description-generator.your-name.workers.dev
```

### 2. Test Endpoints

**Discovery:**
```
https://product-description-generator.your-name.workers.dev/discovery
```

**Health:**
```
https://product-description-generator.your-name.workers.dev/
```

**Execute (POST):**
```powershell
$body = @{
    productName = "Smart Watch"
    partNumber = "SW-2024"
    attributes = @("AMOLED Display", "GPS")
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://product-description-generator.your-name.workers.dev/" `
  -Method Post -Body $body -ContentType "application/json"
```

### 3. Register with Optimizely Opal

Once deployed and tested:

1. Go to: **https://opal.optimizely.com/tools**
2. Click **"Add Custom Tool"**
3. Enter your discovery URL:
   ```
   https://product-description-generator.your-name.workers.dev/discovery
   ```
4. Click **"Save"**

### 4. Test in Opal Chat
```
Generate a product description for Smart Watch (part SW-2024) with AMOLED display and GPS
```

---

## 🔧 **Troubleshooting**

### Issue: "Authentication Required"
**Solution:** Use Option 1 (Manual Upload) - no auth needed!

### Issue: "Worker name already taken"
**Solution:** Choose a different name like:
- `product-desc-gen-yourname`
- `opal-product-tool`
- `description-generator-v1`

### Issue: "Deployment fails"
**Solution:**
1. Make sure code is built: `npm run build`
2. Check `dist/index.js` exists
3. Try manual upload method

---

## 📁 **What You're Deploying**

**File:** `dist/index.js` (12 KB)

**Contains:**
- ✅ Optimizely Opal SDK implementation
- ✅ Discovery endpoint: `/discovery`
- ✅ Health endpoint: `/`
- ✅ Execute endpoint: `POST /`
- ✅ Product description generation logic
- ✅ CORS configuration
- ✅ Error handling

**Ready for:** Immediate use with Optimizely Opal!

---

## ⚡ **Quick Start: Manual Upload Steps**

1. Open: **https://dash.cloudflare.com/**
2. Workers & Pages → Create Worker
3. Name: `product-description-generator`
4. Deploy → Quick edit
5. Copy code from: `dist/index.js`
6. Paste and Save
7. Get URL
8. Test `/discovery` endpoint
9. Register with Opal

**Total time: 2-3 minutes**

---

## 💡 **Pro Tips**

### After First Deployment
- Bookmark your Worker URL
- Add it to your documentation
- Share with your team
- Monitor usage in Cloudflare dashboard

### For Future Updates
1. Make code changes
2. Run `npm run build`
3. In Cloudflare dashboard:
   - Go to your Worker
   - Click "Quick edit"
   - Paste new code
   - Save and Deploy

### Setting Up CI/CD
Once you have the API token, add it to GitHub secrets:
- Secret name: `CLOUDFLARE_API_TOKEN`
- Then GitHub Actions can deploy automatically

---

## 🎉 **You're Almost There!**

Your code is:
- ✅ Built successfully
- ✅ SDK compliant
- ✅ All tests passing
- ✅ Ready to deploy

**Choose Option 1 (Manual Upload) for the fastest deployment!**

---

## 📞 **Need Help?**

### Cloudflare Support
- Dashboard: https://dash.cloudflare.com/
- Docs: https://developers.cloudflare.com/workers/
- Community: https://discord.gg/cloudflaredev

### This Tool
- GitHub: https://github.com/vaibhavWar/opalsdk
- Documentation: See README.md

---

**Ready to deploy? Start with Option 1 above!** 🚀

