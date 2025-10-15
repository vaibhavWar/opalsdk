# Cloudflare Workers Deployment Guide

## Step-by-Step Deployment Instructions

### Prerequisites
- Cloudflare account (free tier works)
- Wrangler CLI installed (already included in your project)
- Project built successfully ✅

---

## Quick Deployment (3 Methods)

### Method 1: OAuth Login (Recommended)

1. **Open PowerShell/Terminal in project directory**
   ```powershell
   cd c:\Projects\Opal\productdescriptiongenerator
   ```

2. **Login to Cloudflare**
   ```powershell
   npx wrangler login
   ```
   - A browser window will open
   - Click **"Allow"** to authorize Wrangler
   - You have 60 seconds to complete authorization
   - Keep the terminal window open

3. **Deploy**
   ```powershell
   npm run deploy
   ```

4. **Get your Worker URL**
   - Copy the URL from the output
   - Example: `https://product-description-generator.yourname.workers.dev`

---

### Method 2: API Token (More Reliable)

If OAuth login has issues, use an API token:

#### Step 1: Create API Token
1. Go to: https://dash.cloudflare.com/profile/api-tokens
2. Click **"Create Token"**
3. Use template: **"Edit Cloudflare Workers"**
4. Or create custom token with these permissions:
   - Account: Workers Scripts - Edit
   - Account: Workers KV Storage - Edit
   - Account: Workers Routes - Edit
