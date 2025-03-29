using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Commands;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class LikeRecipeCommandHandler : IRequestHandler<LikeRecipeCommand, BaseResponse>
    {
        private readonly DataDbContext _dbContext;
        private readonly ILogger<LikeRecipeCommandHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LikeRecipeCommandHandler(DataDbContext dbContext, ILogger<LikeRecipeCommandHandler> logger, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse> Handle(LikeRecipeCommand request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext?.Items["UserId"] is not Guid userId)
            {
                _logger.LogError("UserId not found in HttpContext");
                return new BaseResponse(false, "User authentication required");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var recipeExists = await _dbContext.Recipes.AnyAsync(cancellationToken);

                if (!recipeExists)
                {
                    _logger.LogError($"User: {userId} does not have a bookmark for recipe: {request.RecipeId}");
                    return new BaseResponse(false, "Recipe not found");
                }

                var newLike = new Likes
                {
                    UserId = userId,
                    RecipeId = request.RecipeId,
                };

                await _dbContext.Likes.AddAsync(newLike, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation($"Recipe: {request.RecipeId} has successfully being liked by user: {userId}");
                return new BaseResponse(true, "Recipe liked successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n{ex.StackTrace}: {ex.Message}");
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                return new BaseResponse(false, _appSettings.ProcessingError);
            }
        }
    }
}
