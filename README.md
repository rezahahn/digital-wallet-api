# Digital Wallet API 🏦
A sample microservice for handling digital wallet transactions, featuring Redis caching and message broker-based event handling.

# Solution Structure 📦
![deepseek_mermaid_20250501_08f188](https://github.com/user-attachments/assets/3a0a0cef-8bb7-446b-9588-04c0683c7bb8)


# Architecture Highlights 🚀
- SOLID Principles
- Event-driven (RabbitMQ)
- Layered Design (Api > Application > Infrastructure)
- Caching Strategy (Redis)

# Prerequisites 🛠️
- .NET 8 SDK
- SQL Server
- Docker (for RabbitMQ and Redis)

# Docker Setup 🐳
docker-compose -f docker-compose.infrastructure.yml up -d
docker-compose -f docker-compose.app.yml up --build
