using AppointmentManagement.Application;
using AppointmentManagement.Infrastructure;
using AppointmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Appointment Management API",
        Version = "v1.0.0",
        Description = "API pour la gestion des rendez-vous médicaux",
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "support@appointmentmanagement.com"
        }
    });

    // Add XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add application and infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("DefaultConnection string is not configured."));

// Add health checks (simplified)
builder.Services.AddHealthChecks();

// Configure CORS from configuration
var corsOrigins = builder.Configuration["AppSettings:CorsAllowedOrigins"]?.Split(',') ?? new[] { "*" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (corsOrigins.Contains("*"))
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// FORCE Swagger to be always enabled for troubleshooting
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("Swagger forcé activé pour débogage");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Appointment Management API v1");
    c.RoutePrefix = "swagger"; // Accessible sur /swagger
    c.DisplayRequestDuration();
    c.EnableTryItOutByDefault();
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

// Remove HTTPS redirection for Docker troubleshooting
// app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthorization();

// Map health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

// Add a simple root endpoint for testing
app.MapGet("/", () => new {
    Message = "Appointment Management API is running",
    SwaggerUrl = "/swagger",
    HealthUrl = "/health",
    ApiUrl = "/api/appointments"
});

Console.WriteLine("Application démarrée. Swagger disponible sur /swagger");

app.Run();