5. Click **"Continue to summary"**
6. Click **"Create Token"**
7. **Copy the token** (you won't see it again!)

#### Step 2: Set Environment Variable
```powershell
# PowerShell (Windows)
$env:CLOUDFLARE_API_TOKEN="your-token-here"

# Or permanently:
[System.Environment]::SetEnvironmentVariable('CLOUDFLARE_API_TOKEN', 'your-token-here', 'User')
```

#### Step 3: Get Account ID
1. Go to: https://dash.cloudflare.com/
2. Click on **"Workers & Pages"** in left sidebar
3. On the right side, find your **Account ID**
4. Copy it

#### Step 4: Update wrangler.toml
Add your account_id to `wrangler.toml`:
```toml
name = "product-description-generator"
main = "src/index.ts"
compatibility_date = "2024-01-01"
account_id = "your-account-id-here"  # Add this line
```

#### Step 5: Deploy
```powershell
npm run deploy
```

---

### Method 3: Cloudflare Dashboard (Manual Upload)

If CLI deployment fails, you can deploy manually:

#### Step 1: Build the Project
```powershell
npm run build
```

#### Step 2: Prepare the Worker Script
The compiled file is in: `dist/index.js`

#### Step 3: Go to Cloudflare Dashboard
1. Visit: https://dash.cloudflare.com/
2. Click **"Workers & Pages"** in left sidebar
3. Click **"Create application"**
4. Click **"Create Worker"**
5. Give it a name: `product-description-generator`
6. Click **"Deploy"**

#### Step 4: Upload Your Code
1. Click **"Quick edit"**
2. Delete the default code
3. Copy the contents of `dist/index.js`
4. Paste into the editor
5. Click **"Save and Deploy"**

#### Step 5: Get Your URL
Your Worker URL will be:
```
https://product-description-generator.your-subdomain.workers.dev
```

---

## Troubleshooting

### Issue: "Timed out waiting for authorization code"

**Solution:**
- Complete the authorization within 60 seconds
- Make sure pop-ups aren't blocked
- Try Method 2 (API Token) instead

### Issue: "You are not authenticated"

**Solution:**
```powershell
# Clear any cached credentials
npx wrangler logout

# Try login again
npx wrangler login
```

### Issue: "Account ID not found"

**Solution:**
Add account_id to wrangler.toml (see Method 2, Step 4)

### Issue: "Module not found" errors

**Solution:**
```powershell
npm install
npm run build
```

### Issue: Wrangler version warning

**Solution:**
```powershell
npm install --save-dev wrangler@4
```

---

## Verification Steps

### 1. Test Health Endpoint
```powershell
curl https://your-worker-url.workers.dev/
```

**Expected Response:**
```json
{
  "status": "healthy",
  "tool": "product-description-generator",
  "version": "1.0.0"
}
```

### 2. Test Discovery Endpoint
```powershell
curl https://your-worker-url.workers.dev/discovery
```

**Expected Response:**
```json
{
  "functions": [
    {
      "name": "product-description-generator",
      "parameters": [...]
    }
  ]
}
```

### 3. Test Product Generation
```powershell
$body = @{
    productName = "Test Product"
    partNumber = "TP-001"
    attributes = @("Feature 1", "Feature 2")
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://your-worker-url.workers.dev/" `
  -Method Post -Body $body -ContentType "application/json"
```

---

## Post-Deployment Steps

### 1. Save Your Worker URL
Your Worker URL format:
```
https://product-description-generator.[your-subdomain].workers.dev
```

### 2. Register with Optimizely Opal
1. Go to: https://opal.optimizely.com/tools
2. Click **"Add Custom Tool"**
3. Enter your discovery URL:
   ```
   https://your-worker-url.workers.dev/discovery
   ```
4. Click **"Save"**

### 3. Test in Opal
In Optimizely Opal chat, try:
```
Generate a product description for:
- Product: Wireless Mouse
- Part Number: WM-2024
- Features: Bluetooth 5.0, Ergonomic design
```

---

## Environment Configuration

### Development
```powershell
npm run dev
# Local: http://localhost:8787
```

### Staging (Optional)
```powershell
npm run deploy:staging
# Creates: product-description-generator-staging.workers.dev
```

### Production
```powershell
npm run deploy
# Creates: product-description-generator.workers.dev
```

---

## Custom Domain (Optional)

To use your own domain (e.g., api.yourcompany.com):

1. Go to Cloudflare Dashboard
2. Select your Worker
3. Click **"Settings"** → **"Triggers"**
4. Under **"Custom Domains"**, click **"Add Custom Domain"**
5. Enter your domain: `api.yourcompany.com`
6. Click **"Add Domain"**

DNS will be configured automatically!

---

## Monitoring & Logs

### View Logs (Real-time)
```powershell
npx wrangler tail
```

### View Analytics
1. Go to Cloudflare Dashboard
2. Click your Worker
3. Click **"Metrics"** tab
4. View:
   - Requests per second
   - Error rate
   - CPU time
   - Success rate

---

## Cost & Limits

### Free Tier
- **100,000 requests/day**
- **10ms CPU time per request**
- More than enough for most use cases!

### Paid Plan ($5/month)
- **10 million requests/month**
- **50ms CPU time per request**
- Perfect for production use

---

## Quick Reference

### Deploy Commands
```powershell
# Build
npm run build

# Deploy to production
npm run deploy

# Deploy to staging
npm run deploy:staging

# View logs
npx wrangler tail

# Check auth status
npx wrangler whoami

# Login
npx wrangler login

# Logout
npx wrangler logout
```

---

## Success Checklist

- [ ] Cloudflare account created
- [ ] Authenticated with Wrangler
- [ ] Project built successfully
- [ ] Deployed to Cloudflare Workers
- [ ] Worker URL obtained
- [ ] Health endpoint tested
- [ ] Discovery endpoint tested
- [ ] Product generation tested
- [ ] Registered with Optimizely Opal
- [ ] Tested in Opal chat

---

## Need Help?

### Common Issues
1. **Authentication problems** → Use Method 2 (API Token)
2. **Deployment fails** → Use Method 3 (Manual upload)
3. **Worker not responding** → Check logs with `npx wrangler tail`

### Support Resources
- Cloudflare Workers Docs: https://developers.cloudflare.com/workers/
- Wrangler CLI Docs: https://developers.cloudflare.com/workers/wrangler/
- Community Discord: https://discord.gg/cloudflaredev

---

## Next Steps After Deployment

1. ✅ Share Worker URL with your team
2. ✅ Register with Optimizely Opal
3. ✅ Test in production
4. ✅ Monitor usage and performance
5. ✅ Set up custom domain (optional)
6. ✅ Configure rate limiting if needed
7. ✅ Add authentication if required

---

**Your Worker will be live at:**
```
https://product-description-generator.[your-subdomain].workers.dev
```

**Register this URL with Opal:**
```
https://product-description-generator.[your-subdomain].workers.dev/discovery
```

Good luck with your deployment! 🚀

