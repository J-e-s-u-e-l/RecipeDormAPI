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
    public class GetMyBookmarkedRecipesRequestHandler : IRequestHandler<GetMyBookmarkedRecipesRequest, BaseResponse<GetMyBookmarkedRecipesResponse>>
    {
        private readonly DataDbContext _dbContext;
        private readonly ILogger<GetMyBookmarkedRecipesRequestHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int PageSize = 20;

        public GetMyBookmarkedRecipesRequestHandler(DataDbContext dbContext, ILogger<GetMyBookmarkedRecipesRequestHandler> logger, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse<GetMyBookmarkedRecipesResponse>> Handle(GetMyBookmarkedRecipesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (_httpContextAccessor.HttpContext?.Items["UserId"] is not Guid userId)
                {
                    _logger.LogError("UserId not found in HttpContext");
                    return new BaseResponse<GetMyBookmarkedRecipesResponse>(false, "User authentication required");
                }

                int skip = (request.PageNumber - 1) * PageSize;

                var query = _dbContext.Bookmarks
                    .Where(b => b.UserId == userId)
                    .Select(b => new RecipeDto
                    {
                        RecipeId = b.RecipeId,
                        Title = b.Recipe.Title ?? "Unknow Title",
                        ImageUrl = b.Recipe.ImageUrl ?? "",
                        Description = b.Recipe.Description ?? "",
                        IsLikedByUser = b.Recipe.Likes.Any(l => l.UserId == userId),
                        LikesCount = b.Recipe.Likes.Count,
                        IsBookmarkedByUser = true,
                    });

                int totalRecipes = await query.CountAsync(cancellationToken);

                var myBookmarkedRecipes = await query
                    .Skip(skip)
                    .Take(PageSize)
                    .ToListAsync(cancellationToken);

                int totalPages = (int)Math.Ceiling((double)totalRecipes / PageSize);
                var pagination = new PaginationMetaData
                {
                    TotalItems = totalRecipes,
                    CurrentPage = request.PageNumber,
                    HasNext = request.PageNumber < totalPages,
                    HasPrevious = request.PageNumber > 1
                };

                var response = new GetMyBookmarkedRecipesResponse
                {
                    Recipes = myBookmarkedRecipes,
                    Pagination = pagination
                };

                _logger.LogInformation($"User {userId} retrieved {myBookmarkedRecipes.Count} bookmarked recipes for page {request.PageNumber}");
                return new BaseResponse<GetMyBookmarkedRecipesResponse>(true, "My bookmarked recipes successfully retrieved", response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong:\n {ex.StackTrace}: {ex.Message}");
                return new BaseResponse<GetMyBookmarkedRecipesResponse>(false, $"{_appSettings.ProcessingError}");
            }
        }
    }
}
