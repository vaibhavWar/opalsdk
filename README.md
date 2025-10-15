# Product Description Generator - Optimizely Opal Tool

**C# Implementation using Optimizely.Opal.Tools SDK v0.4.0**

A production-ready Opal custom tool that generates natural, AI-like product descriptions based on product name, part number, and attributes.

## 🎯 Features

- ✅ **Official Optimizely Opal SDK Integration** (`Optimizely.Opal.Tools` v0.4.0)
- ✅ **Natural AI-like Descriptions** - Dynamically generated, professional content
- ✅ **Type & Tone Parameters** - Customize output for ecommerce, technical, or marketing
- ✅ **Clean Architecture** - Separation of concerns with Models, Services, and Tools layers
- ✅ **Dependency Injection** - Testable, maintainable code structure
- ✅ **No Character Limits** - Generate complete, natural descriptions
- ✅ **Auto-Generated Discovery** - Opal SDK handles endpoint generation

## 📋 Prerequisites

- .NET 8.0 SDK or later
- Optimizely Opal instance (for testing)

## 🚀 Quick Start

### Build and Run

```bash
cd csharp
dotnet restore
dotnet build
dotnet run
```

Server starts at: `http://localhost:5000`

### Test Discovery Endpoint

```bash
curl http://localhost:5000/discovery
```

### Test Tool Execution

```bash
curl -X POST http://localhost:5000/tools/product-description-generator \
  -H "Content-Type: application/json" \
  -d '{
    "parameters": {
      "ProductName": "DEWALT 20V Acrylic Dispenser 101 28 oz",
      "PartNumber": "211DCE595D1",
      "Attributes": [
        "Brand: DEWALT",
        "Battery Voltage (V): 20",
        "Capacity: 28 oz.",
        "Cordless / Corded: Cordless"
      ],
      "Type": "ecommerce",
      "Tone": "professional"
    }
  }'
```

## 📊 Architecture

```
csharp/
├── Program.cs                          # Application entry point & DI configuration
├── Models/
│   └── ProductDescriptionParameters.cs # Request parameters (DTOs)
├── Services/
│   ├── IDescriptionGenerationService.cs    # Service interface
│   └── DescriptionGenerationService.cs     # Business logic implementation
└── Tools/
    └── ProductDescriptionGeneratorTool.cs  # Opal tool with [OpalTool] attribute
```

### Clean Architecture Principles

- **Separation of Concerns**: Each layer has a single responsibility
- **Dependency Injection**: Services injected via constructor
- **Interface-Based Design**: Easy to test and extend
- **SOLID Principles**: SRP, OCP, LSP, ISP, DIP all applied

## 🔧 Parameters

| Parameter | Type | Required | Description | Example |
|-----------|------|----------|-------------|---------|
| **ProductName** | string | Yes | Name of the product | `"DEWALT 20V Acrylic Dispenser"` |
| **PartNumber** | string | Yes | Product part number or SKU | `"211DCE595D1"` |
| **Attributes** | array | Yes | Product attributes, features, specifications | `["Brand: DEWALT", "Voltage: 20V"]` |
| **Type** | string | No | Content category | `"ecommerce"`, `"technical"`, `"marketing"`, `"general"` |
| **Tone** | string | No | Description tone | `"professional"`, `"casual"`, `"enthusiastic"` |

### Default Values

- **Type**: `general` (if not provided)
- **Tone**: `professional` (if not provided)

## 📝 Example Output

### Request
```json
{
  "parameters": {
    "ProductName": "DEWALT 20V Acrylic Dispenser 101 28 oz",
    "PartNumber": "211DCE595D1",
    "Attributes": [
      "Brand: DEWALT",
      "Battery Voltage (V): 20",
      "Capacity: 28 oz.",
      "Cartridge Type: 27-28 oz. Adhesive 10:1",
      "Cordless / Corded: Cordless"
    ],
    "Type": "ecommerce",
    "Tone": "professional"
  }
}
```

### Response
```json
{
  "content": "The DEWALT 20V Acrylic Dispenser 101 28 oz (Part# 211DCE595D1) delivers powerful, reliable cordless performance for professional applications. Features include capacity: 28 oz., 27-28 oz. Adhesive 10:1. Built with DEWALT quality and reliability. Designed for demanding applications.",
  "productName": "DEWALT 20V Acrylic Dispenser 101 28 oz",
  "partNumber": "211DCE595D1",
  "attributeCount": 5,
  "type": "ecommerce",
  "tone": "professional"
}
```

## 🔄 How Type & Tone Affect Output

### Default (general + professional)
> "delivers powerful cordless performance."

### Ecommerce + Professional
> "delivers powerful, **reliable** cordless performance **for professional applications**."

## 🧪 Testing

### Unit Testing Strategy
```csharp
// Mock the service
var mockService = new Mock<IDescriptionGenerationService>();
mockService
    .Setup(s => s.GenerateProductDescription(
        It.IsAny<string>(), 
        It.IsAny<string>(), 
        It.IsAny<List<string>>(),
        It.IsAny<string>(),
        It.IsAny<string>()))
    .Returns("Test description");

// Inject into tool
var tool = new ProductDescriptionGeneratorTool(mockService.Object);
```

### Integration Testing

Use PowerShell (Windows):
```powershell
.\examples\test.ps1
```

Or use the test-requests.json with Postman/Insomnia.

## 📦 Dependencies

- **Optimizely.Opal.Tools** v0.4.0 - Official Opal SDK
- **Microsoft.AspNetCore.OpenApi** v8.0.0 - OpenAPI support
- **Swashbuckle.AspNetCore** v6.8.0 - Swagger documentation

## 🏗️ Deployment

### Local Development
```bash
dotnet run --project csharp/ProductDescriptionGenerator.csproj
```

### Production Build
```bash
dotnet publish -c Release -o ./publish
```

### Docker
```bash
docker build -t product-description-generator ./csharp
docker run -p 5000:5000 product-description-generator
```

## 📚 Documentation

- [C# Clean Architecture README](csharp/README.md) - Detailed architecture documentation
- [SDK Integration Guide](SDK_INTEGRATION_COMPLETE.md) - How the SDK is integrated

## 🔗 Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/discovery` | GET | Tool definition and parameters (auto-generated by SDK) |
| `/tools/product-description-generator` | POST | Execute tool (auto-generated by SDK) |

## 🎓 References

- [Optimizely Opal Tools SDK](https://www.nuget.org/packages/Optimizely.Opal.Tools/)
- [Optimizely Academy - Building Simple Tools](https://academy.optimizely.com/student/path/2839076/activity/4331694)
- [Optimizely Support - Create Custom Tools](https://support.optimizely.com/hc/en-us/articles/39190444893837-Create-custom-tools)

## 📄 License

MIT License - See [LICENSE](LICENSE) file

## 🤝 Contributing

This is a reference implementation. Feel free to use it as a starting point for your own Opal custom tools!

---

**Built with Optimizely Opal SDK** | **Clean Architecture** | **Production Ready**
