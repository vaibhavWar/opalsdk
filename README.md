# Product Description Generator - Optimizely Opal Tool

A custom tool for **Optimizely Opal** that generates comprehensive, marketing-ready product descriptions based on product name, part number, and attributes. Available in both **TypeScript** (for Cloudflare Workers) and **C#** (.NET) implementations.

## 📋 Overview

This tool integrates with Optimizely Opal's AI assistant to automatically generate professional product descriptions. It creates structured, markdown-formatted content including:

- Product overview
- Key features
- Technical specifications
- Product attributes
- Use cases and applications
- Marketing benefits

## 🚀 Features

- ✅ **Two implementations**: TypeScript and C# for different deployment scenarios
- ✅ **Structured output**: Markdown-formatted descriptions perfect for AI consumption
- ✅ **Flexible inputs**: Accepts product name, part number, and variable attribute lists
- ✅ **Discovery endpoint**: Self-describing tool that integrates seamlessly with Opal
- ✅ **CORS support**: Ready for web integration
- ✅ **Error handling**: Comprehensive validation and error messages
- ✅ **Health checks**: Built-in monitoring endpoints

## 📦 Prerequisites

### For TypeScript Version:
- Node.js 18+ and npm
- Cloudflare Workers account (free tier available)
- Wrangler CLI
- Optimizely Opal access with Tools (beta) enabled

### For C# Version:
- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code
- Optimizely Opal access with Tools (beta) enabled

## 🛠️ Installation & Setup

### TypeScript (Cloudflare Workers)

#### 1. Install Dependencies

```bash
npm install
```

#### 2. Configure Cloudflare Workers

Update `wrangler.toml` with your Worker name if needed:

```toml
name = "product-description-generator"
main = "src/index.ts"
compatibility_date = "2024-01-01"
```

#### 3. Build the Project

```bash
npm run build
```

#### 4. Test Locally

```bash
npm run dev
```

This starts a local server at `http://localhost:8787`

#### 5. Deploy to Cloudflare

**Deploy to staging:**
```bash
npm run deploy:staging
```

**Deploy to production:**
```bash
npm run deploy
```

After deployment, you'll receive a URL like:
`https://product-description-generator.your-subdomain.workers.dev`

### C# (.NET)

#### 1. Restore Dependencies

```bash
cd csharp
dotnet restore
```

#### 2. Run Locally

```bash
dotnet run
```

The API will start at `http://localhost:5000`

#### 3. Build for Production

```bash
dotnet build -c Release
```

#### 4. Deploy

Deploy to your preferred hosting platform:
- **Azure App Service**
- **AWS Lambda**
- **Google Cloud Run**
- **On-premises IIS**

## 🔧 Testing the Tool

### Test Discovery Endpoint

**TypeScript:**
```bash
curl https://your-worker.workers.dev/discovery
```

**C#:**
```bash
curl http://localhost:5000/discovery
```

### Test Health Check

```bash
curl https://your-worker.workers.dev/
```

### Test Product Description Generation

```bash
curl -X POST https://your-worker.workers.dev/ \
  -H "Content-Type: application/json" \
  -d '{
    "productName": "Professional Drill Set",
    "partNumber": "DRL-2024-PRO",
    "attributes": [
      "Color: Black and Orange",
      "Power: 20V Lithium-Ion",
      "Chuck Size: 1/2 inch",
      "Speed: Variable 0-1,500 RPM",
      "Weight: 3.5 lbs",
      "Includes: 50-piece accessory kit"
    ]
  }'
```

**Example Response:**

```json
{
  "success": true,
  "content": "# Product Description\n\n## Professional Drill Set\n\n**Part Number:** `DRL-2024-PRO`\n\n..."
}
```

## 🔗 Integrating with Optimizely Opal

### 1. Enable Opal Tools (Beta)

Contact your Optimizely Customer Success Manager (CSM) to enable Opal Tools beta access.

### 2. Register Your Tool

