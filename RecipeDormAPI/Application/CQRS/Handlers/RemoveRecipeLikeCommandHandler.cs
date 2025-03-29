using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecipeDormAPI.Application.CQRS.Commands;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class RemoveRecipeLikeCommandHandler : IRequestHandler<RemoveRecipeLikeCommand, BaseResponse>
    {
        private readonly DataDbContext _dbContext;
        private readonly ILogger<RemoveRecipeLikeCommandHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RemoveRecipeLikeCommandHandler(DataDbContext dbContext, ILogger<RemoveRecipeLikeCommandHandler> logger, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse> Handle(RemoveRecipeLikeCommand request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext?.Items["UserId"] is not Guid userId)
            {
                _logger.LogError("UserId not found in HttpContext");
                return new BaseResponse(false, "User authentication required");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var like = await _dbContext.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.RecipeId == request.RecipeId, cancellationToken);

                if (like == null)
                {
                    return new BaseResponse(false, "You haven't liked this recipe yet.");
                }

                _dbContext.Likes.Remove(like);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return new BaseResponse(false, "Recipe unliked successfully.");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
