# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Fiap.TechChallenge.Domain/Fiap.TechChallenge.Domain.csproj src/Fiap.TechChallenge.Domain/
COPY src/Fiap.TechChallenge.Application/Fiap.TechChallenge.Application.csproj src/Fiap.TechChallenge.Application/
COPY src/Fiap.TechChallenge.Infrastructure/Fiap.TechChallenge.Infrastructure.csproj src/Fiap.TechChallenge.Infrastructure/
COPY src/Fiap.TechChallenge.External/Fiap.TechChallenge.External.csproj src/Fiap.TechChallenge.External/
COPY src/Fiap.TechChallenge.Api/Fiap.TechChallenge.Api.csproj src/Fiap.TechChallenge.Api/

RUN dotnet restore src/Fiap.TechChallenge.Api/Fiap.TechChallenge.Api.csproj

COPY src/ src/
RUN dotnet publish src/Fiap.TechChallenge.Api/Fiap.TechChallenge.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Create a non-root user and group for security (principle of least privilege)
RUN addgroup --system appgroup && \
    adduser --system --ingroup appgroup --no-create-home --shell /bin/false appuser && \
    chown -R appuser:appgroup /app

USER appuser

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Fiap.TechChallenge.Api.dll"]