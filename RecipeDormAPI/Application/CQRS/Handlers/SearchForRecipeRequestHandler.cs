using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Queries;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.DTOs;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Auth.Jwt;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class SearchForRecipeRequestHandler : IRequestHandler<SearchForRecipeRequest, BaseResponse<SearchForRecipeResponse>>
    {
        private readonly DataDbContext _dbContext;
        private readonly ILogger<SearchForRecipeRequestHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int PageSize = 20;

        public SearchForRecipeRequestHandler(DataDbContext dbContext, ILogger<SearchForRecipeRequestHandler> logger, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse<SearchForRecipeResponse>> Handle(SearchForRecipeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (_httpContextAccessor.HttpContext?.Items["UserId"] is not Guid userId)
                {
                    _logger.LogError("UserId not found in HttpContext");
                    return new BaseResponse<SearchForRecipeResponse>(false, "User authentication required");
                }

                // Search Process
                // Input Processing
                string normalizedQuery = request.SearchQuery.ToLower().Trim();
                var queryWords = normalizedQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 2) // Simple heuristic to remove short words like "and"
                    .ToList();

                // Exact Match Search
                var allResults = new List<(Recipes Recipe, int Score, string Relevance)>();
                var exactMatches = await _dbContext.Recipes
                    .Where(r => r.Title.ToLower() == normalizedQuery)
                    .ToListAsync();

                foreach (var recipe in exactMatches)
                {
                    allResults.Add((recipe, 100, "Exact Match"));
                }

                // If exact matches exist, proceed to pagination; otherwise, continue to adaptive search
                if (allResults.Count == 0)
                {
                    // Approximate Match Search (Title Similarity)
                    var approximateMatches = await _dbContext.Recipes
                        .Where(r => queryWords.Any(q => r.Title.ToLower().Contains(q)))
                        .ToListAsync();

                    foreach(var recipe in approximateMatches)
                    {
                        if (allResults.Any(r => r.Recipe.Id == recipe.Id)) continue; // Skip duplicates

                        int wordOverlap = queryWords.Count(q => recipe.Title.ToLower().Contains(q));
                        int score = 50 + (wordOverlap * 10); // 50-80 based on overlap (e.g., 2/3 words = 70)
                        allResults.Add((recipe, score, "Similar Title"));
                    }

                    // Adaptive Related Search (Ingredients-Based)
                    var existingRecipeIds = allResults.Select(r => r.Recipe.Id).ToList();

                    var ingredientMatches = await _dbContext.Ingredients
                        .Where(i => queryWords.Any(q => i.Name.ToLower().Contains(q)))
                        .GroupBy(i => i.RecipeId)
                        .Select(g => new
                        {
                            RecipeId = g.Key,
                            MatchCount = g.Count()
                        })
                        .Join(_dbContext.Recipes,
                            g => g.RecipeId,
                            r => r.Id,
                            (g, r) => new
                            {
                                Recipe = r,
                                MatchCount = g.MatchCount
                            })
                        .Where(x => !existingRecipeIds.Contains(x.Recipe.Id))
                        .ToListAsync();

                    foreach (var match in ingredientMatches)
                    {
                        int score = 10 + (match.MatchCount * 10); // 10-50 based in ingredient matches
                        allResults.Add((match.Recipe, score, "Related by Ingredients"));
                    }
                }

                // Result Aggregation and Ranking
                var rankedResults = allResults
                    .OrderByDescending(r => r.Score)
                    .ThenBy(r => r.Recipe.Title)
                    .ToList();

                // Pagination
                int totalItems = rankedResults.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);
                int skip = (request.page - 1) * PageSize;
                var paginatedResults = rankedResults
                    .Skip(skip)
                    .Take(PageSize)
                    .Select(r => new RecipeDto
                    {
                        RecipeId = r.Recipe.Id,
                        Title = r.Recipe.Title,
                        ImageUrl = r.Recipe?.ImageUrl ?? "",
                        Relevance = r.Relevance,
                        Description = r.Recipe?.Description ?? "",
                        IsLikedByUser = r.Recipe.Likes.Any(l => l.UserId == userId),
                        LikesCount = r.Recipe.Likes.Count,
                        IsBookmarkedByUser = r.Recipe.Bookmarks.Any(b => b.UserId == userId)
                    })
                    .ToList();

                var pagination = new PaginationMetaData
                {
                    CurrentPage = request.page,
                    TotalItems = totalItems,
                    HasNext = request.page < totalPages,
                    HasPrevious = request.page > 1
                };

                // Output
                return new BaseResponse<SearchForRecipeResponse>(true, "Search result successfully retrieved", new SearchForRecipeResponse { Data = paginatedResults, Pagination = pagination });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong:\n {ex.StackTrace}: {ex.Message}");
                return new BaseResponse<SearchForRecipeResponse>(false, $"{_appSettings.ProcessingError}");
            }
        }
    }
}
