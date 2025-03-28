using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecipeDormAPI.Application.CQRS.Queries;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.DTOs;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;
using RecipeDormAPI.Infrastructure.Infrastructure.Services.Interfaces;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class GetMyRecipesRequestHandler : IRequestHandler<GetMyRecipesRequest, BaseResponse<GetMyRecipesResponse>>
    {
        private readonly DataDbContext _dbContext;
        private readonly ILogger<GetMyRecipesRequestHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int PageSize = 20;

        public GetMyRecipesRequestHandler(DataDbContext dbContext, ILogger<GetMyRecipesRequestHandler> logger, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse<GetMyRecipesResponse>> Handle(GetMyRecipesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (_httpContextAccessor.HttpContext?.Items["UserId"] is not Guid userId)
                {
                    _logger.LogError("UserId not found in HttpContext");
                    return new BaseResponse<GetMyRecipesResponse>(false, "User authentication required");
                }

                var recipes = await _dbContext.Recipes
                    .Where(r => r.UserId == userId)
                    .Select(r => new RecipeDto
                    {
                        RecipeId = r.Id,
                        Title = r.Title,
                        ImageUrl = r.ImageUrl,
                        Description = r.Description
                    })
                    .ToListAsync(cancellationToken);

                // Pagination
                int totalRecipes = recipes.Count;
                int totalPages = (int)Math.Ceiling((double)totalRecipes / PageSize);
                int skip = (request.page - 1) * PageSize;
                recipes = recipes
                            .Skip(skip)
                            .Take(PageSize)
                            .Select(r => new RecipeDto
                            {
                                RecipeId = r.RecipeId,
                                Title = r.Title,
                                ImageUrl = r.ImageUrl,
                                Description = r.Description
                            })
                            .ToList();

                var response = new GetMyRecipesResponse
                {
                    Recipes = recipes,
                    Pagination = new PaginationMetaData
                    {
                        CurrentPage = request.page,
                        TotalItems = totalRecipes,
                        HasNext = request.page < totalPages,
                        HasPrevious = request.page > 1
                    }
                };

                _logger.LogInformation($"User {userId} successfully retrieved personally created recipes");
                return new BaseResponse<GetMyRecipesResponse>(true, "Recipes fetched successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n{ex.StackTrace}: {ex.Message}");
                return new BaseResponse<GetMyRecipesResponse>(false, _appSettings.ProcessingError);
            }

        }
    }
}
