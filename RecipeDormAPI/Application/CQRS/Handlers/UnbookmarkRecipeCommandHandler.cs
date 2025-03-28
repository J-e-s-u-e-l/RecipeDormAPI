using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecipeDormAPI.Application.CQRS.Commands;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class UnbookmarkRecipeCommandHandler : IRequestHandler<UnbookmarkRecipeCommand, BaseResponse>
    {
        private readonly DataDbContext _dbContext;
        private readonly ILogger<UnbookmarkRecipeCommandHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UnbookmarkRecipeCommandHandler(DataDbContext dbContext, ILogger<UnbookmarkRecipeCommandHandler> logger, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BaseResponse> Handle(UnbookmarkRecipeCommand request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext?.Items["UserId"] is not Guid userId)
            {
                _logger.LogError("UserId not found in HttpContext");
                return new BaseResponse(false, "User authentication required");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var bookmark = await _dbContext.Bookmarks.FirstOrDefaultAsync(x => x.UserId == userId && x.RecipeId == request.RecipeId, cancellationToken);

                if (bookmark == null)
                {
                    _logger.LogError($"User: {userId} does not have a bookmark for recipe: {request.RecipeId}");
                    return new BaseResponse(false, "User does not have a bookmark for this recipe");
                }

                _dbContext.Bookmarks.Remove(bookmark);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation($"Recipe: {request.RecipeId} has successfully being unbookmarked by user: {userId}");
                return new BaseResponse(true, "Recipe unbookmarked successfully");
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
