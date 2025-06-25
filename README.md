# Developer Evaluation Project

`READ CAREFULLY`

## Instructions
- The code must be versioned in a public Github repository and a link must be sent for evaluation once completed
- Upload this template to your repository and start working from it
- Read the instructions carefully and make sure all requirements are being addressed
- The repository must provide instructions on how to configure, execute and test the project
- Documentation and overall organization will also be taken into consideration

## How to Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (or use Docker)
- [MongoDB](https://www.mongodb.com/try/download/community) (or use Docker)
- [Docker](https://www.docker.com/) (optional, for database)

### Database Setup (Docker)
```sh
docker run --name postgres-dev -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=DeveloperEvaluation -p 5432:5432 -d postgres:15
docker run --name mongo-dev -e MONGO_INITDB_ROOT_USERNAME=developer -e MONGO_INITDB_ROOT_PASSWORD=ev@luAt10n -p 27017:27017 -d mongo:8.0
```

### Configuration
Edit `template/backend/src/Ambev.DeveloperEvaluation.WebApi/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=DeveloperEvaluation;Username=postgres;Password=postgres",
  "MongoDb": "mongodb://developer:ev@luAt10n@localhost:27017/DeveloperEvaluation"
},
"DatabaseProvider": "PostgreSQL" // or "MongoDB"
```

### Switching Database Provider
To use MongoDB instead of PostgreSQL, set:
```json
"DatabaseProvider": "MongoDB"
```

### Apply Migrations (PostgreSQL only)
```sh
cd template/backend
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

### Run the API
```sh
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```
Access: https://localhost:5001/swagger

## How to Test

### Unit Tests
```sh
dotnet test tests/Ambev.DeveloperEvaluation.Unit
```

### Integration Tests
```sh
dotnet test tests/Ambev.DeveloperEvaluation.Integration
```

### Code Coverage
```sh
# Windows
./src/coverage-report.bat
# Linux/Mac
./src/coverage-report.sh
```

## Example API Usage

### Create Sale
```sh
curl -X POST https://localhost:5001/api/sales \
  -H "Content-Type: application/json" \
  -d '{
    "saleDate": "2024-06-20T12:00:00Z",
    "customerId": "<customer-guid>",
    "customerName": "John Doe",
    "branchId": "<branch-guid>",
    "branchName": "Main Branch",
    "items": [
      { "productId": "<product-guid>", "productName": "Product A", "quantity": 5, "unitPrice": 10.0 }
    ]
  }'
```
**Response Example:**
```json
{
  "success": true,
  "data": {
    "id": "...",
    "saleNumber": "SALE-20240620120000-1234",
    "saleDate": "2024-06-20T12:00:00Z",
    "customerId": "...",
    "customerName": "John Doe",
    "totalAmount": 45.0,
    "branchId": "...",
    "branchName": "Main Branch",
    "items": [
      {
        "id": "...",
        "productId": "...",
        "productName": "Product A",
        "quantity": 5,
        "unitPrice": 10.0,
        "discount": 5.0,
        "totalAmount": 45.0
      }
    ]
  },
  "message": "Sale created successfully"
}
```

### Get Sales (paginated)
```sh
curl https://localhost:5001/api/sales?_page=1&_size=10
```
**Response Example:**
```json
{
  "success": true,
  "data": {
    "items": [ ... ],
    "pagination": { "page": 1, "size": 10, "totalCount": 1, "totalPages": 1 }
  },
  "message": "Sales retrieved successfully"
}
```

### Cancel Sale
```sh
curl -X DELETE https://localhost:5001/api/sales/<sale-guid> \
  -H "Content-Type: application/json" \
  -d '{ "reason": "Customer request" }'
```
**Response Example:**
```json
{
  "success": true,
  "data": {
    "id": "...",
    "saleNumber": "SALE-20240620120000-1234",
    "status": "Cancelled",
    "cancelledAt": "2024-06-20T13:00:00Z",
    "cancellationReason": "Customer request"
  },
  "message": "Sale cancelled successfully"
}
```

## API Endpoints Table
| Method | Endpoint                | Description                |
|--------|-------------------------|----------------------------|
| POST   | /api/sales              | Create a new sale          |
| GET    | /api/sales              | List sales (paginated)     |
| GET    | /api/sales/{id}         | Get sale by id             |
| PUT    | /api/sales/{id}         | Update sale                |
| DELETE | /api/sales/{id}         | Cancel sale                |

## Test Data & Mocks
- Unit tests use [Bogus (Faker)](https://github.com/bchavez/Bogus) for data generation.
- Unit tests use [NSubstitute](https://github.com/nsubstitute/NSubstitute) for mocking dependencies.

## Messaging (Rebus)
- Domain events (SaleCreated, SaleModified, SaleCancelled, ItemCancelled) are published using [Rebus](https://github.com/rebus-org/Rebus) (in-memory transport).
- No external message broker is required for local development.

## Performance
- All data access is async/await.
- EF Core and MongoDB queries are optimized for pagination and filtering.

## Project Structure
See [.doc/project-structure.md](.doc/project-structure.md)

## Tech Stack & Frameworks
See [.doc/tech-stack.md](.doc/tech-stack.md) and [.doc/frameworks.md](.doc/frameworks.md)

## Use Case
**You are a developer on the DeveloperStore team. Now we need to implement the API prototypes.**

As we work with `DDD`, to reference entities from other domains, we use the `External Identities` pattern with denormalization of entity descriptions.

Therefore, you will write an API (complete CRUD) that handles sales records. The API needs to be able to inform:

* Sale number
* Date when the sale was made
* Customer
* Total sale amount
* Branch where the sale was made
* Products
* Quantities
* Unit prices
* Discounts
* Total amount for each item
* Cancelled/Not Cancelled

It's not mandatory, but it would be a differential to build code for publishing events of:
* SaleCreated
* SaleModified
* SaleCancelled
* ItemCancelled

If you write the code, **it's not required** to actually publish to any Message Broker. You can log a message in the application log or however you find most convenient.

### Business Rules

* Purchases above 4 identical items have a 10% discount
* Purchases between 10 and 20 identical items have a 20% discount
* It's not possible to sell above 20 identical items
* Purchases below 4 items cannot have a discount

These business rules define quantity-based discounting tiers and limitations:

1. Discount Tiers:
   - 4+ items: 10% discount
   - 10-20 items: 20% discount

2. Restrictions:
   - Maximum limit: 20 items per product
   - No discounts allowed for quantities below 4 items

## Overview
This section provides a high-level overview of the project and the various skills and competencies it aims to assess for developer candidates.

See [Overview](/.doc/overview.md)
