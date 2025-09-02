# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project files
COPY AppointmentManagement.Domain/*.csproj AppointmentManagement.Domain/
COPY AppointmentManagement.Application/*.csproj AppointmentManagement.Application/
COPY AppointmentManagement.Infrastructure/*.csproj AppointmentManagement.Infrastructure/
COPY WebApplication6/*.csproj WebApplication6/

# Restore dependencies
RUN dotnet restore WebApplication6/AppointmentManagement.Api.csproj

# Copy source code
COPY AppointmentManagement.Domain/ AppointmentManagement.Domain/
COPY AppointmentManagement.Application/ AppointmentManagement.Application/
COPY AppointmentManagement.Infrastructure/ AppointmentManagement.Infrastructure/
COPY WebApplication6/ WebApplication6/

# Build the application
WORKDIR /app/WebApplication6
RUN dotnet build AppointmentManagement.Api.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish AppointmentManagement.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=publish /app/publish .

# Create a non-root user
RUN groupadd -r appgroup && useradd -r -g appgroup appuser
RUN chown -R appuser:appgroup /app
USER appuser

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "AppointmentManagement.Api.dll"]