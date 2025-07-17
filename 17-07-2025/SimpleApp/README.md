# Simple .NET API App

A simple ASP.NET Core Web API application with Entity Framework, Swagger documentation, and Docker support for testing and deployment.

## Features

- **ASP.NET Core 8.0** Web API
- **Entity Framework Core** with SQL Server
- **Swagger/OpenAPI** documentation
- **Azure Key Vault** integration (optional)
- **Docker** support with multi-stage build
- **Docker Compose** with SQL Server
- **Clean Architecture** with DTOs, Services, and Interfaces
- **CRUD Operations** for User management
- **Health Check** endpoint

## API Endpoints

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `GET /api/users/health` - Health check

## Quick Start

### Local Development

1. **Prerequisites:**
   - .NET 8.0 SDK
   - SQL Server (LocalDB or Express)

2. **Run the application:**
   ```bash
   dotnet restore
   dotnet run
   ```

3. **Access Swagger UI:**
   - Open browser to `https://localhost:7000` or `http://localhost:5000`

### Docker Development

1. **Build and run with Docker Compose:**
   ```bash
   docker-compose up --build
   ```

2. **Access the application:**
   - API: `http://localhost:5000`
   - Swagger UI: `http://localhost:5000`

### Docker Only

1. **Build Docker image:**
   ```bash
   docker build -t simpleapp .
   ```

2. **Run container:**
   ```bash
   docker run -p 5000:8080 simpleapp
   ```

## Configuration

### Database Connection

- **Local:** Uses SQL Server with trusted connection
- **Docker:** Uses SQL Server container with username/password

### Azure Key Vault (Optional)

Set the `KeyVault:VaultUrl` in `appsettings.json` to enable Key Vault integration:

```json
{
  "KeyVault": {
    "VaultUrl": "https://your-keyvault.vault.azure.net/"
  }
}
```

## Project Structure

```
SimpleApp/
├── Controllers/        # API Controllers
├── Data/              # Entity Framework DbContext
├── DTOs/              # Data Transfer Objects
├── Interfaces/        # Service Interfaces
├── Models/            # Entity Models
├── Services/          # Business Logic Services
├── Dockerfile         # Docker configuration
├── docker-compose.yml # Docker Compose configuration
└── Program.cs         # Application entry point
```

## Testing the API

### Sample User Creation

```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test User",
    "email": "test@example.com",
    "phone": "1234567890"
  }'
```

### Health Check

```bash
curl http://localhost:5000/api/users/health
```

## Database

The application uses Entity Framework Code First approach with:
- **User** entity with validation
- **Automatic database creation** on startup
- **Seed data** for testing

## Docker Notes

- **Multi-stage build** for optimized image size
- **Non-root user** for security
- **Health checks** included
- **Volume persistence** for SQL Server data
- **Network isolation** with custom bridge network

## Troubleshooting

### Database Connection Issues
- Ensure SQL Server is running
- Check connection string in `appsettings.json`
- For Docker: Ensure containers are on same network

### Port Conflicts
- Change ports in `docker-compose.yml` if needed
- Default ports: 5000 (HTTP), 5001 (HTTPS), 1433 (SQL Server)

### Build Issues
- Run `dotnet clean` and `dotnet restore`
- Check .NET 8.0 SDK installation
- Ensure all NuGet packages are restored
