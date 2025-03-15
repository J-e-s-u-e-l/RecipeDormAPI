using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAPI.Infrastructure.Data.Entities
{
    public class Likes : BaseEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(Recipe))]
        public Guid RecipeId { get; set; }

        // Navigation properties
        public Users User { get; set; }
        public Recipes Recipe { get; set; }
    }
}