1. Navigate to [https://opal.optimizely.com/tools](https://opal.optimizely.com/tools)
2. Click **"Add Custom Tool"**
3. Enter your discovery URL:
   - TypeScript: `https://your-worker.workers.dev/discovery`
   - C#: `https://your-api-domain.com/discovery`
4. Configure authentication if needed (optional)
5. Click **"Save"**

### 3. Test Integration

In the Optimizely Opal chat:

```
Generate a product description for:
- Product Name: Wireless Keyboard
- Part Number: KB-W-2024
- Attributes: Bluetooth 5.0, Rechargeable Battery, 78 Keys
```

Opal will automatically detect and use your custom tool!

## 📝 API Reference

### Endpoints

#### `GET /discovery`
Returns the tool definition for Optimizely Opal.

**Response:**
```json
{
  "functions": [
    {
      "name": "product-description-generator",
      "description": "Generates comprehensive product descriptions...",
      "parameters": [...],
      "endpoint": "/",
      "http_method": "POST"
    }
  ]
}
```

#### `GET /` (Health Check)
Returns service status.

**Response:**
```json
{
  "status": "healthy",
  "tool": "product-description-generator",
  "version": "1.0.0"
}
```

#### `POST /` (Generate Description)
Generates a product description.

**Request:**
```json
{
  "productName": "string (required)",
  "partNumber": "string (required)",
  "attributes": ["string"] (optional)
}
```

**Response:**
```json
{
  "success": true,
  "content": "markdown formatted description"
}
```

## 🎯 Use Cases

### E-commerce Platforms
Automatically generate product descriptions for new inventory items.

### Content Management Systems
Bulk generate descriptions for product catalogs.

### Marketing Teams
Quickly create consistent, professional product copy.

### Product Information Management (PIM)
Enhance product data with AI-generated descriptions.

## 🔒 Security Considerations

### Authentication (Optional)

To add bearer token authentication:

**TypeScript:**
```typescript
const authHeader = request.headers.get('Authorization');
if (authHeader !== 'Bearer YOUR_SECRET_TOKEN') {
  return new Response('Unauthorized', { status: 401 });
}
```

**C#:**
```csharp
app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"];
    if (authHeader != "Bearer YOUR_SECRET_TOKEN")
    {
        context.Response.StatusCode = 401;
        return;
    }
    await next();
});
```

### Environment Variables

Store sensitive data in environment variables:

**Cloudflare Workers:**
```bash
wrangler secret put API_KEY
```

**.NET:**
Use User Secrets or Azure Key Vault.

## 🏗️ Project Structure

```
productdescriptiongenerator/
├── src/
│   └── index.ts              # TypeScript implementation
├── csharp/
│   ├── Program.cs            # C# implementation
│   ├── ProductDescriptionGenerator.csproj
│   └── appsettings.json
├── package.json              # Node dependencies
├── tsconfig.json             # TypeScript config
├── wrangler.toml             # Cloudflare config
├── .gitignore
└── README.md
```

## 🐛 Troubleshooting

### TypeScript Issues

**Error: Module not found**
```bash
npm install
npm run build
```

**Deployment fails**
- Check Wrangler authentication: `wrangler login`
- Verify wrangler.toml configuration

### C# Issues

**Port already in use**
- Change port in `appsettings.json`
- Or set environment variable: `ASPNETCORE_URLS=http://localhost:5001`

**Package restore fails**
```bash
dotnet nuget locals all --clear
dotnet restore
```

## 📚 Additional Resources

- [Optimizely Opal Documentation](https://support.optimizely.com/hc/en-us/articles/39190444893837-Create-custom-tools)
- [Opal Tools SDK (npm)](https://www.npmjs.com/package/@optimizely-opal/opal-tools-sdk)
- [Building Custom Tools Guide](https://www.david-tec.com/2025/08/a-guide-to-building-custom-tools-for-optimizely-opal/)
- [Cloudflare Workers Documentation](https://developers.cloudflare.com/workers/)
- [.NET Web API Documentation](https://learn.microsoft.com/en-us/aspnet/core/web-api/)

## 🤝 Contributing

Contributions are welcome! Feel free to:
- Report bugs
- Suggest features
- Submit pull requests
- Improve documentation

## 📄 License

MIT License - feel free to use this tool in your projects!

## 💡 Tips for Customization

### Add More Product Details
Extend the `ProductDescriptionParams` interface to include:
- Price
- Dimensions
- Weight
- Materials
- Warranty information

### Integrate with External APIs
- Fetch product data from your PIM system
- Call AI services for enhanced descriptions
- Integrate with translation APIs for multilingual support

### Add Templates
Create industry-specific templates for:
- Electronics
- Clothing & Fashion
- Industrial equipment
- Software products

### Example: Adding Price Support

**TypeScript:**
```typescript
interface ProductDescriptionParams {
  productName: string;
  partNumber: string;
  attributes: string[];
  price?: number;  // Add price field
}
```

Update the description generator to include pricing information in the output.

## 📞 Support

For issues with:
- **This tool**: Open a GitHub issue
- **Optimizely Opal**: Contact Optimizely support
- **Cloudflare Workers**: Check Cloudflare documentation

---

**Built for Optimizely Opal** | Powered by AI | Deployed Globally

*This tool demonstrates the power of custom Optimizely Opal tools. Extend it to match your specific business needs!*

