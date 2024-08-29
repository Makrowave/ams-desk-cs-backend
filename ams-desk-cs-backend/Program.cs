using Npgsql;
using ams_desk_cs_backend;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using ams_desk_cs_backend.Models;
using ams_desk_cs_backend.Controllers;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DBConnectionString");
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<BikesDbContext>(options
    => options.UseNpgsql(connectionString));

builder.Services.AddControllers()
    .AddJsonOptions(o => { o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();