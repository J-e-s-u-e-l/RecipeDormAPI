using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Commands;
using RecipeDormAPI.Application.CQRS.Queries;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Infrastructure.Auth;

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


        public AuthController(IMediator mediator, ILogger<AuthController> logger, IOptions<AppSettings> appSettings, SignInManager<Users> signInManager)
        {
            _mediator = mediator;
            _logger = logger;
            _appSettings = appSettings.Value;
            _signInManager = signInManager;
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
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);

                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AUTH_CONTOLLER => Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

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
    }
}
