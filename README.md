# Order Management System

Sistem complet de gestionare comenzi implementat în **ASP.NET Core** cu aplicare a principiilor **SOLID** și conceptelor **OOP**.

## Quick Start

### Cerințe
- .NET 8.0+
- SQL Server (în docker)
- Visual Studio Code sau Visual Studio

### Setup

1. **Pornire SQL Server Docker**
```bash
docker run -e "ACCEPT_EULA=Y" \
           -e "MSSQL_SA_PASSWORD=YourSAPassword123!" \
           -p 1433:1433 \
           -d mcr.microsoft.com/mssql/server:latest
```

2. **Restaurare pachete**
```bash
cd src
dotnet restore
```

3. **Migrații (când sunt implementate)**
```bash
dotnet ef migrations add Initial
dotnet ef database update
```

4. **Pornire API**
```bash
dotnet run
```

5. **Swagger UI**
```
https://localhost:5001/swagger/index.html
```

## Structură Proiect

### Layers Arhitectură

```
┌─────────────────────────────┐
│     Controllers             │ REST API endpoints
├─────────────────────────────┤
│     Services (DIP)          │ Business logic (SRP)
├─────────────────────────────┤
│     Repositories (DIP)      │ Data access layer
├─────────────────────────────┤
│     Models + DbContext      │ Database schema
├─────────────────────────────┤
│     SQL Server              │ Persistence
└─────────────────────────────┘
```

### Principii Implementate

| Principiu | Implementare |
|-----------|--------------|
| **SRP** | Fiecare serviciu = 1 responsabilitate |
| **OCP** | Generic Repository extensibil |
| **LSP** | Subclasele pot înlocui clase de bază |
| **ISP** | Interfețe mici și specifice |
| **DIP** | Services depind de interfețe |

## API Endpoints

### Orders
| Method | Endpoint | Descriere |
|--------|----------|-----------|
| POST | `/api/orders` | Creare comandă |
| GET | `/api/orders/{id}` | Obținere comandă |
| GET | `/api/orders/customer/{customerId}` | Comenzi client |
| PUT | `/api/orders/{id}/status` | Update status |
| DELETE | `/api/orders/{id}` | Anulare comandă |

### Customers
| Method | Endpoint | Descriere |
|--------|----------|-----------|
| POST | `/api/customers` | Creare client |
| GET | `/api/customers/{id}` | Obținere client |
| PUT | `/api/customers/{id}` | Update client |
| DELETE | `/api/customers/{id}` | Ștergere client |
| GET | `/api/customers/{id}/orders` | Comenzi client |

### Products
| Method | Endpoint | Descriere |
|--------|----------|-----------|
| GET | `/api/products` | Lista produse |
| GET | `/api/products/{id}` | Detalii produs |
| GET | `/api/products/category/{category}` | Produse pe categorie |
| GET | `/api/products/search` | Căutare |
| POST | `/api/products` | Creare produs |
| PUT | `/api/products/{id}` | Update produs |

### Payments
| Method | Endpoint | Descriere |
|--------|----------|-----------|
| POST | `/api/payments` | Procesare plată |
| GET | `/api/payments/{id}` | Status plată |
| POST | `/api/payments/{id}/refund` | Rambursare |

### Inventory
| Method | Endpoint | Descriere |
|--------|----------|-----------|
| GET | `/api/inventory/product/{productId}` | Stoc disponibil |
| POST | `/api/inventory/reserve` | Rezervare stoc |
| POST | `/api/inventory/release` | Eliberare rezervare |
| PUT | `/api/inventory/update` | Update stoc |

### Discounts
| Method | Endpoint | Descriere |
|--------|----------|-----------|
| GET | `/api/discounts/validate/{code}` | Validare cod |
| POST | `/api/discounts/apply` | Aplicare discount |
| GET | `/api/discounts/{code}/price` | Preț cu discount |

## Documentație

- [IMPLEMENTATION.md](docs/IMPLEMENTATION.md) - Structura și endpoints detaliate
- [SOLID_Principles.md](docs/SOLID_Principles.md) - Explicarea principiilor SOLID
- [LAB1_INSTRUCTIONS.md](LAB1_INSTRUCTIONS.md) - Instrucțiuni laborator

## Status Laborator 1

✅ **Completat:**
- Structura completă OOP cu 11 entități
- Implementare interfețe (repositories + services)
- Dependency Injection configurată
- REST API controllers
- DbContext și migrații gata
- Documentație SOLID principles

⏳ **Următorul Pas:**
- Implementare metode (fill TODO comments)
- Unit testing
- Frontend React/Blazor

## Autor

Laborator 1 - Principii SOLID și OOP  
Tema: Sistem de Gestionare Comenzi
