
# Colours.API ![Docker](https://img.shields.io/badge/Docker-Enabled-blue) ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-green) ![Redis](https://img.shields.io/badge/Redis-Caching-red)

## Introduction
**Colours.API** is a simple WebAPI developed as part of a series of Medium articles on the topic of **Dockerize .NET Web API**. It provides CRUD operations for managing colors and utilizes **PostgreSQL** as the database and **Redis** for caching. The project is containerized with Docker, and the `docker-compose` file enables spinning up all services—API, PostgreSQL, and Redis—together.

## Features
- **CRUD Operations**: Perform Create, Read, Update, Delete operations for colors.
- **PostgreSQL**: Used as the backend database, managed with a Docker image.
- **Redis**: Implements caching with Redis, also managed with a Docker image.
- **Dockerized**: Includes a `Dockerfile` for building the API image and a `docker-compose` setup for running the entire stack.
- **.NET 8**: Built using the latest version of .NET 8.

## Medium Articles
This API is part of a series of articles I've written on Medium discussing **Docker Containers**. You can follow along with the articles to understand how the solution is built and deployed using Docker.

### Articles Links:
Part 1: Containerize .NET 8.0 Web API
https://medium.com/@zeeshan.mustafa91/dockerize-net-8-0-web-api-cbb4b8426fdb

Part 2: Multi-Container Setup for .NET 8.0 Web API with PostgreSQL and Redis
https://medium.com/@zeeshan.mustafa91/docker-multi-container-setup-for-net-8-0-web-api-with-postgresql-and-redis-5147f1-5147f1f269cd

Part 3: Using Volumes to Persist Data in a Multi-Container .NET 8.0 Web API
https://medium.com/@zeeshan.mustafa91/docker-volumes-to-persist-logs-in-multi-container-net-8-0-web-api-bd5880971b17

Part 4: Running a .NET 8.0 Web API over HTTPS Inside Docker
https://medium.com/@zeeshan.mustafa91/api-over-https-inside-docker-04dc0916e790

## Getting Started

### Prerequisites
- [Docker](https://www.docker.com/products/docker-desktop) ![Docker](https://img.shields.io/badge/Docker-Install-blue)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ![Download .NET 8](https://img.shields.io/badge/Download-.NET%208-blue)

### Installation

1. **Clone the repository**
    ```bash
    git clone https://github.com/zeecorleone/Colours.API.git
    cd Colours.API
    ```

2. **Build and run using Docker Compose**
    ```bash
    docker-compose up --build
    ```

3. **Access the API**
   The API will be accessible at:
   - `http://localhost:5000`
   - `http://localhost:5001`

### API Endpoints
- **GET /api/colours**: Retrieve all colors.
- **POST /api/colours**: Add a new color.
- **PUT /api/colours/{id}**: Update an existing color.
- **DELETE /api/colours/{id}**: Delete a color.

## Docker Setup

### Dockerfile
This is how the **Dockerfile** is structured to build and serve the API:

```Dockerfile
# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./Colours.API/Colours.API.csproj" --disable-parallel
RUN dotnet publish "./Colours.API/Colours.API.csproj" -c release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "Colours.API.dll"]
```

### Docker Compose
The `docker-compose.yml` file is used to orchestrate the API, PostgreSQL, and Redis services:

```yaml
version: '3.8'

services:
  api:
    image: modulez33/colours-api:3.0.0
    environment:
        - ASPNETCORE_ENVIRONMENT=Development
        - ASPNETCORE_URLS=http://+:5000;https://+:5001
        - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/colours-api-cert.pfx
        - ASPNETCORE_Kestrel__Certificates__Default__Password=123456
        - ConnectionStrings__Default=User ID=postgres;Password=123456;Server=postgres;Port=5432;Database=coloursdb;Pooling=true;
        - Redis__Host=redis
        - Redis__Port=6379
    volumes:
        - colours_api_logs_data:/app/logs
        - D:/temp-certs/:/https/
    ports:
        - "5000:5000"
        - "5001:5001"
    depends_on:
        - postgres
        - redis

  postgres:
    image: postgres:latest
    environment:
        POSTGRES_USER: postgres
        POSTGRES_PASSWORD: 123456
        POSTGRES_DB: coloursdb
    ports:
        - "5432:5432"

  redis:
    image: redis:latest
    ports:
        - "6379:6379"

volumes:
  colours_api_logs_data: {}
```

### Running Containers
After running `docker-compose up --build`, the following containers will be started:
- **API**: `localhost:5000`
- **PostgreSQL**: on port `5432`
- **Redis**: on port `6379`

## Future Plans
In future updates, I plan to extend this example by discussing advanced Docker topics such as networking between containers, multi-stage builds, and using Docker in CI/CD pipelines.

## Contributing
Contributions are welcome! Please submit a pull request or open an issue to discuss what you would like to change.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

