# E-Commerce Microservices

A containerized e-commerce backend built with **ASP.NET Core Web API** and a microservices architecture. The solution separates user management, product catalog, and order processing into independently developed services, each with its own database and responsibilities.

> **Project status:** Learning / portfolio project. Some production concerns—particularly secure authentication, reliable message processing, and deployment configuration—still need further work.

## Architecture

The system consists of three backend services and an API Gateway:

| Component | Responsibility | Data store |
|---|---|---|
| **UsersService** | User registration, login, and user lookup | PostgreSQL |
| **ProductsService** | Product CRUD and product events | MySQL |
| **OrdersService** | Order creation, retrieval, updates, and deletion | MongoDB |
| **API Gateway** | Routes external requests to backend services | — |

Supporting infrastructure:

- **Ocelot** – API Gateway and request routing.
- **RabbitMQ** – asynchronous product events.
- **Redis** – distributed caching for user and product lookups.
- **Polly** – retry, circuit-breaker, and timeout policies for calls to UsersService.
- **Docker Compose** – local multi-container environment.

### High-level request flow

```text
                         Client
                           |
                           v
                    +--------------+
                    | API Gateway  |
                    |   Ocelot     |
                    +--------------+
                      /     |     \
                     v      v      v
                  Users  Products Orders
                    |       |       |
                PostgreSQL MySQL  MongoDB
                            |
                            | Product events
                            v
                       RabbitMQ
                            |
                            v
                         Orders

                 Redis <---- service lookups
```

OrdersService communicates with UsersService and ProductsService through HTTP clients routed via the API Gateway. Product update and deletion events are published through RabbitMQ and consumed by OrdersService.

## Technologies

- **Language:** C#
- **Framework:** .NET 9 / ASP.NET Core Web API
- **Architecture:** Microservices, layered separation
- **Databases:** PostgreSQL, MySQL, MongoDB
- **Data access:** Dapper, Entity Framework Core, MongoDB.Driver
- **Messaging:** RabbitMQ
- **Caching:** Redis
- **API Gateway:** Ocelot
- **Resilience:** Polly
- **Validation:** FluentValidation
- **Object mapping:** AutoMapper
- **Containers:** Docker, Docker Compose

## Main features

### UsersService

- User registration and login endpoints.
- Retrieve a user by ID.
- PostgreSQL data access using Dapper.
- Request validation with FluentValidation.

### ProductsService

- Create, retrieve, update, and delete products.
- MySQL persistence using Entity Framework Core.
- Request validation and DTO mapping.
- Publish product events to RabbitMQ when a product is deleted or its name changes.

### OrdersService

- Create, retrieve, update, and delete orders.
- Store orders in MongoDB, including embedded order items.
- Validate order and order-item requests.
- Retrieve user and product information through HTTP clients.
- Cache user and product lookup responses in Redis.
- Consume product update and deletion events from RabbitMQ.
- Apply Polly resilience policies to UsersService HTTP requests.

### API Gateway

- Route client requests to UsersService, ProductsService, and OrdersService.
- Centralize external service entry points through Ocelot.
- Configure gateway-level QoS, retry, rate-limiting, and caching options for selected routes.

## Repository structure

```text
E-Commerce-Microservices/
├── UsersService/
│   ├── eCommerce.API/
│   ├── eCommerce.Core/
│   └── eCommerce.Infrastructure/
│
├── ProductsService/
│   ├── ProductsMicroService.API/
│   ├── BusinessLogicLayer/
│   └── DataAccessLayer/
│
└── OrdersService/
    ├── ApiGateway/
    ├── OrdersMicroService.API/
    ├── BusinessLogicLayer/
    ├── DataAccessLayer/
    └── docker-compose.yml
```

Each service has its own solution and project structure. The exact folders may contain additional configuration, migration, and infrastructure files.

## Running the project locally

### Prerequisites

- Docker Desktop with the Docker Compose plugin.
- .NET 9 SDK (for local development outside containers).
- Git.

### 1. Clone the repository

