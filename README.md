# order-service

## Instructions

```sh
# Clone the repository
git clone <repo-url>
cd order-service

# Start the database (PostgreSQL) via Docker
docker compose up -d

# Apply EF Core migrations to create the schema
dotnet ef database update --project OrderService

# Run the ASP.NET Core Web API
dotnet run --project OrderService

# Build the Docker image
docker build -t orderservice -f OrderService/Dockerfile .
```
