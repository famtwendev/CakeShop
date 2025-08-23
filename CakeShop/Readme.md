docker-compose up --build



==================== Wrap product ======================
```Dockerfile

# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP\_UID
WORKDIR /app
EXPOSE 5125
ENV ASPNETCORE\_URLS=http://0.0.0.0:5125


# This stage is used to build the service project

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD\_CONFIGURATION=Release
WORKDIR /src
COPY \["CakeShop.csproj", "."]
RUN dotnet restore "./CakeShop.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./CakeShop.csproj" -c $BUILD\_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage

FROM build AS publish
ARG BUILD\_CONFIGURATION=Release
RUN dotnet publish "./CakeShop.csproj" -c $BUILD\_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 👇 Set ASPNETCORE\_ENVIRONMENT = Production

ENV ASPNETCORE\_ENVIRONMENT=Production

ENTRYPOINT \["dotnet", "CakeShop.dll"]
```

### Build image
```
docker build -t famtwen/cakeshop:0.1 .
```
### Kiểm tra image đã build
```
docker images
```

### Login Docker
```
docker login
```

### Push image:
```
docker push famtwen/cakeshop:0.1

```



### =============== Install Docker on Ubuntu ====================
sudo apt update
sudo apt install -y docker.io
sudo systemctl enable docker
sudo systemctl start docker

### ==============   SQLSERVER 2022   ===========================
sudo docker run -d \
 --name sqlserver  
 --restart unless-stopped \
 -e "ACCEPT\_EULA=Y" \
 -e "SA_PASSWORD=Admin@123" \
 -p 1433:1433 \
 -d mcr.microsoft.com/mssql/server:2022-latest


docker run -d \
  --name sqlserver \
  --restart unless-stopped \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Admin@123" \
  -p 1433:1433 \
  -v sqlserver_data:/var/opt/mssql \
  mcr.microsoft.com/mssql/server:2022-latest
### ====================  CAKESHOP  =============================
### Pull Image:
```
docker pull famtwen/cakeshop:0.1
```

### Run Container:
```
docker run -d \
  -p 5125:5125 \
  --restart unless-stopped \
  -e HOSTNAME="ftshop.click" \
  -e "ConnectionStrings__CakeShop=Server=13.213.226.221,1433;Database=CAKESHOP;User Id=sa;Password=Admin@123;TrustServerCertificate=True;" \
  -e "ASPNETCORE_ENVIRONMENT=Development" \
  -e "ASPNETCORE_URLS=http://0.0.0.0:5125" \
  -e "reCAPTCHA__SiteKey=6Lf3n6srAAAAAGL7BggS5ceMuP5K9zyhe6BGJURr" \
  -e "reCAPTCHA__SiteSecret=6Lf3n6srAAAAALHLpB9yD73YmvUlGHMCgi70xcqj" \
  --name cakeshop \
  famtwen/cakeshop:0.1
```
**hoặc:**
```
docker run -d -p 5125:5125 -e HOSTNAME="kinhdocacanh.shop" -e "ConnectionStrings__CakeShop=Server=13.213.226.221,1433;Database=CAKESHOP;User Id=sa;Password=Admin@123;TrustServerCertificate=True;" -e "ASPNETCORE_ENVIRONMENT=Production" -e "ASPNETCORE_URLS=http://0.0.0.0:5125" -e "reCAPTCHA__SiteKey=6Lf3n6srAAAAAGL7BggS5ceMuP5K9zyhe6BGJURr" -e "reCAPTCHA__SiteSecret=6Lf3n6srAAAAALHLpB9yD73YmvUlGHMCgi70xcqj" --name cakeshop famtwen/cakeshop:0.1
```
docker run -d \
  -p 5125:5125 \
  -e "ConnectionStrings__CakeShop=Server=13.213.226.221,1433;Database=CAKESHOP;User Id=sa;Password=Admin@123;TrustServerCertificate=True;" \
  -e "ASPNETCORE_ENVIRONMENT=Production" \
  -e "ASPNETCORE_URLS=http://0.0.0.0:5125" \
  -e "reCAPTCHA__SiteKey=6Lf3n6srAAAAAGL7BggS5ceMuP5K9zyhe6BGJURr" \
  -e "reCAPTCHA__SiteSecret=6Lf3n6srAAAAALHLpB9yD73YmvUlGHMCgi70xcqj" \
  --name cakeshop \
  famtwen/cakeshop:0.1

docker exec -it cakeshop printenv | grep reCAPTCHA

### Check docker container:
```
sudo docker ps
```
**or:**
```
sudo docker ps -all
```

### Stop container:
```
docker stop <CONTAINER\_ID>
```

### Start container:
```
docker stop <CONTAINER\_ID>
```

### Remove container:
```
docker rm <CONTAINER\_ID>
```

### Check image:
```
sudo docker images
```

### Remove image:
```
rmi <DOCKER\_IMAGES\_ID>
```


