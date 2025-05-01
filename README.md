# Digital Wallet API 🏦
A sample microservice for handling digital wallet transactions, featuring Redis caching and message broker-based event handling.

# Solution Structure 📦
![deepseek_mermaid_20250501_0c3f4a](https://github.com/user-attachments/assets/bed32d7f-d775-4d5a-aa6d-4d011e7483fa)

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
