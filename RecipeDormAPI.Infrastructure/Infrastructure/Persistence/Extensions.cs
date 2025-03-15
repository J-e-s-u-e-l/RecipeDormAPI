using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Infrastructure.Config;

namespace RecipeDormAPI.Infrastructure.Infrastructure.Persistence
{
    public static class Extensions
    {
        public static IServiceCollection RegisterCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
                string[] origins = appSettings.AllowedOrigins;

                options.AddPolicy("MyCorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                    builder.WithOrigins(origins);
                });
            });
            
            return services;
        }

        public static IServiceCollection RegisterPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }


        public static IServiceCollection RegisterAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AppSettings>();
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            return services;
        }

        public static void SetUp(this ModelBuilder modelBuilder)
        {
            #region Change Identity Table Names
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            #endregion

            #region Cascade Delete for Bookmarks
            modelBuilder.Entity<Bookmarks>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookmarks)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bookmarks>()
                .HasOne(b => b.Recipe)
                .WithMany(r => r.Bookmarks)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Cascade Delete for Likes
            modelBuilder.Entity<Likes>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.Recipe)
                .WithMany(r => r.Likes)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
