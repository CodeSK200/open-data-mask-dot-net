# OpenDataMask .NET Console Application

This repository contains a standalone .NET 10 console application for masking MongoDB documents using an XML-driven configuration.

## Structure

- `src/OpenDataMask.Console` — console application source code
- `tests/OpenDataMask.Console.Tests` — xUnit unit tests
- `Dockerfile` — containerization support
- `azuredeploy.bicep` — Azure deployment template

## How It Works

```
┌────────────────────────────────────────────────────────────────────────┐
│           OpenDataMask for .net Data Flow Architecture                 │
└────────────────────────────────────────────────────────────────────────┘

  INPUT SOURCES
  ─────────────
   
  ┌──────────────────────┐         ┌──────────────────────┐
  │  XML Config File     │         │  Source MongoDB      │
  │  ├─ Field Masks      │         │  ├─ Collections      │
  │  ├─ Masking Rules    │         │  ├─ Documents        │
  │  └─ Data Types       │         │  └─ Field Values     │
  └──────────┬───────────┘         └──────────┬───────────┘
             │                                 │
             ▼                                 ▼
  ┌──────────────────────────────────────────────────────┐
  │         Configuration Parser & Repository            │
  │  ├─ XmlMaskingConfigReader                          │
  │  └─ MongoRepository                                 │
  └──────────┬───────────────────────────────────────────┘
             │
             ▼
  ┌────────────────────────────────────────────────────────────┐
  │              MaskingEngine (Orchestrator)                  │
  │  ├─ Iterates through collections                          │
  │  ├─ Fetches documents batch by batch                      │
  │  └─ Instructs MaskService for each document               │
  └──────────┬───────────────────────────────────────────────┘
             │
             ▼
  ┌────────────────────────────────────────────────────────────┐
  │             MaskService (Processing Engine)                │
  │  ├─ Type-aware masking                                    │
  │  │  ├─ Strings → "MASKED"                               │
  │  │  ├─ Integers → 0                                     │
  │  │  ├─ Dates → null or default                          │
  │  │  └─ Complex types → recursive masking                │
  │  ├─ Nested document handling                             │
  │  └─ Field-level masking rules application               │
  └──────────┬───────────────────────────────────────────────┘
             │
             ▼
  ┌────────────────────────────────────────────────────────────┐
  │          Masked Documents Ready for Storage                │
  │  ├─ All sensitive fields masked                           │
  │  ├─ Data structure preserved                              │
  │  └─ Integrity maintained                                  │
  └──────────┬───────────────────────────────────────────────┘
             │
             ▼
  ┌──────────────────────┐
  │Destination MongoDB   │
  │  ├─ Collections      │
  │  ├─ Masked Documents │
  │  └─ Field Values     │
  └──────────────────────┘
```

**Component Details:**

- **XmlMaskingConfigReader** — Parses XML configuration into MaskingConfig record
- **MongoRepository** — Data access layer connecting to source/destination MongoDB
- **MaskingEngine** — Orchestrates the masking workflow, iterates collections and documents
- **MaskService** — Applies type-aware masking rules to document fields, handles nested objects
- **Input** — XML config file defining which fields to mask and how
- **Output** — Destination MongoDB with fully masked sensitive data

## Configuration Example

The `masking-config.xml` file defines which MongoDB collections and fields should be masked:

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

**How it works:**
- Each top-level element represents a MongoDB collection (e.g., `<Customers>`, `<Orders>`)
- Child elements list the fields within each collection to mask (e.g., `<Name>`, `<Email>`)
- When the application runs, it will mask these specified fields with type-appropriate values (strings → "MASKED", numbers → 0, etc.)
- All other fields not listed in the configuration will remain unchanged in the destination database

## Run locally

1. Set the required configuration in `src/OpenDataMask.Console/appsettings.json`.
2. Run the application from the `src/OpenDataMask.Console` folder:
   ```bash
   dotnet run --project src/OpenDataMask.Console/OpenDataMask.Console.csproj -- --input-file masking-config.xml
   ```

## Build

```bash
cd C:\Users\SuhasKrishnamurthy\Work\Learn\open-data-mask-dot-net
dotnet build OpenDataMask.sln
```

## Tests

```bash
dotnet test OpenDataMask.sln
```

## Docker

```bash
docker build -t opendatamask-console .
```

## Azure

Use `azuredeploy.bicep` to deploy the application to Azure Container Instances with Key Vault integration.
