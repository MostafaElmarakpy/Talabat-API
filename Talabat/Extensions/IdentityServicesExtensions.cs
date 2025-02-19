using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Service;
using Talabat.Repository.Identity;
using Talabat.Service;

namespace Talabat.Extensions
{
    public static class IdentityServicesExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped(typeof(ITokenService), typeof(TokenService));
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // options.Password.RequireNonAlphanumeric = false;
                // options.Password.RequireDigit = false;
                // options.Password.RequireLowercase = false;
                // options.Password.RequireUppercase = false;
                // options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudience = config["JWT:ValidAudience"],
                        ValidateIssuer = true,
                        ValidIssuer = config["JWT:ValidIssuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecretKey"])),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromDays(double.Parse(config["JWT:TokenLifeTime"]))

                    };
                });
            return services;
        }
    }
}