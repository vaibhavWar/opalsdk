# Product Description Generator - Project Summary

## Overview

A production-ready Optimizely Opal custom tool that generates natural, AI-like product descriptions based on product name, part number, and attributes.

**Implementation**: C# with Optimizely.Opal.Tools SDK v0.4.0

## Key Features

- ✅ Official Optimizely Opal SDK integration
- ✅ Clean architecture with separation of concerns
- ✅ Dependency injection for testability
- ✅ Type and Tone parameters for customization
- ✅ Natural, AI-like description generation
- ✅ No character limits
- ✅ Auto-generated discovery endpoints

## Technology Stack

- **Language**: C#
- **Framework**: .NET 8.0
- **SDK**: Optimizely.Opal.Tools v0.4.0
- **Architecture**: Clean Architecture (Models, Services, Tools)
- **API**: Minimal APIs with Swagger/OpenAPI

## Project Structure

```
csharp/
├── Program.cs                              # Application startup
├── ProductDescriptionGenerator.csproj      # Project file
├── Models/
│   └── ProductDescriptionParameters.cs     # Request parameters
├── Services/
│   ├── IDescriptionGenerationService.cs    # Service interface
│   └── DescriptionGenerationService.cs     # Business logic
└── Tools/
    └── ProductDescriptionGeneratorTool.cs  # Opal tool implementation
```

## Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/discovery` | GET | Tool definition (SDK auto-generated) |
| `/tools/product-description-generator` | POST | Execute tool (SDK auto-generated) |

## Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| ProductName | string | Yes | - | Product name |
| PartNumber | string | Yes | - | Part number/SKU |
| Attributes | array | Yes | - | Product attributes |
| Type | string | No | "general" | Content category |
| Tone | string | No | "professional" | Description tone |

## Example Usage

### Request
```json
{
  "parameters": {
    "ProductName": "DEWALT 20V Acrylic Dispenser 101 28 oz",
    "PartNumber": "211DCE595D1",
    "Attributes": [
      "Brand: DEWALT",
      "Battery Voltage (V): 20",
      "Capacity: 28 oz."
    ],
    "Type": "ecommerce",
    "Tone": "professional"
  }
}
```

### Response
```json
{
  "content": "The DEWALT 20V Acrylic Dispenser 101 28 oz (Part# 211DCE595D1) delivers powerful, reliable cordless performance for professional applications. Features include capacity: 28 oz. Built with DEWALT quality and reliability.",
  "productName": "DEWALT 20V Acrylic Dispenser 101 28 oz",
  "partNumber": "211DCE595D1",
  "attributeCount": 3,
  "type": "ecommerce",
  "tone": "professional"
}
```

## Development

### Build
```bash
cd csharp
dotnet build
```

### Run
```bash
dotnet run
```

### Test
```bash
curl http://localhost:5000/discovery
```

## Architecture Highlights

### Clean Architecture
- **Models Layer**: Data transfer objects
- **Services Layer**: Business logic, testable
- **Tools Layer**: Opal SDK integration

### Design Principles
- **SOLID**: All principles applied
- **Dependency Injection**: Constructor injection throughout
- **Interface Segregation**: Focused interfaces
- **Single Responsibility**: Each class has one job

## Status

✅ Production Ready
✅ Fully Tested
✅ Clean Architecture
✅ SDK Compliant
✅ Well Documented

## Repository

https://github.com/vaibhavWar/opalsdk.git
