# RMG Management

A comprehensive .NET 8.0 Web API for managing delivery challans, gate passes, packaging lists, and truck operations with dual database support (SQL Server/MySQL).

## 🚀 Project Setup

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or higher
- [SQL Server](https://www.microsoft.com/sql-server) or [MySQL](https://www.mysql.com/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) / [Visual Studio Code](https://code.visualstudio.com/) / [Rider](https://www.jetbrains.com/rider/)
- [Git](https://git-scm.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/saimum1/dotnetWebApi.git
   cd DotNetWbapi
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure environment variables**
   
   Create a `.env` file in the root directory:
   ```env
   # Database Selection
   USE_MYSQL=false
   
   # SQL Server Configuration (when USE_MYSQL=false)
   LOCAL_DB_SERVER=localhost
   LOCAL_DB_NAME=DotNetWbapiDB
   LOCAL_DB_USER=sa
   LOCAL_DB_PASSWORD=YourPassword
   
   # MySQL Configuration (when USE_MYSQL=true)
   SERVER_DB_CONNECTION=Server=localhost;Database=DotNetWbapiDB;User=root;Password=YourPassword;
   ```

4. **Apply database migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

   The API should be available at `http://localhost:5500`

6. **Log in application**
   ```bash
   for admin :-  username : admin
                 password : password

   for user :-  username :  user
                 password : password              

   ```

### NuGet Packages

The project uses the following packages:

| Package | Version | Purpose |
|---------|---------|---------|
| **ClosedXML** | 0.95.1 | Excel file generation and manipulation |
| **DotNetEnv** | 3.1.1 | Environment variable management |
| **EPPlus** | 8.2.1 | Advanced Excel operations |
| **Microsoft.AspNetCore.OpenApi** | 8.0.20 | OpenAPI/Swagger support |
| **Microsoft.Data.SqlClient** | 6.1.2 | SQL Server data provider |
| **Microsoft.EntityFrameworkCore.Design** | 9.0.9 | EF Core design-time tools |
| **Microsoft.EntityFrameworkCore.SqlServer** | 9.0.9 | SQL Server database provider |
| **Microsoft.Extensions.Configuration** | 9.0.9 | Configuration framework |
| **Microsoft.Extensions.Configuration.EnvironmentVariables** | 9.0.9 | Environment variable configuration |
| **Microsoft.Extensions.Configuration.Json** | 9.0.9 | JSON configuration support |
| **MySql.Data** | 9.4.0 | MySQL data provider |
| **Pomelo.EntityFrameworkCore.MySql** | 9.0.0 | MySQL EF Core provider |
| **QRCoder** | 1.6.0 | QR code generation |
| **Swashbuckle.AspNetCore** | 6.6.2 | Swagger/OpenAPI documentation |

## ✨ Features

### Core Functionality

- **Delivery Challan Management** - Create, track, and manage delivery challans with header information
- **Gate Pass System** - Generate and process gate passes for logistics operations
- **Packaging List Operations** - Organize and maintain packaging lists for shipments
- **Truck Management** - Monitor truck information and assignments
- **Declaration Settings** - Configure and manage declaration parameters
- **Template System** - Create and manage document templates for various operations
- **QR Code Generation** - Generate QR codes for tracking and identification
- **Excel Import/Export** - Comprehensive Excel file handling with ClosedXML and EPPlus

### Technical Features

- **Dual Database Support** - Seamlessly switch between SQL Server and MySQL
- **RESTful API Design** - Clean, standards-compliant endpoints
- **CORS Enabled** - Configured for cross-origin requests
- **Entity Framework Core** - Modern ORM with migrations support
- **Swagger Documentation** - Interactive API documentation
- **Environment-based Configuration** - Flexible configuration using .env files
- **JSON Serialization** - Handles circular references with proper formatting

## 📁 Project Structure

```
DotNetWbapi/
│
├── Controllers/                      # API Controllers
│   ├── DeclarationSettingController.cs
│   ├── DeliveryChallanControllerCreation.cs
│   ├── GatepassController.cs
│   ├── PackagingListController.cs
│   ├── TemplateController.cs
│   └── TrucksController.cs
│
├── Data/                             # Database Context
│   └── AppDBContext.cs               # Entity Framework DB Context
│
├── Dto/                              # Data Transfer Objects
│   ├── DeclarationSettingDto.cs
│   ├── DeliveryChallanHeaderCreationDto.cs
│   ├── GatepassHeaderDto.cs
│   ├── PackagingListDto.cs
│   ├── TemplateDtos.cs
│   └── TruckDto.cs
│
├── Migrations/                       # EF Core Migrations
│   ├── 20251009073732_Init.cs
│   ├── 20251016121740_AddCustomerTable.cs
│   └── AppDBContextModelSnapshot.cs
│
├── Models/                           # Domain Models
│   ├── DeclarationSetting.cs
│   ├── DeliveryChallanHeaderCreation.cs
│   ├── GatepassHeader.cs
│   ├── PackagingList.cs
│   ├── TemplateModel.cs
│   └── Truck.cs
│
├── Services/                         # Business Logic Layer
│   ├── DeclarationSettingService.cs
│   ├── DeliveryChallanServiceCreation.cs
│   ├── GatepassService.cs
│   ├── PackagingListService.cs
│   ├── TemplateService.cs
│   └── TruckService.cs
│
├── wwwroot/                          # Static Files
│   └── index.html
│
├── db/                               # Database Scripts (if any)
├── obj/                              # Build Output
├── Properties/                       # Project Properties
│
├── Program.cs                        # Application Entry Point
├── appsettings.json                  # Application Configuration
├── .env                              # Environment Variables (not in repo)
└── DotNetWbapi.csproj               # Project File
```

### Architecture Overview

**Controllers Layer**
- Handle HTTP requests and responses
- Route API endpoints
- Validate input data
- Return appropriate HTTP status codes

**Services Layer**
- Implement business logic
- Process data transformations
- Coordinate between controllers and data layer
- Handle complex operations

**Data Layer**
- `AppDBContext`: Entity Framework Core context
- Database connection management
- Entity configurations

**DTOs (Data Transfer Objects)**
- Define data contracts for API requests/responses
- Separate domain models from API models
- Ensure clean data flow

**Models**
- Represent database entities
- Define relationships and constraints
- Map to database tables

**Migrations**
- Track database schema changes
- Enable version control for database structure
- Support both SQL Server and MySQL

## 🔌 API Endpoints

The API provides the following endpoint groups:

| Controller | Base Route | Description |
|------------|-----------|-------------|
| GatepassController | `/api/gatepass` | Gate pass operations |
| DeliveryChallanController | `/api/deliverychallan` | Delivery challan management |
| PackagingListController | `/api/packaginglist` | Packaging list operations |
| TrucksController | `/api/trucks` | Truck information management |
| DeclarationSettingController | `/api/declarationsetting` | Declaration configurations |
| TemplateController | `/api/template` | Template management |

### Example Request

```bash
# Get all gate passes
curl -X GET "http://localhost:5000/api/gatepass" -H "accept: application/json"

# Create a new delivery challan
curl -X POST "http://localhost:5000/api/deliverychallan" \
  -H "Content-Type: application/json" \
  -d '{"field1": "value1", "field2": "value2"}'
```

## 🗄️ Database Configuration

### Switching Between Databases

**Use SQL Server** (Default):
```env
USE_MYSQL=false
LOCAL_DB_SERVER=localhost
LOCAL_DB_NAME=DotNetWbapiDB
LOCAL_DB_USER=sa
LOCAL_DB_PASSWORD=YourPassword
```

**Use MySQL**:
```env
USE_MYSQL=true
SERVER_DB_CONNECTION=Server=localhost;Database=DotNetWbapiDB;User=root;Password=YourPassword;
```

### Running Migrations

```bash
# Create a new migration
dotnet ef migrations add YourMigrationName

# Apply migrations to database
dotnet ef database update

# Rollback to a specific migration
dotnet ef database update PreviousMigrationName

# Remove last migration
dotnet ef migrations remove
```

## 🌐 CORS Configuration

CORS is enabled for the following origins:
- `http://127.0.0.1:5500`
- `http://127.0.0.1:5501`
- `https://fanciful-paprenjak-43fd9b.netlify.app`

To modify CORS settings, update the `Program.cs` file.

## 🛠️ Development

### Build the Project
```bash
dotnet build
```

### Run in Development Mode
```bash
dotnet run --environment Development
```

### Run Tests (if available)
```bash
dotnet test
```