```bash
git clone https://github.com/OmarOsama4/E-Commerce-Microservices.git
cd E-Commerce-Microservices
```

### 2. Build the UsersService and ProductsService images

The current Compose file builds OrdersService and the API Gateway, but references pre-built images for UsersService and ProductsService. Build those images from the repository root:

```bash
docker build -t users-microservice:latest \
  -f UsersService/eCommerce.API/Dockerfile UsersService

docker build -t products-microservice:latest \
  -f ProductsService/ProductsMicroService.API/Dockerfile ProductsService
```

### 3. Review the Compose configuration

The Compose file is located at `OrdersService/docker-compose.yml`. Before running it on another machine:

- Replace machine-specific Windows volume paths with paths relative to the Compose file (for example, `./mysql-init`).
- Ensure the referenced database initialization directories and scripts exist.
- Move development credentials into environment variables or an untracked `.env` file.
- Do not use the sample development credentials in a production environment.

### 4. Start the containers

```bash
cd OrdersService
docker compose up --build
```

To stop the environment:

```bash
docker compose down
```

To also remove the containers' named/anonymous volumes, review the impact first and then use:

```bash
docker compose down --volumes
```

> The current Compose configuration uses host-specific volume paths and development credentials, so the configuration review in step 3 is required before expecting it to run unchanged on a different machine.

## Local service endpoints

The current Compose configuration maps these host ports:

| Component | Host address |
|---|---|
| API Gateway | `http://localhost:4501` |
| OrdersService | `http://localhost:7100` |
| ProductsService | `http://localhost:6000` |
| UsersService | `http://localhost:5000` |
| RabbitMQ Management UI | `http://localhost:15672` |
| MongoDB | `localhost:27017` |
| MySQL | `localhost:3306` |
| PostgreSQL | `localhost:5433` |
| Redis | `localhost:6379` |

The API Gateway exposes routes under `/gateway`, including:

| Method | Gateway route | Purpose |
|---|---|---|
| GET | `/gateway/orders` | Retrieve orders |
| POST | `/gateway/orders` | Create an order |
| PUT | `/gateway/orders` | Update an order |
| GET | `/gateway/orders/search/orderid/{orderID}` | Retrieve an order by ID |
| GET | `/gateway/orders/search/orderdate/{orderDate}` | Search orders by date |
| GET | `/gateway/orders/search/productid/{productID}` | Search orders by product |
| POST | `/gateway/auth/register` | Register a user |
| POST | `/gateway/auth/login` | Log in a user |

See `OrdersService/ApiGateway/ocelot.json` for the complete gateway route configuration.

## Configuration and security notes

- Connection strings and service addresses are configured through application configuration and environment variables.
- The repository currently contains development-only credentials in its Compose configuration. Replace them before sharing a deployment or using the system outside a local learning environment.
- The current UsersService implementation still needs secure password hashing and real JWT generation/validation before it can be considered suitable for real user accounts.
- Use HTTPS, proper authorization policies, secret management, and production-specific settings before deployment.

## Future improvements

- Implement secure password hashing and JWT authentication/authorization.
- Add automated unit and integration tests.
- Improve RabbitMQ reliability with manual acknowledgements, retry handling, and dead-letter queues.
- Consider the Transactional Outbox Pattern for reliable database-to-message publishing.
- Improve cache invalidation and consistency.
- Add health checks, structured logging, and distributed tracing.
- Make Docker Compose configuration portable and separate development from production settings.
- Add a CI/CD workflow with GitHub Actions.

## Acknowledgment

This project was originally developed while following the **E-Commerce Microservices** course by [Web Academy by Harsha Vardhan](https://www.udemy.com/user/harsha-vardhan-28/) on Udemy.

I use this repository as a learning project to study the implementation of microservices concepts with .NET, understand the underlying architecture, and continue improving the code through independent exploration and enhancements.

## Author

**Omar Osama**

GitHub: [@OmarOsama4](https://github.com/OmarOsama4)

---

This project is developed for learning and demonstrating backend development and microservices concepts with .NET.
