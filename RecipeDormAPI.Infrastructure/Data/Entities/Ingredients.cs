using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAPI.Infrastructure.Data.Entities
{
    public class Ingredients : BaseEntity
    {
        [ForeignKey(nameof(Recipe))]
        public Guid RecipeId { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }

        // Navigation properties
        public virtual Recipes Recipe { get; set; }
    }
}
