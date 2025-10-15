# Deployment Guide

Comprehensive deployment instructions for both TypeScript and C# implementations.

## 📦 TypeScript - Cloudflare Workers

### Prerequisites
- Cloudflare account (free tier works)
- Wrangler CLI installed
- Node.js 18+ and npm

### Deployment Steps

#### 1. Login to Cloudflare
```bash
npx wrangler login
```
This opens a browser window to authenticate.

#### 2. Configure Your Worker
Edit `wrangler.toml` if needed:
```toml
name = "product-description-generator"  # Your worker name
main = "src/index.ts"
compatibility_date = "2024-01-01"
```

#### 3. Deploy to Production
```bash
npm run deploy
```

#### 4. Get Your Worker URL
After deployment, you'll see:
```
Published product-description-generator
  https://product-description-generator.your-subdomain.workers.dev
```

#### 5. Verify Deployment
```bash
curl https://product-description-generator.your-subdomain.workers.dev/discovery
```

### Environment Variables & Secrets

Add secrets to your worker:
```bash
# Add API key
wrangler secret put API_KEY

# Add webhook secret
wrangler secret put WEBHOOK_SECRET
```

Use in code:
```typescript
export default {
  async fetch(request: Request, env: any): Promise<Response> {
    const apiKey = env.API_KEY;
    // ... your code
  }
}
```

### Custom Domain (Optional)

1. Go to Cloudflare dashboard
2. Workers & Pages → your worker → Settings → Triggers
3. Add custom domain: `api.yourdomain.com`
4. Update Opal with: `https://api.yourdomain.com/discovery`

---

## 🔷 C# - Multiple Deployment Options

### Option 1: Azure App Service

#### Quick Deploy
```bash
cd csharp

# Login to Azure
az login

# Create and deploy in one command
az webapp up --name product-desc-gen --runtime "DOTNET:8.0" --sku B1
```

#### Your URL
```
https://product-desc-gen.azurewebsites.net
```

#### Configure App Settings
```bash
az webapp config appsettings set \
  --name product-desc-gen \
  --resource-group <your-resource-group> \
  --settings API_KEY=your-secret-key
```

### Option 2: Docker (Any Platform)

#### Build Docker Image
```bash
cd csharp
docker build -t product-description-generator .
```

#### Run Locally
```bash
docker run -p 8080:8080 product-description-generator
```

#### Deploy to Docker Hub
```bash
docker tag product-description-generator yourusername/product-description-generator
docker push yourusername/product-description-generator
```

#### Deploy to Cloud Run (Google Cloud)
```bash
# Tag for Google Cloud
docker tag product-description-generator gcr.io/YOUR-PROJECT/product-description-generator

# Push to Container Registry
docker push gcr.io/YOUR-PROJECT/product-description-generator

# Deploy to Cloud Run
gcloud run deploy product-description-generator \
  --image gcr.io/YOUR-PROJECT/product-description-generator \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated
```

### Option 3: AWS Lambda (with .NET)

#### Install AWS Lambda Tools
```bash
dotnet tool install -g Amazon.Lambda.Tools
```

#### Deploy
```bash
cd csharp
dotnet lambda deploy-serverless
```

### Option 4: Traditional IIS (Windows Server)

#### Publish from Visual Studio
1. Right-click project → Publish
2. Choose Folder
3. Target: `bin\Release\net8.0\publish`
4. Click Publish

#### Configure IIS
1. Open IIS Manager
2. Add Application Pool (.NET 8.0, No Managed Code)
3. Add Website
4. Point to publish folder
5. Set bindings (HTTP/HTTPS)

#### Install .NET Hosting Bundle
Download from: https://dotnet.microsoft.com/download/dotnet/8.0

---

## 🔒 Security Best Practices

### 1. Add Authentication

**Cloudflare Workers:**
```typescript
const authHeader = request.headers.get('Authorization');
if (authHeader !== `Bearer ${env.API_KEY}`) {
  return new Response('Unauthorized', { status: 401 });
}
```

**C# .NET:**
```csharp
app.Use(async (context, next) =>
{
    var apiKey = context.Request.Headers["X-API-Key"];
    if (apiKey != Environment.GetEnvironmentVariable("API_KEY"))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
        return;
    }
    await next();
});
```

### 2. Rate Limiting

**Cloudflare Workers:**
Use Cloudflare's built-in rate limiting in dashboard.

