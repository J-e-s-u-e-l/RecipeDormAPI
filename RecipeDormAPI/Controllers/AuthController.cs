using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Commands;
using RecipeDormAPI.Application.CQRS.Queries;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Auth;
using RecipeDormAPI.Infrastructure.Infrastructure.Auth.Jwt;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading;

namespace RecipeDormAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        ILogger<AuthController> _logger;
        private readonly AppSettings _appSettings;
        private readonly SignInManager<Users> _signInManager;
        private readonly DataDbContext _dbContext;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Users> _userManager;
        private readonly IJwtHandler _jwtHandler;
        private readonly IConfiguration _configuration;

        public AuthController(IMediator mediator, ILogger<AuthController> logger, IOptions<AppSettings> appSettings, SignInManager<Users> signInManager, DataDbContext dataDbContext, IJwtHandler jwtHandler, IConfiguration configuration)
        {
            _mediator = mediator;
            _logger = logger;
            _appSettings = appSettings.Value;
            _signInManager = signInManager;
            _dbContext = dataDbContext;
            _jwtHandler = jwtHandler;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser(RegistrationCommand request)
        {
            try
            {
                var modelxfmed = new RegistrationCommand { Email = request.Email, UserName = request.UserName, };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"REGISTRATION_CONTROLLER => User attempt to REGISTER \n{req}");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"REGISTRATION_CONTROLLER => Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(LoginRequest request)
        {
            try
            {
                var modelxfmed = new LoginRequest { Email = request.Email };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"AUTH_CONTROLLER => User attempt to LOGIN \n{req}");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AUTH_CONTOLLER => Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        [HttpGet("google-sign-in")]
        public async Task<IActionResult> GoogleSignIn()
        {
            try
            {
                //var modelxfmed = new LoginRequest { Email = request.Email };
                //var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"AUTH_CONTROLLER => User attempt to LOGIN using Google Auth");
                //var response = await _mediator.Send(new GoogleSignInRequest());


                var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, HttpContext.Request.Scheme);
                //var redirectUrl = _appSettings.HomePage;
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);

                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AUTH_CONTOLLER => Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        #region End of the wall of my attempt to be more flexible with Google Callback

       /* [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state)
        {
            try
            {
                _logger.LogInformation($"AUTH_CONTROLLER => User attempt to access Google Callback");
                
                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
                {
                    _logger.LogError("AUTH_CONTROLLER => Missing code or state parameter.");
                    return BadRequest(new { Error = "Missing requeired parameters" });
                }

                // Exchange the authorization code for an access token
                var tokenResponse = await ExchangeCodeForToken(code);
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.IdToken))
                {
                    _logger.LogError("AUTH_CONTROLLER => Failed to exchange code for token.");
                    return StatusCode(500, new { Error = "Failed to authenticate with Google" });
                }

                *//*var userInfo = await GetUserInfoFromToken(tokenResponse.IdToken);
                if (userInfo == null)
                {
                    _logger.LogError("AUTH_CONTROLLER => Failed to get user info.");
                    return StatusCode(500, new { Error = "Failed to retrieve user info" });
                }*//*

                var info = await _signInManager.GetExternalLoginInfoAsync();

                //var user = await CreateOrGetUser(userInfo);
                //var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);


                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                if (user == null)
                {
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    if (string.IsNullOrEmpty(email))
                    {
                        return new BadRequestObjectResult(_appSettings.ExternalLoginError);

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

                    await _dbContext.AddAsync(user);
                    await _dbContext.SaveChangesAsync();

                    await _userManager.AddLoginAsync(user, info);
                }

                var loginResponse = _jwtHandler.Create(new JwtRequest
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    EmailAddress = user.Email
                });

                //await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);


                *//*return new BaseResponse<LoginResponse>(true, _appSettings.SignInSuccessful, new LoginResponse
                {
                    Token = loginResponse.Token,
                    Expires = loginResponse.Expires,
                    Username = user.UserName,
                    Email = user.Email,
                    UserId = user.Id
                });*//*

                return Ok(new
                {
                    Token = loginResponse.Token,
                    Expires = loginResponse.Expires,
                    Username = user.UserName,
                    Email = user.Email,
                    UserId = user.Id
                });

            }
            catch (Exception ex)
            {
                _logger.LogError($"AUTH_CONTOLLER => Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }*/

        // Helper method to exchange the code for a token
        /*private async Task<GoogleTokenResponse> ExchangeCodeForToken(string code)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "code", code },
                        { "client_id", _configuration["Google:ClientId"] },
                        { "client_secret", _configuration["Google:ClientSecret"]  }, 
                        { "redirect_uri", "https://localhost:7160/api/auth/google-callback" },
                        { "grant_type", "authorization_code" }
                    })
            };

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("AUTH_CONTROLLER => Token exchange failed: {Status}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<GoogleTokenResponse>(json);
        }

        // Helper method to get user info from the ID token
        private async Task<GoogleUserInfo> GetUserInfoFromToken(string idToken)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("AUTH_CONTROLLER => Failed to validate ID token: {Status}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<GoogleUserInfo>(json);
        }

        // DTOs for deserialization
        public class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            [JsonPropertyName("id_token")]
            public string IdToken { get; set; }
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
        }

        public class GoogleUserInfo
        {
            [JsonPropertyName("sub")]
            public string Sub { get; set; } // Google user ID
            [JsonPropertyName("email")]
            public string Email { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }*/

        #endregion


        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            try
            {
                _logger.LogInformation($"AUTH_CONTROLLER => User attempt to access Google Callback");
                var response = await _mediator.Send(new GoogleCallbackCommand());

                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError($"AUTH_CONTOLLER => Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        /*[HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            try
            {
                _logger.LogInformation($"AUTH_CONTROLLER => User attempt to access Google Callback");
                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                var info = await _signInManager.GetExternalLoginInfoAsync();

                if (info == null)
                {
                    return new BadRequestObjectResult(_appSettings.ExternalLoginError);
                }

                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                if (user == null)
                {
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    if (string.IsNullOrEmpty(email))
                    {
                        return new BadRequestObjectResult(_appSettings.ExternalLoginError);
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

                    await _dbContext.AddAsync(user);
                    await _dbContext.SaveChangesAsync();

                    await _userManager.AddLoginAsync(user, info);
                }


                var loginResponse = _jwtHandler.Create(new JwtRequest
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    EmailAddress = user.Email
                });

                await transaction.CommitAsync().ConfigureAwait(false);

                Response.Cookies.Append(
                    "authToken",
                    loginResponse.Token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        //SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.TryParse(loginResponse.Expires, out var expires) ? expires : (DateTimeOffset?)null
                    }
                );

                return Redirect(_appSettings.HomePage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AUTH_CONTOLLER => Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }*/
    }
}