using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Configuration;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
}

// Connect to DBs
var connectionString = builder.Configuration["Bikes:ConnectionString"];
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<BikesDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(o => { o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

builder.AddBikeServices();
builder.AddAuthServices();
builder.AddAllAuthentication();
builder.AddAllAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
var frontEndUrl = builder.Configuration["CORSOrigins"];
const string policyName = "FrontEnd";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName,
        policy =>
        {
            if (frontEndUrl != null)
            {
                policy.WithOrigins(frontEndUrl)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
            else
            {
                throw new ConfigurationErrorsException("FrontEndURL was not specified in appsettings.json");
            }
        });
});

var app = builder.Build();

// Run migrations at startup with logging
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<BikesDbContext>();
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying database migrations.");
        throw; // Optionally rethrow to stop app startup
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(policyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
