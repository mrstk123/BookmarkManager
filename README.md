# 📚 BookmarkManager

<div align="center">

**A modern, full-stack bookmark management application built with Clean Architecture.**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-21-DD0031?logo=angular&logoColor=white)](https://angular.io/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-supported-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-supported-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

</div>

---

BookmarkManager is a modern, full-stack web application designed to help users efficiently manage, organize, and categorize their bookmarks. Built with **Clean Architecture** and the **CQRS** pattern, it delivers a scalable, testable, and maintainable codebase with a premium Angular frontend.

## ✨ Features

| Feature | Description |
|---|---|
| 📌 **Bookmark CRUD** | Create, read, update, and delete bookmarks with rich metadata |
| 🏷️ **Tags & Folders** | Organize bookmarks using tags and hierarchical folders |
| 🔐 **JWT Authentication** | Secure login/registration with `BCrypt.Net-Next` password hashing |
| 🛡️ **IDOR Prevention** | Ownership checks enforce that users can only access their own data |
| ⚡ **Rate Limiting** | Built-in ASP.NET Core rate limiting protects sensitive endpoints |
| ✅ **FluentValidation** | Comprehensive request validation across all API commands |
| 🌙 **Dark Mode** | Persistent dark/light mode toggle with a premium Angular UI |
| 🗄️ **Multi-Database** | Configurable connection factory supporting PostgreSQL and SQL Server |

## 🏗️ Architecture

This project implements **Clean Architecture** with strict layer separation, combined with **CQRS** for clear segregation of read and write operations.

```
┌─────────────────────────────────────────┐
│             ClientApp (Angular 21)       │  ← Presentation (SPA)
├─────────────────────────────────────────┤
│              WebAPI (ASP.NET Core)       │  ← API / Host Layer
├─────────────────────────────────────────┤
│              Application Layer           │  ← Commands, Queries, DTOs
├─────────────────────────────────────────┤
│              Domain Layer                │  ← Entities, Interfaces
├─────────────────────────────────────────┤
│           Infrastructure Layer           │  ← Dapper, DbUp, Auth
└─────────────────────────────────────────┘
```

### Project Structure

```
BookmarkManager/
├── BookmarkManager.Domain/         # Entities, domain interfaces (Bookmark, User, Tag)
├── BookmarkManager.Application/    # CQRS Commands & Queries, business use cases, validators
├── BookmarkManager.Infrastructure/ # Dapper repos, DB connection factory, DbUp migrations, JWT
├── BookmarkManager.WebAPI/         # REST API controllers, DI setup, rate limiting, SPA host
└── BookmarkManager.ClientApp/      # Angular 21 SPA (TypeScript, RxJS)
```

## 💻 Tech Stack

### Backend

| Technology | Purpose |
|---|---|
| **ASP.NET Core (.NET 10)** | Web API framework |
| **Clean Architecture + CQRS** | Architectural patterns |
| **Dapper** | Micro-ORM for high-performance SQL |
| **DbUp** | SQL migration runner (auto-runs on startup) |
| **FluentValidation** | Request validation |
| **BCrypt.Net-Next** | Secure password hashing |
| **JWT Bearer Authentication** | Stateless API authentication |
| **PostgreSQL / SQL Server** | Supported relational databases |

### Frontend

| Technology | Purpose |
|---|---|
| **Angular 21** | SPA framework |
| **TypeScript** | Type-safe development |
| **RxJS** | Reactive asynchronous data streams |

## 🛠️ Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`
- A running instance of **PostgreSQL** or **SQL Server**

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/BookmarkManager.git
cd BookmarkManager
```

### 2. Configure the Application

Open `BookmarkManager.WebAPI/appsettings.json` and update the following values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookmarkManager;User Id=sa;Password=your_password;TrustServerCertificate=true;",
    "PostgresConnection": "Server=localhost;Port=5432;Database=BookmarkManager;Username=postgres;Password=your_password"
  },
  "Provider": "PostgreSQL",
  "JwtSettings": {
    "Secret": "your-super-secret-key-at-least-32-chars",
    "Issuer": "BookmarkManagerApi",
    "Audience": "BookmarkManagerClient",
    "ExpiryMinutes": 60
  }
}
```

> **Provider** accepts `"PostgreSQL"` or `"SqlServer"`. Set this to switch which connection string the app uses at runtime.

### 3. Install Frontend Dependencies

```bash
cd BookmarkManager.ClientApp
npm install
```

### 4. Run the Application

Navigate back to the WebAPI directory and start the app:

```bash
cd ../BookmarkManager.WebAPI
dotnet run
```

> The WebAPI uses an **SPA proxy** — it automatically launches the Angular development server alongside the backend. Database migrations via **DbUp** also run automatically on startup.

### 5. Open the App

Navigate to [http://localhost:4200](http://localhost:4200) in your browser.

## 🔑 Key Design Decisions

### Why Dapper over EF Core?
Dapper provides direct SQL control, aligning perfectly with the CQRS pattern where read queries need to be highly optimized. It eliminates ORM overhead while keeping the data access layer transparent and performant.

### Why DbUp?
DbUp runs migration scripts automatically at application startup, keeping database schema evolution purely code-driven with no external tooling required. It's simple, reliable, and infrastructure-agnostic.

### Why CQRS?
Separating **Commands** (writes) from **Queries** (reads) enforces single-responsibility at the use-case level. This makes the application layer easy to reason about, test, and extend independently for read and write workloads.

### IDOR Prevention
All API endpoints that access user-owned resources perform explicit ownership checks, ensuring a user cannot read, update, or delete data belonging to another user — even with a valid JWT token.

## 📄 License

This project is licensed under the [MIT License](LICENSE).
