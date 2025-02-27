# ChartWebApp
# README: Guide to Running Docker Compose for .NET & MySQL

## 🚀 Introduction
This project uses **.NET Core** with **MySQL** and runs on **Docker Compose**. This guide will help you set up the environment and run the application easily.

---

## 🛠 System Requirements
Before starting, ensure you have installed:
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

---

## 📦 Project Structure
```
.
├── docker-compose.yml    # Docker Compose configuration
├── Dockerfile            # Docker configuration for .NET service
├── appsettings.json      # Application settings
├── src/                  # .NET source code
└── README.md             # Usage guide
```

---

## 🔧 `docker-compose.yml` Configuration
Below is the sample `docker-compose.yml` configuration to run **MySQL** and **.NET Service**:

```yaml
version: '3.8'
services:
  chartwebapp:
    image: chartwebapp
    build:
      context: .
      dockerfile: Dockerfile
    entrypoint: ["/bin/sh", "-c", "echo 'Waiting 30s for MySQL...'; sleep 30; dotnet ChartWebApp.dll"]
    ports:
      - "9800:80"
    depends_on:
      mysql:
        condition: service_healthy
  mysql:
    image: mysql:8.0
    container_name: mysql_container
    restart: always
    environment:
      MYSQL_ROOT_PASSWORD: chart_app
      MYSQL_DATABASE: chart_app
      MYSQL_USER: chart_app
      MYSQL_PASSWORD: chart_app
    ports:
      - "3306:3306"
    volumes:
      - ./mysql_data:/var/lib/mysql
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      interval: 10s
      retries: 5
      start_period: 10s
```

---

## ▶️ Running the Project
### 1️⃣ **Build & Run Containers**
```sh
docker-compose up -d --build
```
📌 **Note:**
- `-d`: Runs in detached mode.
- `--build`: Rebuilds the image if there are changes.

### 2️⃣ **View Container Logs**
```sh
docker-compose logs -f chartwebapp-chartwebapp-1
```

### 3️⃣ **Check Running Containers**
```sh
docker ps
```

### 4️⃣ **Stop & Remove Containers**
```sh
docker-compose down
```

---

🌍 Accessing the Application

Once the containers are running, you can access the application in your browser at:
👉 http://localhost:9980

---
## 🐳 Checking MySQL Connection from the Container
To ensure the MySQL container is running, you can check the connection with:
```sh
docker exec -it chartwebapp-chartwebapp-1 sh
mysql -h mysql_container -u chart_app -p
```

If the connection is successful, MySQL is ready! 🎉

---

## ❓ Common Issues
### 🔴 `Unable to connect to MySQL`
**Solution:**
1. Check if MySQL is running:
   ```sh
   docker ps
   ```
2. If MySQL is not running, restart it:
   ```sh
   docker-compose up -d mysql
   ```
3. If the issue persists, check MySQL logs:
   ```sh
   docker-compose logs -f mysql
   ```

---

## 🎯 Conclusion
You have successfully run the `.NET + MySQL` application using Docker Compose. If you encounter any issues, check the logs or connection settings. 🚀

