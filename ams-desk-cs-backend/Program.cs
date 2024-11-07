using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Configuration;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Application.Services;
using ams_desk_cs_backend.BikeApp.Application.Interfaces.Validators;
using ams_desk_cs_backend.BikeApp.Application.Validators;
using ams_desk_cs_backend.LoginApp.Infrastructure.Data;
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
//Bikes app services
builder.Services.AddScoped<IBikesService, BikesService>();
builder.Services.AddScoped<IColorsService, ColorsService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IEmployeesService, EmployeesService>();
builder.Services.AddScoped<IManufacturersService, ManufacturersService>();
builder.Services.AddScoped<IPlacesService, PlaceService>();
builder.Services.AddScoped<IModelsService, ModelsService>();

//Bikes app validators
builder.Services.AddScoped<IModelValidator, IncompleteModelValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer("AccessToken", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Login:JWT:Issuer"],
        ValidAudience = builder.Configuration["Login:JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Login:JWT:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
    };
}).AddJwtBearer("RefreshToken", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Login:JWT:Issuer"],
        ValidAudience = builder.Configuration["Login:JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Login:JWT:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("refresh_token"))
            {
                context.Token = context.Request.Cookies["refresh_token"];
            }
            return Task.CompletedTask;
        }
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AccessToken", policy =>
    {
        policy.AddAuthenticationSchemes("AccessToken");
        policy.RequireAuthenticatedUser();
    });

    options.AddPolicy("RefreshToken", policy =>
    {
        policy.AddAuthenticationSchemes("RefreshToken");
        policy.RequireAuthenticatedUser();
    });
});

// Configure CORS
var FrontEndURL = builder.Configuration["CORSOrigins"];
var PolicyName = "FrontEnd";
builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: PolicyName,
            policy =>
            {
                if(FrontEndURL != null)
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