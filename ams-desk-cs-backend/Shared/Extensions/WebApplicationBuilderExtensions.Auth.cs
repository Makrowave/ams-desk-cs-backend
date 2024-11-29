using ams_desk_cs_backend.LoginApp.Application.Authorization;
using ams_desk_cs_backend.LoginApp.Application.Interfaces;
using ams_desk_cs_backend.LoginApp.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ams_desk_cs_backend.Shared.Extensions
{
    public static partial class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAllAuthentication(this WebApplicationBuilder builder)
        {
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
            return builder;
        }

        public static WebApplicationBuilder AddAllAuthorization(this WebApplicationBuilder builder)
        {
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
            return builder;
        }
        public static WebApplicationBuilder AddAuthServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            return builder;
        }
    }
}
