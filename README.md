# order-service

Order Service is an ASP.NET Core Web API for managing orders.

## Requirements

- Docker

or

- .NET SDK 8
- PostgreSQL

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

## Configuration

You need to configure the database connection string and the JWT issuer domain and audience. You can do this two ways:

1) Using the .NET Secret Manager:

```sh
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:OrderDb" "Host=postgres;Port=5432;Database=orderdb;Username=postgres;Password=mypassword"
dotnet user-secrets set "Jwt:Domain" "dev-abc123.us.auth0.com"
dotnet user-secrets set "Jwt:Audience" "https://orderservice/api"
```

2) By setting environment variables:

```sh
export ConnectionStrings__OrderDb="Host=postgres;Port=5432;Database=orderdb;Username=postgres;Password=mypassword"
export Jwt__Domain="dev-abc123.us.auth0.com"
export JwtF__Audience="https://orderservice/api"
```
