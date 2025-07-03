# order-service

Order Service is an ASP.NET Core Web API for managing orders.

![Order Service Architecture Diagram](./order-service-architecture-diagram.avif)

## Requirements

- [Docker](https://www.docker.com/)

## Setup

You will need to configure the database connection string and the JWT domain and audience. You'll need to get your own domain and audience. You can get them from [Auth0](https://auth0.com/).

Once acquired you will need to set them as environment variables. These will be used both when running in Docker Compose or when running on the host machine:

```sh
export Jwt__Domain="dev-abc123.us.auth0.com"
export Jwt__Audience="https://orderservice/api"
```

1. Clone the project to your workspace.

    ```sh
    git clone <url> order-service
    cd order-service
    ```

2. Start the app using Docker Compose.

    ```sh
    docker compose -f compose.local.yaml up
    ```
