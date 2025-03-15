using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Infrastructure.Infrastructure.Auth.Jwt;

namespace RecipeDormAPI.Infrastructure.Infrastructure.Auth
{
    public static class Extensions
    {
        public static IServiceCollection RegisterIdentity(this IServiceCollection services)
        {
            services.AddIdentity<Users, IdentityRole<Guid>>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 6;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<DataDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromMinutes(30));

            return services;
        }


        public static IServiceCollection RegisterJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<JwtSettings>();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddSingleton<IJwtHandler, JwtHandler>();

            return services;
        }
    }
}
