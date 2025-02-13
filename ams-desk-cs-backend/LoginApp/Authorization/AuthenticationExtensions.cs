using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ams_desk_cs_backend.LoginApp.Authorization
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddJwtBearerFromCookie(
            this AuthenticationBuilder builder,
            string name,
            string cookieName,
            IConfiguration configuration)
        {
            return builder.AddJwtBearer(name, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["Login:JWT:Issuer"],
                    ValidAudience = configuration["Login:JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Login:JWT:Key"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                };
                options.MapInboundClaims = false;
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey(cookieName))
                        {
                            context.Token = context.Request.Cookies[cookieName];
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
