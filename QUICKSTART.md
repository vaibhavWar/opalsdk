# Quick Start Guide

Get your Product Description Generator up and running in under 5 minutes!

## 🚀 TypeScript + Cloudflare Workers (Recommended)

### Step 1: Install Dependencies (30 seconds)
```bash
npm install
```

### Step 2: Test Locally (1 minute)
```bash
npm run dev
```

Open a new terminal and test:
```bash
curl http://localhost:8787/discovery
```

### Step 3: Test Product Description
```bash
curl -X POST http://localhost:8787/ \
  -H "Content-Type: application/json" \
  -d '{
    "productName": "Smart Watch Pro",
    "partNumber": "SW-PRO-2024",
    "attributes": [
      "Display: AMOLED 1.4 inch",
      "Battery: 7 days",
      "Water Resistance: IP68"
    ]
  }'
```

### Step 4: Deploy to Cloudflare (2 minutes)

**First time setup:**
```bash
# Login to Cloudflare
npx wrangler login

# Deploy
npm run deploy
```

You'll get a URL like: `https://product-description-generator.your-name.workers.dev`

### Step 5: Register with Opal (1 minute)

1. Go to [https://opal.optimizely.com/tools](https://opal.optimizely.com/tools)
2. Click "Add Custom Tool"
3. Enter: `https://product-description-generator.your-name.workers.dev/discovery`
4. Save

**Done!** Try in Opal chat:
```
Generate a product description for Smart Watch Pro (part SW-PRO-2024) with AMOLED display
```

---

## 🔷 C# + .NET Alternative

### Step 1: Run Locally
```bash
cd csharp
dotnet run
```

### Step 2: Test
```bash
curl http://localhost:5000/discovery
```

### Step 3: Deploy
Choose your platform:
- **Azure App Service**: `az webapp up`
- **Docker**: `docker build -t product-gen .`
- **IIS**: Publish from Visual Studio

---

## 🧪 Quick Tests

### Windows (PowerShell)
```powershell
.\examples\test.ps1
```

### Mac/Linux (Bash)
```bash
chmod +x examples/test.sh
./examples/test.sh
```

---

## 🎯 Example Usage in Opal

Once registered, you can use natural language:

**Simple:**
```
Create a product description for "Bluetooth Speaker" part number SP-BT-100
```

**Detailed:**
```
Generate a description for:
- Product: Professional Coffee Maker
- Part #: CM-2024-PRO
- Features: 12-cup capacity, programmable timer, thermal carafe
```

**Bulk:**
```
Create descriptions for these products:
1. Wireless Mouse (MS-W-100) - Ergonomic, 2.4GHz
2. USB Hub (HUB-4-2024) - 4 ports, USB 3.0
3. Laptop Stand (LS-AL-2024) - Aluminum, adjustable
```

---

## 📝 Common Issues

**"Module not found"**
```bash
npm install
```

**Port already in use**
```bash
# Change port in wrangler.toml or for C#:
dotnet run --urls="http://localhost:5001"
```

**Wrangler not found**
```bash
npx wrangler login
```

**Cloudflare auth fails**
```bash
wrangler logout
wrangler login
```

---

## 🎓 Next Steps

- ✅ Add authentication (see README.md security section)
- ✅ Customize product templates
- ✅ Integrate with your PIM/database
- ✅ Add multilingual support
- ✅ Monitor usage with Cloudflare Analytics

---

## 💡 Pro Tips

1. **Test locally first**: Always use `npm run dev` before deploying
2. **Use staging**: Deploy to staging with `npm run deploy:staging`
3. **Check logs**: Use `wrangler tail` to see live logs
4. **Version control**: Tag releases with `git tag v1.0.0`
5. **Monitor costs**: Check Cloudflare dashboard for usage

---

Need more help? Check the full [README.md](README.md) for detailed documentation!

