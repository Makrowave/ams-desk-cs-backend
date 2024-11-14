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
using ams_desk_cs_backend.LoginApp.Application.Interfaces;
using ams_desk_cs_backend.LoginApp.Application.Services;
using ams_desk_cs_backend.LoginApp.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
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
builder.Services.AddSingleton<IModelValidator, IncompleteModelValidator>();
builder.Services.AddSingleton<ICommonValidator, CommonValidator>();

//Auth
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
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
    options.MapInboundClaims = false;
}).AddJwtBearerFromCookie("RefreshToken", "refresh_token", builder.Configuration)
.AddJwtBearerFromCookie("AdminRefreshToken", "admin_token", builder.Configuration);

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AccessToken", policy =>
    {
        policy.AddAuthenticationSchemes("AccessToken");
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new VersionRequirement());
    });

    options.AddPolicy("RefreshToken", policy =>
    {
        policy.AddAuthenticationSchemes("RefreshToken");
        policy.RequireAuthenticatedUser();
    });
    // The policy shouldn't log out user when the admin token is invalidated or admin is logged in.
    // It should do that to their admin permission, that's why so many policies.
    options.AddPolicy("AdminAccessToken", policy =>
    {
        policy.AddAuthenticationSchemes("AccessToken");
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new VersionRequirement());
        policy.AddRequirements(new AdminRequirement());
    });

    options.AddPolicy("AdminRefreshToken", policy =>
    {
        policy.AddAuthenticationSchemes("AdminRefreshToken");
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new AdminRequirement());
    });
});
builder.Services.AddScoped<IAuthorizationHandler, VersionAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();

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