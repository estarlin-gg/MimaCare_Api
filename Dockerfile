# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore por proyecto (ayerde el SDK limpie los paquetes de otros proyectos)
COPY ["MimaCare.Presentation/MimaCare.Presentation.csproj", "MimaCare.Presentation/"]
COPY ["MimaCare.Application/MimaCare.Application.csproj", "MimaCare.Application/"]
COPY ["MimaCare.Entities/MimaCare.Domain.csproj", "MimaCare.Entities/"]
COPY ["MimaCare.infrastructure/MimaCare.Infrastructure.csproj", "MimaCare.infrastructure/"]
RUN dotnet restore "MimaCare.Presentation/MimaCare.Presentation.csproj"

# Compilar y publicar
COPY . .
WORKDIR /src/MimaCare.Presentation
RUN dotnet publish "MimaCare.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Escucha sin TLS dentro del contenedor (el reverse proxy/TLS es responsabilidad del host)
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "MimaCare.Presentation.dll"]