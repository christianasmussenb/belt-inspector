# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY api/BeltInspector.Api/BeltInspector.Api.csproj api/BeltInspector.Api/
RUN dotnet restore api/BeltInspector.Api/BeltInspector.Api.csproj
COPY api/BeltInspector.Api/ api/BeltInspector.Api/
RUN dotnet publish api/BeltInspector.Api/BeltInspector.Api.csproj -c Release -o /app/out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
CMD ["/bin/sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} dotnet BeltInspector.Api.dll"]
