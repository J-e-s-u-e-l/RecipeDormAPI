using Microsoft.AspNetCore.Identity;

namespace RecipeAPI.Infrastructure.Data.Entities
{
    public class Users : IdentityUser<Guid>
    {
        public bool IsActive { get; set; }
        public DateTimeOffset LastLogin { get; set; }
        public DateTimeOffset TimeCreated { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset TimeUpdated { get; set; } = DateTimeOffset.UtcNow;
    

        // Navigation properties
        public List<Recipes>? Recipes { get; set; }
        public List<Likes>? Likes { get; set; }
        public List<Bookmarks>? Bookmarks { get; set; }
    }
}
