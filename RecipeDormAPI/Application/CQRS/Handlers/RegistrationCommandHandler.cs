using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Commands;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, BaseResponse>
    {
        private readonly DataDbContext _dbContext;
        private UserManager<Users> _userManager;
        private readonly ILogger<RegistrationCommand> _logger;
        private readonly AppSettings _appSettings;

        public RegistrationCommandHandler(DataDbContext dbContext, UserManager<Users> userManager, ILogger<RegistrationCommand> logger, IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public async Task<BaseResponse> Handle(RegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var user = new Users()
                    {
                        Email = request.Email,
                        NormalizedEmail = request.Email!.ToUpperInvariant(),
                        UserName = request.UserName,
                        NormalizedUserName = request.UserName!.ToUpperInvariant(),
                        EmailConfirmed = false
                    };
                    await _dbContext.AddAsync(user, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    // Persist password
                    var addPassword = await _userManager.AddPasswordAsync(user, request.Password!);
                    if (!addPassword.Succeeded)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return new BaseResponse(false, _appSettings.ProcessingError);
                    }

                    await _dbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return new BaseResponse(true, _appSettings.RegistrationSuccessfully);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"REGISTRATION_HANDLER => Something went wrong\n{ex.StackTrace}: {ex.Message}");
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    return new BaseResponse(false, _appSettings.ProcessingError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"REGISTRATION_HANDLER => Something went wrong\n{ex.StackTrace}: {ex.Message}");
                return new BaseResponse(false, _appSettings.ProcessingError);
            }
        }
    }
}