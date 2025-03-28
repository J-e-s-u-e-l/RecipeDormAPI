using RecipeAPI.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeDormAPI.Infrastructure.Data.Models.DTOs
{
    public class RecipeSearchResultDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Relevance { get; set; }
    }

    public class PaginationMetaData
    {
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
    public class IngredientsDto
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
    }

    public class StepsDto
    {
        public int StepNumber { get; set; }
        public string Description { get; set; }
    }

    public class RecipeDto
    {
        public Guid RecipeId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }
}
