using Azure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecipeDormAPI.Application.CQRS.Queries;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.DTOs;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class GetRecipeByIdRequestHandler : IRequestHandler<GetRecipeByIdRequest, BaseResponse<GetRecipeByIdResponse>>
    {
        private readonly DataDbContext _dbContext;
        private readonly ILogger<GetRecipeByIdRequestHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetRecipeByIdRequestHandler(DataDbContext dbContext, ILogger<GetRecipeByIdRequestHandler> logger, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse<GetRecipeByIdResponse>> Handle(GetRecipeByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (_httpContextAccessor.HttpContext?.Items["UserId"] is not Guid userId)
                {
                    _logger.LogError("UserId not found in HttpContext");
                    return new BaseResponse<GetRecipeByIdResponse>(false, "User authentication required");
                }

                var recipe = await _dbContext.Recipes
                    .Where(r => r.Id == request.RecipeId)
                    .Select(r => new GetRecipeByIdResponse
                    {
                        RecipeDetails = new RecipeDto
                        {
                            RecipeId = r.Id,
                            Title = r.Title,
                            ImageUrl = r.ImageUrl,
                            Description = r.Description,
                            IsLikedByUser = r.Likes.Any(l => l.UserId == userId),
                            LikesCount = r.Likes.Count,
                            IsBookmarkedByUser = r.Bookmarks.Any(b => b.UserId == userId)
                        },
                        Ingredients = r.Ingredients.Select(i => new IngredientsDto { Name = i.Name, Quantity = i.Quantity }).ToList(),
                        Steps = r.Steps.Select(i => new StepsDto { StepNumber = i.StepNumber, Description= i.Description}).OrderBy(x => x.StepNumber).ToList()
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (recipe == null)
                {
                    return new BaseResponse<GetRecipeByIdResponse>(false, "Recipe not found");
                }

                _logger.LogInformation($"User: {userId} successfully retrieved recipe: {request.RecipeId}");
                return new BaseResponse<GetRecipeByIdResponse>(true, "Recipes fetched successfully", recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n{ex.StackTrace}: {ex.Message}");
                return new BaseResponse<GetRecipeByIdResponse>(false, _appSettings.ProcessingError);
            }
        }
    }
}
