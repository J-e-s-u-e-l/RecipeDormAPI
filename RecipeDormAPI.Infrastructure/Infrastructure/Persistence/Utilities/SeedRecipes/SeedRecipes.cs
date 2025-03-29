using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipeDormAPI.Infrastructure.Infrastructure.Persistence.Utilities.SeedRecipes
{
    public class SpoonaCularRecipe
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("summary")] 
        public string Summary { get; set; }

        [JsonPropertyName("extendedIngredients")]
        public List<Ingredient> ExtendedIngredients { get; set; }

        [JsonPropertyName("analyzedInstructions")]
        public List<Instruction> AnalyzedInstructions { get; set; }

        public class Ingredient
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("amount")]
            public double Amount { get; set; }

            [JsonPropertyName("unit")]
            public string Unit { get; set; }
        }

        public class Instruction
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("steps")]
            public List<Step> Steps { get; set; }
        }

        public class Step
        {
            [JsonPropertyName("number")]
            public int Number { get; set; }

            [JsonPropertyName("step")]
            public string StepText { get; set; }
        }
    }

    public class SeedRecipes
    {
        static readonly HttpClient client = new HttpClient();

        public static async Task Initialize(IServiceProvider serviceProvider, int numberOfRecipes, AppSettings appSettings)
        {
            using var scope = serviceProvider.CreateScope();
            var _dataDbContext = scope.ServiceProvider.GetRequiredService<DataDbContext>();

            var existingTitles = _dataDbContext.Recipes.Select(r => r.Title).ToHashSet();

            try
            {
                string url = $"https://api.spoonacular.com/recipes/random?number={numberOfRecipes}&apiKey={appSettings.SpoonacularApiKey}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Dictionary<string, List<SpoonaCularRecipe>>>(json);
                var recipes = data["recipes"];

                foreach (var apiRecipe in recipes)
                {
                    // Validate essential data
                    if (string.IsNullOrEmpty(apiRecipe.Title) ||
                        apiRecipe.ExtendedIngredients == null || apiRecipe.ExtendedIngredients.Count == 0 ||
                        apiRecipe.AnalyzedInstructions == null || apiRecipe.AnalyzedInstructions.Count == 0)
                    {
                        Console.WriteLine($"Skipping recipe '{apiRecipe.Title ?? "Unknown"}' due to missing data.");
                        continue;
                    }
                    if (existingTitles.Contains(apiRecipe.Title))
                    {
                        Console.WriteLine($"Duplicate '{apiRecipe.Title}' skipped.");
                        continue;
                    }

                    // Clean summary (remove HTML tags if present)
                    string cleanedSummary = apiRecipe.Summary != null
                        ? Regex.Replace(apiRecipe.Summary, "<.*?>", "")
                        : "No description available";

                    // Seed Recipes with Description and SpoonacularId
                    var recipe = new Recipes
                    {
                        Id = Guid.NewGuid(),
                        UserId = null,
                        Title = apiRecipe.Title,
                        ImageUrl = apiRecipe.Image,
                        Description = cleanedSummary,      
                        SpoonacularId = apiRecipe.Id,      
                        Ingredients = new List<Ingredients>(),
                        Steps = new List<Steps>()
                    };

                    // Seed Ingredients
                    foreach (var ingr in apiRecipe.ExtendedIngredients)
                    {
                        recipe.Ingredients.Add(new Ingredients
                        {
                            Id = Guid.NewGuid(),
                            RecipeId = recipe.Id,
                            Name = ingr.Name,
                            Quantity = $"{ingr.Amount} {ingr.Unit}".Trim()
                        });
                    }

                    // Seed Steps (check if instructions exist)
                    if (apiRecipe.AnalyzedInstructions?.Count > 0 && apiRecipe.AnalyzedInstructions[0].Steps?.Count > 0)
                    {
                        foreach (var step in apiRecipe.AnalyzedInstructions[0].Steps)
                        {
                            recipe.Steps.Add(new Steps
                            {
                                Id = Guid.NewGuid(),
                                RecipeId = recipe.Id,
                                StepNumber = step.Number,
                                Description = step.StepText
                            });
                        }
                    }

                    _dataDbContext.Recipes.Add(recipe);
                    existingTitles.Add(apiRecipe.Title);
                }

                await _dataDbContext.SaveChangesAsync();
                Console.WriteLine($"Seeded {recipes.Count} recipes with descriptions and Spoonacular IDs");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API request failed: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to parse API response: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding failed: {ex.Message}");
            }
        }
    }
}