**C# .NET:**
```bash
dotnet add package AspNetCoreRateLimit
```

### 3. HTTPS Only

**Cloudflare:** Automatic
**Azure:** Enable in portal
**AWS:** Use API Gateway

### 4. CORS Configuration

Restrict origins in production:

**TypeScript:**
```typescript
const corsHeaders = {
  'Access-Control-Allow-Origin': 'https://opal.optimizely.com',
  // ...
};
```

**C#:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://opal.optimizely.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

---

## 📊 Monitoring & Logging

### Cloudflare Workers

#### View Logs (Live Tail)
```bash
wrangler tail
```

#### Analytics Dashboard
- Go to Cloudflare dashboard
- Workers & Pages → your worker → Metrics
- View requests, errors, CPU time

### Azure App Service

#### View Logs
```bash
az webapp log tail --name product-desc-gen
```

#### Application Insights (Recommended)
```bash
az monitor app-insights component create \
  --app product-desc-gen-insights \
  --resource-group <your-rg>
```

### Docker/Kubernetes

#### Container Logs
```bash
docker logs -f <container-id>
```

#### Kubernetes Logs
```bash
kubectl logs -f deployment/product-description-generator
```

---

## 🚀 CI/CD Automation

### GitHub Actions - Cloudflare

Create `.github/workflows/deploy.yml`:
```yaml
name: Deploy to Cloudflare Workers

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          
      - name: Install dependencies
        run: npm install
        
      - name: Deploy to Cloudflare Workers
        uses: cloudflare/wrangler-action@v3
        with:
          apiToken: ${{ secrets.CLOUDFLARE_API_TOKEN }}
```

### GitHub Actions - Azure

```yaml
name: Deploy to Azure

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0'
          
      - name: Build
        run: |
          cd csharp
          dotnet build -c Release
          
      - name: Publish
        run: |
          cd csharp
          dotnet publish -c Release -o ./publish
          
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: product-desc-gen
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: csharp/publish
```

---

## 🧪 Testing Deployment

### Automated Testing Script

**TypeScript/Bash:**
```bash
# Set your deployed URL
export BASE_URL="https://your-worker.workers.dev"

# Run tests
./examples/test.sh $BASE_URL
```

**PowerShell:**
```powershell
# Set your deployed URL
$BaseUrl = "https://your-worker.workers.dev"

# Run tests
.\examples\test.ps1 -BaseUrl $BaseUrl
```

### Health Check Endpoint

Monitor with:
- UptimeRobot
- Pingdom
- StatusCake
- Built-in cloud provider monitoring

Add to monitoring:
```
GET https://your-api.com/
Expected: 200 OK
Contains: "status": "healthy"
```

---

## 🔄 Rolling Back

### Cloudflare Workers
```bash
# List deployments
wrangler deployments list

# Rollback to previous version
wrangler rollback
```

### Azure App Service
```bash
az webapp deployment slot swap \
  --name product-desc-gen \
  --slot staging
```

---

## 📈 Scaling

### Cloudflare Workers
- **Free tier:** 100,000 requests/day
- **Paid:** $5/10M requests
- Automatic global scaling
- Zero cold starts

### Azure App Service
- Scale up: Increase tier (B1 → S1 → P1V2)
- Scale out: Add instances
```bash
az appservice plan update \
  --name myAppServicePlan \
  --number-of-workers 3
```

### Docker/Kubernetes
```yaml
# kubernetes-deployment.yml
spec:
  replicas: 3  # Number of pods
  resources:
    requests:
      cpu: "100m"
      memory: "128Mi"
    limits:
      cpu: "500m"
      memory: "512Mi"
```

---

## 🛠️ Troubleshooting

### Common Issues

**"Worker exceeded CPU time"**
- Optimize code
- Move heavy processing to async
- Consider caching

**"502 Bad Gateway"**
- Check app is running
- Verify port configuration
- Check firewall rules

**"CORS errors"**
- Add proper CORS headers
- Check origin whitelist
- Verify preflight handling

**"Unauthorized" errors**
- Check API key configuration
- Verify header format
- Test authentication separately

---

## 📞 Support

- **Cloudflare Workers:** https://discord.gg/cloudflaredev
- **Azure:** https://azure.microsoft.com/support/
- **Docker:** https://forums.docker.com/
- **This Project:** Open a GitHub issue

---

Ready to deploy? Choose your platform and follow the steps above!

