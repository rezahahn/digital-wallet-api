# Digital Wallet API 🏦
A sample microservice for handling digital wallet transactions, featuring Redis caching and message broker-based event handling.

# Solution Structure 📦
graph TD
    A[digital-wallet-api] --> B[src]
    A --> C[test]
    B --> D[Api]
    B --> E[Application]
    B --> F[Core]
    B --> G[Infrastructure]
    C --> H[Tests]

# Features 🚀
User management
Wallet transactions
Event-driven architecture
Redis caching

# Prerequisites 🛠️
.NET 8 SDK
SQL Server
Docker (for RabbitMQ and Redis)

# Docker Setup 🐳
docker-compose -f docker-compose.infrastructure.yml up -d
docker-compose -f docker-compose.app.yml up --build
