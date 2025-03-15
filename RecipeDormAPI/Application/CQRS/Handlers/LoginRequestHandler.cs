using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Queries;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Auth.Jwt;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class LoginRequestHandler : IRequestHandler<LoginRequest, BaseResponse<LoginResponse>>
    {
        private readonly DataDbContext _dbContext;
        private UserManager<Users> _userManager;
        private readonly ILogger<LoginRequestHandler> _logger;
        private readonly IJwtHandler _jwtHandler;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginRequestHandler(DataDbContext dbContext, UserManager<Users> userManager, ILogger<LoginRequestHandler> logger, IJwtHandler jwtHandler, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
            _jwtHandler = jwtHandler;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BaseResponse<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _dbContext.Users.Where(x => x.Email!.ToLower() == request.Email.ToLower()).FirstOrDefaultAsync(cancellationToken);

                if (user == null)
                    return new BaseResponse<LoginResponse>(false, $"{_appSettings.UserWithEmailNotFound}");

                if (user.LockoutEnabled)
                {
                    return new BaseResponse<LoginResponse>(false, _appSettings.AccountLocked);
                }
                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var verifyPswd = await _userManager.CheckPasswordAsync(user, request.Password);

                    if (!verifyPswd)
                    {
                        await _userManager.AccessFailedAsync(user);
                        int maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
                        int failedAttempts = user.AccessFailedCount;
                        if (maxAttempts - failedAttempts == 1)
                        {
                            user.LockoutEnabled = true;
                            user.LockoutEnd = DateTime.UtcNow.AddYears(1000);
                            await _dbContext.SaveChangesAsync(cancellationToken);
                            await transaction.CommitAsync();
                            return new BaseResponse<LoginResponse>(false, $"{_appSettings.AccountLocked}");
                        }

                        await _dbContext.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync();
                        _logger.LogInformation($"LOGIN_REQUEST => Process cancelled | Invalid login credentials provided: \nRequest: {request}");
                        return new BaseResponse<LoginResponse>(false, $"Invalid login credentials provided. Please try again. You have {maxAttempts - 1 - failedAttempts} login attempt(s) remaining.");
                    }

                    // reset lockout count
                    user.LockoutEnabled = false;
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = DateTimeOffset.UtcNow;
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    user.LastLogin = DateTime.UtcNow;
                    user.IsActive = true;
                    await _dbContext.SaveChangesAsync(cancellationToken);


                    var loginResponse = _jwtHandler.Create(new JwtRequest
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        EmailAddress = user.Email
                    });

                    await transaction.CommitAsync(cancellationToken);
                    _logger.LogInformation($"User signed in successfully at {DateTime.UtcNow}\nUser name: {user.UserName}\nUser ID: {user.Id}");

                    /*var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        MaxAge = TimeSpan.FromMinutes(30),
                        Path = "/"
                    };

                    _httpContextAccessor.HttpContext.Response.Cookies.Append("authToken", loginResponse.Token, cookieOptions);*/

                    return new BaseResponse<LoginResponse>(true, $"{_appSettings.SingInSuccessful}", loginResponse);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Something went wrong:\n {ex.StackTrace}: {ex.Message}");
                    await transaction.RollbackAsync(cancellationToken);
                    return new BaseResponse<LoginResponse>(false, $"{_appSettings.ProcessingError}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong:\n {ex.StackTrace}: {ex.Message}");
                return new BaseResponse<LoginResponse>(false, $"{_appSettings.ProcessingError}");
            }
        }
    }
}
