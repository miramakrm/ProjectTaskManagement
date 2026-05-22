# Project Task Management API

A RESTful API for managing projects and tasks, built with ASP.NET Core and Clean Architecture.

## Tech Stack
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- xUnit + Moq + FluentAssertions

## Architecture
ProjectTaskManagement/
├── Domain          → Entities, Enums
├── Application     → DTOs, Interfaces, Common
├── Infrastructure  → DbContext, Identity, JWT
└── API             → Controllers, Program.cs

## Setup

1. Clone the repository
```bash
git clone https://github.com/miramakrm/ProjectTaskManagement.git
```

2. Update `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ProjectTaskManagement;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JWT": {
    "Key": "YourSuperSecretKeyHereMustBeLong",
    "Issuer": "ProjectTaskManagement",
    "Audience": "ProjectTaskManagement"
  }
}
```

3. Run migrations
```bash
dotnet ef database update
```

4. Run the project
```bash
dotnet run
```

5. Open Swagger
https://localhost:7231/swagger

## API Endpoints

### Auth
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/Auth/register | Register new user |
| POST | /api/Auth/login | Login and get JWT token |

### Projects
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/Projects | Create project |
| GET | /api/Projects | Get all projects |
| GET | /api/Projects/{id} | Get project by id |
| PUT | /api/Projects/{id} | Update project |
| DELETE | /api/Projects/{id} | Delete project |

### Tasks
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/Tasks | Create task |
| GET | /api/Tasks/project/{projectId} | Get tasks by project |
| PUT | /api/Tasks/{id}/status | Update task status |
| DELETE | /api/Tasks/{id} | Delete task |

## Running Tests
```bash
dotnet test
```

## Response Format
```json
{
  "success": true,
  "message": "Success",
  "data": {}
}
```
