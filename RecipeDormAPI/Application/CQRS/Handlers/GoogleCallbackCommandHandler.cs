using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Commands;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Auth.Jwt;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;
using System.Security.Claims;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    //public class GoogleCallbackCommandHandler : IRequestHandler<GoogleCallbackCommand, IActionResult>
    //public class GoogleCallbackCommandHandler : IRequestHandler<GoogleCallbackCommand, object>
    public class GoogleCallbackCommandHandler : IRequestHandler<GoogleCallbackCommand, BaseResponse<LoginResponse>>
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        private readonly IJwtHandler _jwtHandler;
        private readonly DataDbContext _dbContext;
        private readonly ILogger<GoogleCallbackCommand> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoogleCallbackCommandHandler(SignInManager<Users> signInManager, UserManager<Users> userManager, IJwtHandler jwtHandler, DataDbContext dbContext, ILogger<GoogleCallbackCommand> logger, IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        //public async Task<IActionResult> Handle(GoogleCallbackCommand request, CancellationToken cancellationToken)
        public async Task<BaseResponse<LoginResponse>> Handle(GoogleCallbackCommand request, CancellationToken cancellationToken)
        {

            try
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var info = await _signInManager.GetExternalLoginInfoAsync();

                    if (info == null)
                    {
                        //return new BadRequestObjectResult(_appSettings.ExternalLoginError);
                        return new BaseResponse<LoginResponse>(false, _appSettings.ExternalLoginError);
                    }

                    var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                    if (user == null)
                    {
                        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                        if (string.IsNullOrEmpty(email))
                        {
                            //return new BadRequestObjectResult(_appSettings.ExternalLoginError);
                            return new BaseResponse<LoginResponse>(false, _appSettings.ExternalLoginError);

                        }

                        user = new Users()
                        {
                            Email = email,
                            NormalizedEmail = email!.ToUpperInvariant(),
                            UserName = email,
                            NormalizedUserName = email!.ToUpperInvariant(),
                            EmailConfirmed = false,
                            SecurityStamp = Guid.NewGuid().ToString()
                        };

                        await _dbContext.AddAsync(user, cancellationToken);
                        await _dbContext.SaveChangesAsync(cancellationToken);

                        await _userManager.AddLoginAsync(user, info);
                    }

                    // Sign in user
                    //await _signInManager.SignInAsync(user, false);


                    var loginResponse = _jwtHandler.Create(new JwtRequest
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        EmailAddress = user.Email
                    });

                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

                    /*_httpContextAccessor.HttpContext.Response.Cookies.Append(
                        "authToken",
                        loginResponse.Token,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.TryParse(loginResponse.Expires, out var expires) ? expires : (DateTimeOffset?)null
                        }
                    );*/

                    /*return await Task.FromResult<IActionResult>(new ObjectResult(new BaseResponse<LoginResponse>(true, _appSettings.SignInSuccessful, new LoginResponse
                    {
                        Token = loginResponse.Token,
                        Expires = loginResponse.Expires,
                        Username = user.UserName,
                        Email = user.Email,
                        UserId = user.Id
                    }))
                    {
                        StatusCode = StatusCodes.Status200OK
                    });*/
                    return new BaseResponse<LoginResponse>(true, _appSettings.SignInSuccessful, new LoginResponse
                    {
                        Token = loginResponse.Token,
                        Expires = loginResponse.Expires,
                        Username = user.UserName,
                        Email = user.Email,
                        UserId = user.Id
                    });


                    //return new RedirectResult(_appSettings.HomePage);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"REGISTRATION_HANDLER => Something went wrong\n{ex.StackTrace}: {ex.Message}");
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    /*return await Task.FromResult<IActionResult>(new ObjectResult(new BaseResponse(false, _appSettings.ProcessingError))
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    });*/
                    return new BaseResponse<LoginResponse>(false, _appSettings.ProcessingError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"REGISTRATION_HANDLER => Something went wrong\n{ex.StackTrace}: {ex.Message}");
                /*return await Task.FromResult<IActionResult>(new ObjectResult(new BaseResponse(false, _appSettings.ProcessingError))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                });*/
                return new BaseResponse<LoginResponse>(false, _appSettings.ProcessingError);
            }
        }
    }
}
