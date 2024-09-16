using Npgsql;
using ams_desk_cs_backend;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using ams_desk_cs_backend.BikeService.Controllers;
using System.Text.Json.Serialization;
using ams_desk_cs_backend.BikeService.Models;
var builder = WebApplication.CreateBuilder(args);
var ReactFrontend = "reactFrontEnd";
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DBConnectionString");
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<BikesDbContext>(options
    => options.UseNpgsql(connectionString));

builder.Services.AddControllers()
    .AddJsonOptions(o => { o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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