# Appointment Management API

## Quick Start

### Launch the applicationdocker-compose up -d
### Access the application
- **API Documentation (Swagger)**: http://localhost:8080/swagger
- **API Base URL**: http://localhost:8080
- **Health Check**: http://localhost:8080/health

## Database Configuration

The database is automatically configured with Docker. Default credentials:
- **Server**: localhost:1433
- **Database**: AppointmentManagementDb
- **User**: sa
- **Password**: Passw0rd123

To change database settings, edit `environment/.env` file:DB_PASSWORD=YourPassword
DB_NAME=YourDatabaseName
## Useful Commands
# Start application
docker-compose up -d

# Stop application
docker-compose down

# View logs
docker-compose logs -f

# Restart
docker-compose restart
That's it! ??
