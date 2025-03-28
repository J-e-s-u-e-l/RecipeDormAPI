using Microsoft.EntityFrameworkCore;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipeDormAPI.Infrastructure.Infrastructure.Persistence.Utilities.SeedRecipes
{
    public class SeedRecipes_AddDescToAllRecipes
    {
        private readonly DataDbContext _dbContext;

        public SeedRecipes_AddDescToAllRecipes(DataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region SeedRecipe Algorithm V1
        public async Task UpdateRecipeDescriptions(AppSettings appSettings)
        {
            var recipes = await _dbContext.Recipes.Where(r => r.SpoonacularId == null || string.IsNullOrEmpty(r.Description)).ToListAsync();
            var httpClient = new HttpClient();

            foreach (var recipe in recipes)
            {
                try
                {
                    string url = $"https://api.spoonacular.com/recipes/complexSearch?query={Uri.EscapeDataString(recipe.Title)}&apiKey={appSettings.SpoonacularApiKey}";

                    // Search by Title to get Spooonacular ID
                    var searchResponse = await httpClient.GetStringAsync(url);
                    var searchJson = JsonSerializer.Deserialize<JsonElement>(searchResponse);

                    if (searchJson.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
                    {
                        var firstMatch = results[0];
                        int sponacularId = firstMatch.GetProperty("id").GetInt32();

                        // Fetch full recipe details using the Spoonacular ID
                        string detailsUrl = $"https://api.spoonacular.com/recipes/{sponacularId}/information?apiKey={appSettings.SpoonacularApiKey}";
                        var detailsResponse = await httpClient.GetStringAsync(detailsUrl);
                        var detailsJson = JsonSerializer.Deserialize<JsonElement>(detailsResponse);

                        if (detailsJson.TryGetProperty("summary", out var summary))
                        {
                            // Remove HTML tags from the summary
                            string cleanedSummary = Regex.Replace(summary.GetString(), "<.*?>", "");

                            // Update the recipe record
                            recipe.SpoonacularId = sponacularId;
                            recipe.Description = cleanedSummary;

                            _dbContext.Update(recipe);

                            await _dbContext.SaveChangesAsync();

                            Console.WriteLine($"Updated recipe '{recipe.Title}' with Spoonacular ID {sponacularId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠ Error processing {recipe.Title}: {ex.Message}");
                }
            }

            await _dbContext.SaveChangesAsync();
            Console.WriteLine("All recipes updated successfully.");
        }

        #endregion

        #region SeedRecipe Algorithm V2
        /*public async Task UpdateRecipeDescriptions(AppSettings appSettings)
    {
        var recipes = await _dbContext.Recipes.Where(r => r.SpoonacularId == null || string.IsNullOrEmpty(r.Description)).ToListAsync();
        using var httpClient = new HttpClient();
        var recipesList = recipes.ToList();
        var updatedRecipes = new List<Recipes>();
        const int searchBatchSize = 10; // Titles per complexSearch request
        const int bulkBatchSize = 50;   // IDs per informationBulk request (adjust based on free tier)

        try
        {
            // Step 1: Batch titles for complexSearch
            var titleBatches = recipesList.Select(r => r.Title).Distinct()
                .Select((title, index) => new { title, batch = index / searchBatchSize })
                .GroupBy(x => x.batch, x => x.title);

            var titleToIdMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var allIds = new HashSet<int>();

            foreach (var batch in titleBatches)
            {
                try
                {
                    string combinedQuery = string.Join(" OR ", batch.Select(Uri.EscapeDataString));
                    string searchUrl = $"https://api.spoonacular.com/recipes/complexSearch?query={combinedQuery}&number={batch.Count()}&apiKey={appSettings.SpoonacularApiKey}";
                    var searchResponse = await httpClient.GetStringAsync(searchUrl);
                    var searchJson = JsonSerializer.Deserialize<JsonElement>(searchResponse);

                    if (searchJson.TryGetProperty("results", out var results))
                    {
                        foreach (var result in results.EnumerateArray())
                        {
                            int id = result.GetProperty("id").GetInt32();
                            string title = result.GetProperty("title").GetString();
                            allIds.Add(id);
                            titleToIdMap[title] = id;
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Search batch failed: {ex.Message}");
                }
            }

            // Step 2: Fetch details in bulk
            foreach (var idBatch in allIds.Chunk(bulkBatchSize))
            {
                try
                {
                    string bulkUrl = $"https://api.spoonacular.com/recipes/informationBulk?ids={string.Join(",", idBatch)}&apiKey={appSettings.SpoonacularApiKey}";
                    var bulkResponse = await httpClient.GetStringAsync(bulkUrl);
                    var bulkJson = JsonSerializer.Deserialize<JsonElement>(bulkResponse);

                    foreach (var recipeDetail in bulkJson.EnumerateArray())
                    {
                        int id = recipeDetail.GetProperty("id").GetInt32();
                        string summary = recipeDetail.TryGetProperty("summary", out var summaryProp)
                            ? Regex.Replace(summaryProp.GetString(), "<.*?>", "")
                            : null;

                        var matchingRecipes = recipesList.Where(r =>
                            titleToIdMap.ContainsKey(r.Title) && titleToIdMap[r.Title] == id);
                        foreach (var recipe in matchingRecipes)
                        {
                            recipe.SpoonacularId = id;
                            recipe.Description = summary;
                            updatedRecipes.Add(recipe);
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Bulk fetch failed for IDs {string.Join(",", idBatch)}: {ex.Message}");
                }
            }

            // Step 3: Update database
            if (updatedRecipes.Any())
            {
                _dbContext.Recipes.UpdateRange(updatedRecipes);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"Updated {updatedRecipes.Count} of {recipesList.Count} recipes");
            }
            else
            {
                Console.WriteLine("No recipes updated; no matches found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }*/
        #endregion
    }
}
