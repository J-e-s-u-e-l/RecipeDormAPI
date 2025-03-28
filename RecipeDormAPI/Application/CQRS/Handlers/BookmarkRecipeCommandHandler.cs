using MediatR;
using Microsoft.Extensions.Options;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Commands;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class BookmarkRecipeCommandHandler : IRequestHandler<BookmarkRecipeCommand, BaseResponse>
    {
        private readonly DataDbContext _dbContext;
        private readonly ILogger<BookmarkRecipeCommandHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookmarkRecipeCommandHandler(DataDbContext dbContext, ILogger<BookmarkRecipeCommandHandler> logger, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse> Handle(BookmarkRecipeCommand request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext?.Items["UserId"] is not Guid userId)
            {
                _logger.LogError("UserId not found in HttpContext");
                return new BaseResponse(false, "User authentication required");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var newRecipeBookmark = new Bookmarks
                {
                    UserId = userId,
                    RecipeId = request.RecipeId
                };

                await _dbContext.Bookmarks.AddAsync(newRecipeBookmark, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation($"Recipe: {request.RecipeId} has successfully being bookmarked by user: {userId}");
                return new BaseResponse(true, "Recipe bookmarked successfully");
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
