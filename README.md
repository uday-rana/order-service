# order-service

Order Service is an ASP.NET Core Web API for managing orders.

## Requirements

- [Docker](https://www.docker.com/)
- [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Setup

You will need to configure the database connection string and the JWT domain and audience. You'll need to get your own domain and audience. You can get them from [Auth0](https://auth0.com/).

You can do this in two ways:

- Using the .NET Secret Manager:

    ```sh
    dotnet user-secrets init
    dotnet user-secrets set "ConnectionStrings:OrderDb" "Host=postgres;Port=5432;Database=orderdb;Username=postgres;Password=mypassword"
    dotnet user-secrets set "Jwt:Domain" "dev-abc123.us.auth0.com"
    dotnet user-secrets set "Jwt:Audience" "https://orderservice/api"
    ```

- By setting environment variables:

    ```sh
    export ConnectionStrings__OrderDb="Host=postgres;Port=5432;Database=orderdb;Username=postgres;Password=mypassword"
    export Jwt__Domain="dev-abc123.us.auth0.com"
    export Jwt__Audience="https://orderservice/api"
    ```



1. Clone the project to your workspace.

    ```sh
    git clone <url> order-service
    cd order-service/OrderService
    ```

2. Start the database (PostgreSQL) with Docker

    ```sh
    docker compose up -d
    ```

3. Apply EF Core migrations to create the schema

    ```sh
    dotnet ef database update
    ```

4. Start the web API

    ```sh
    dotnet run
    
    # Or, build and run the Docker image
    docker build -t orderservice -f OrderService/Dockerfile .
    docker run --rm --init -it -p 5293:5293 -e ConnectionStrings__OrderDb="<your-connection-string>" -e Jwt__Domain="dev-abc123.us.auth0.com" -e Jwt__Audience="https://orderservice/api" orderservice

    # To connect to Postgres running in Docker Compose, use --network
    docker run --rm --init -it -p 5293:5293 -e ConnectionStrings__OrderDb="Host=postgres;Port=5432;Database=orderdb;Username=postgres;Password=mypassword" -e Jwt__Domain="dev-abc123.us.auth0.com" -e Jwt__Audience="https://orderservice/api" --network=order-service_default  orderservice
    ```
