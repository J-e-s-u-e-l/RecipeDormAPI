using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAPI.Infrastructure.Data.Entities
{
    public class Steps : BaseEntity
    {
        [ForeignKey(nameof(Recipe))]
        public Guid RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string Description{ get; set; }

        // Navigation properties
        public Recipes Recipe { get; set; }
    }
}
