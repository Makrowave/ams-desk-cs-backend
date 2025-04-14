using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Configuration;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Login.Data;
using ams_desk_cs_backend.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);
if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.SetBasePath(AppContext.BaseDirectory)
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
}

// Connect to DBs
var connectionString = builder.Configuration["Bikes:ConnectionString"];
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<BikesDbContext>(options
    => options.UseNpgsql(connectionString));

var loginConnectionString = builder.Configuration["Login:ConnectionString"];
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<UserCredContext>(options
    => options.UseNpgsql(loginConnectionString));

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(o => { o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

builder.AddBikeServices();
builder.AddAuthServices();
builder.AddAllAuthentication();
builder.AddAllAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
var FrontEndURL = builder.Configuration["CORSOrigins"];
var PolicyName = "FrontEnd";
builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: PolicyName,
            policy =>
            {
                if (FrontEndURL != null)
                {
                    policy.WithOrigins([FrontEndURL])
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(PolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();