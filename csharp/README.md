# Product Description Generator - C# Implementation

## Clean Architecture Overview

This project follows clean code principles with clear separation of concerns, dependency injection, and SOLID principles.

## Project Structure

```
csharp/
├── Program.cs                          # Application entry point and configuration
├── Models/
│   └── ProductDescriptionParameters.cs # Parameter models (DTOs)
├── Services/
│   ├── IDescriptionGenerationService.cs    # Service interface
│   └── DescriptionGenerationService.cs     # Service implementation
└── Tools/
    └── ProductDescriptionGeneratorTool.cs  # Opal tool implementation
```

## Architecture Layers

### 1. **Program.cs** - Application Configuration
- **Responsibility**: Application startup and dependency injection
- **Key Features**:
  - Service registration
  - CORS configuration
  - Opal SDK integration
  - Endpoint mapping

```csharp
builder.Services.AddScoped<IDescriptionGenerationService, DescriptionGenerationService>();
builder.Services.AddOpalToolService();
builder.Services.AddOpalTool<ProductDescriptionGeneratorTool>();
```

### 2. **Models Layer** - Data Transfer Objects
- **Responsibility**: Define data structures for input/output
- **Key Features**:
  - Data annotations for validation
  - Clean parameter definitions
  - Immutable data contracts

**ProductDescriptionParameters.cs**:
- Contains all input parameters
- Uses `[Required]` and `[Description]` attributes
- Strongly typed

### 3. **Services Layer** - Business Logic
- **Responsibility**: Core business logic for description generation
- **Key Features**:
  - Interface-based design (IDescriptionGenerationService)
  - Single Responsibility Principle
  - Testable and reusable

**IDescriptionGenerationService**:
- Interface defining the contract
- Enables dependency injection
- Facilitates unit testing

**DescriptionGenerationService**:
- Implements business logic
- Parses attributes
- Generates AI-like descriptions
- Handles feature prioritization
- Formats output

### 4. **Tools Layer** - Opal Integration
- **Responsibility**: Bridge between Opal SDK and business logic
- **Key Features**:
  - `[OpalTool]` attribute decoration
  - Parameter validation
  - Service consumption via DI
  - Response formatting

**ProductDescriptionGeneratorTool.cs**:
- Receives parameters from Opal
- Validates input
- Calls service layer
- Returns structured response

## Design Principles Applied

### 1. **Separation of Concerns**
- Each class has a single, well-defined responsibility
- Business logic separated from API/tool concerns
- Models separated from services and tools

### 2. **Dependency Injection**
- Services registered in DI container
- Constructor injection used throughout
- Loose coupling between components

### 3. **SOLID Principles**

**Single Responsibility Principle (SRP)**:
- `DescriptionGenerationService`: Only handles description generation
- `ProductDescriptionGeneratorTool`: Only handles Opal integration
- `ProductDescriptionParameters`: Only defines data structure

**Open/Closed Principle (OCP)**:
- Service interface allows extension without modification
- New features can be added by implementing new services

**Liskov Substitution Principle (LSP)**:
- `DescriptionGenerationService` can be replaced with any implementation of `IDescriptionGenerationService`

**Interface Segregation Principle (ISP)**:
- `IDescriptionGenerationService` has a focused, minimal interface

**Dependency Inversion Principle (DIP)**:
- Tool depends on `IDescriptionGenerationService` interface, not concrete implementation
- High-level policy doesn't depend on low-level details

### 4. **Clean Code Practices**
- Descriptive naming
- Small, focused methods
- Constants for magic numbers
- Private helper methods
- XML documentation comments

## Service Layer Details

### Method Breakdown

**ParseAttributes**:
- Extracts key-value pairs from attribute strings
- Returns both map and list for flexible access

**BuildDescription**:
- Orchestrates description generation
- Calls specialized methods for each section

**GetOpeningStatement**:
- Analyzes product type (cordless, powered, etc.)
- Returns contextual opening sentence

**GetFeatureHighlights**:
- Filters and prioritizes attributes
- Formats features with context labels
- Limits to top 3 features

**GetBrandStatement** & **GetWarrantyStatement**:
- Extract specific information
- Format appropriately

**TrimToMaxLength**:
- Ensures 500-character limit
- Adds ellipsis if needed

## Testing

### Unit Testing Strategy
```csharp
// Mock the service
var mockService = new Mock<IDescriptionGenerationService>();
mockService
    .Setup(s => s.GenerateProductDescription(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
    .Returns("Test description");

// Inject into tool
var tool = new ProductDescriptionGeneratorTool(mockService.Object);
```

### Integration Testing
```bash
# Test discovery
curl http://localhost:5000/discovery

# Test execution
curl -X POST http://localhost:5000/tools/product-description-generator \
  -H "Content-Type: application/json" \
  -d '{"parameters": {...}}'
```

## Building and Running

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Run with Hot Reload
```bash
dotnet watch run
```

## API Endpoints

### Discovery (Auto-generated by Opal SDK)
- **URL**: `GET /discovery`
- **Response**: Tool definition with parameters

### Execute (Auto-generated by Opal SDK)
- **URL**: `POST /tools/product-description-generator`
- **Request Body**:
```json
{
  "parameters": {
    "ProductName": "Product Name",
    "PartNumber": "12345",
    "Attributes": ["Brand: XYZ", "Voltage: 20V"]
  }
}
```
- **Response**:
```json
{
  "content": "Generated description...",
  "productName": "Product Name",
  "partNumber": "12345",
  "attributeCount": 2
}
```

## Benefits of This Architecture

1. **Maintainability**: Easy to understand and modify
2. **Testability**: Each component can be tested in isolation
3. **Scalability**: Easy to add new features or services
4. **Reusability**: Services can be used in multiple tools
5. **Flexibility**: Easy to swap implementations
6. **Clarity**: Clear structure makes onboarding easier

## Dependencies

- **Optimizely.Opal.Tools** v0.4.0 - Opal SDK integration
- **Microsoft.AspNetCore.OpenApi** v8.0.0 - OpenAPI support
- **Swashbuckle.AspNetCore** v6.8.0 - Swagger documentation

## Author

Product Description Generator with Optimizely Opal SDK integration.

