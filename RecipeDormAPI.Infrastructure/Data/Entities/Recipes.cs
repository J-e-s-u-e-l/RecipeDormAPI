using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAPI.Infrastructure.Data.Entities
{
    public class Recipes : BaseEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public int? SpoonacularId { get; set; }

        // Navigation properties
        public Users User { get; set; }
        public List<Ingredients> Ingredients { get; set; }
        public List<Steps> Steps { get; set; }
        public List<Likes> Likes { get; set; }
        public List<Bookmarks> Bookmarks { get; set; }
    }
}
