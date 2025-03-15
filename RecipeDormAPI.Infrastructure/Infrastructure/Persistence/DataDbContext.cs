using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeAPI.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeDormAPI.Infrastructure.Infrastructure.Persistence
{
    public class DataDbContext : IdentityDbContext<Users, IdentityRole<Guid>, Guid>
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Recipes> Recipes { get; set; }
        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<Steps> Steps { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Bookmarks> Bookmarks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);
            modelbuilder.SetUp();
        }
    }
}