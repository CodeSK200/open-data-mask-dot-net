## Instructions to convert the main codebase to a .NET application

### Related Guidelines & Resources

Before starting implementation, review these documents:

- **[DDD Systems & .NET Guidelines](https://github.com/github/awesome-copilot/blob/main/instructions/dotnet-architecture-good-practices.instructions.md)** — Domain-Driven Design, SOLID principles, architecture patterns, and .NET best practices
- **[XUnit Testing Best Practices](https://github.com/github/awesome-copilot/tree/main/plugins/csharp-dotnet-development/skills/csharp-xunit)** — Unit testing strategies, test structure, data-driven tests, and naming conventions
- **[.NET/C# Best Practices](https://github.com/github/awesome-copilot/tree/main/skills/dotnet-best-practices)** — Code standards, documentation, design patterns, dependency injection, async/await, and configuration
---

### Overview

This tool will:
- Read MongoDB collection names and property names from an input XML file
- Connect to the source MongoDB
- Obfuscate or mask the specified document properties
- Connect to the destination MongoDB
- Upsert the masked data into the destination

### Scope

- Build a standalone .NET 10 console application
- Implement a clean layer separation: Application / Domain / Infrastructure
- Use dependency injection, async programming, and modern C# features
- Reuse the existing masking business rules from the current codebase
- Provide unit and integration tests using xUnit
- Support Docker containerization
- Support Azure deployment with Bicep templates
- Support Azure Key Vault for connection string retrieval when deployed in Azure

### Input XML

The console app should accept an XML file describing collections and properties to mask.

Example:
```xml
<MaskingConfig>
  <Customers>
    <Name />
    <Email />
  </Customers>
  <Orders>
    <CreditCardNumber />
    <ShippingAddress />
  </Orders>
</MaskingConfig>
```

Rules:
- First-level nodes represent collection names
- Child nodes represent property names to mask
- The app should validate the XML structure and report errors clearly
- The XML parser should support simple element-based property names

### Configuration

The app should load configuration from:
- `appsettings.json` / `appsettings.Development.json` locally
- Azure Key Vault in Azure deployments

Required settings:
- Source MongoDB connection string
- Destination MongoDB connection string
- Optional: database names, retry settings, logging level

When deployed to Azure:
- Use Azure Key Vault for MongoDB connection strings
- Prefer managed identity / Azure SDK Key Vault provider
- Keep secrets out of source control

### Business workflow

1. Read the XML masking configuration
2. Load configuration and secrets
3. Connect to source MongoDB
4. Connect to destination MongoDB
5. For each collection in the XML:
   - Read documents from source
   - Mask the specified properties
   - Upsert masked documents into destination
6. Ensure collections not included in the XML are also copied to the destination unchanged
7. Preserve collection structure and data shape as much as possible

### Masking behavior

- Use the masking logic or template from the existing main codebase
- Mask only the fields specified in the XML
- Preserve other fields as-is
- Ensure the output collection is an exact replica except for masked values
- Support a consistent, testable masking strategy

### Architecture and quality

- Follow SOLID and DDD-friendly design
- Do not expose any personally identifiable information anywhere including the logs
- Keep responsibilities separated:
  - Domain/service layer for masking behavior
  - Infrastructure layer for MongoDB and configuration
  - Application/orchestration layer for workflow
- Use abstractions and interfaces
- Implement async MongoDB operations (`async`/`await`)
- Validate inputs before processing
- Log meaningful progress, warnings, and and errors

### Testing

- Use xUnit
- Aim for at least 90% coverage
- Test categories:
  - XML parsing
  - Masking logic
  - MongoDB repository interactions
  - Workflow orchestration
- Use naming convention: `MethodName_Condition_ExpectedResult()`

### Deployment

- Provide a Dockerfile for containerization
- Provide Azure Bicep templates for deployment
- Include:
  - App service / container app definition
  - Key Vault integration
  - Managed identity or service principal access
- Ensure the app supports both local and Azure execution modes

### Additional requirements

- Clear error handling and retry behavior for MongoDB connectivity
- Support command-line arguments for:
  - `--input-file`
  - `--source-connection`
  - `--destination-connection`
  - `--dry-run` or `--validate-only`
- Preserve and copy unchanged collections to destination


    