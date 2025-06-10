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

# Or, build and run the Docker image
docker build -t orderservice -f OrderService/Dockerfile .
docker run --rm --init -it -p 5293:5293 -e ConnectionStrings__OrderDb="<your-connection-string>" orderservice

# To connect to Postgres running in Docker Compose, use --network
docker run --rm --init -it -p 5293:5293 -e ConnectionStrings__OrderDb="Host=postgres;Port=5432;Database=orderdb;Username=postgres;Password=mypassword" --network=order-service_default  orderservice
```
