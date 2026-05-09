# E-Commerce App

Angular 21 + .NET 10 + PostgreSQL

## Tech Stack
- **Frontend**: Angular 21, TailwindCSS
- **Backend**: .NET 10 Web API, Clean Architecture
- **Database**: PostgreSQL + Redis
- **Payments**: BOG Pay, TBC Pay

## დაყენება

### Prerequisites
- .NET 10 SDK
- Node.js 20+
- Docker Desktop
- Angular CLI

### Backend
cd backend
cp appsettings.Example.json appsettings.json
შეავსე appsettings.json შენი მონაცემებით

docker compose up -d

dotnet ef database update --project ECommerce.Infrastructure --startup-project ECommerce.API

cd ECommerce.API
dotnet watch run --launch-profile http

### Frontend
cd frontend/ecommerce-frontend
npm install
ng serve

## URLs
- Frontend: http://localhost:4200
- Backend API: http://localhost:5099
- Swagger: http://localhost:5099/swagger
- Admin: admin@eshop.ge / Admin123!# E-Commerce App
