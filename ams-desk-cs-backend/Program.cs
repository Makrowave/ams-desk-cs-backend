using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ams_desk_cs_backend.BikeService.Models;
using ams_desk_cs_backend.LoginService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
var builder = WebApplication.CreateBuilder(args);

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Authentication

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//   options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//
//}).AddJwtBearer(options =>
//{
//
//})

// Configure CORS
var ReactFrontend = "reactFrontEnd";
builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: ReactFrontend,
            policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .WithMethods("GET", "PUT", "DELETE")
                    .AllowAnyHeader();
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

app.UseCors(ReactFrontend);

app.UseAuthorization();

app.MapControllers();

app.Run();