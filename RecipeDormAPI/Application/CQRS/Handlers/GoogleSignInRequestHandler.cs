using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Queries;
using RecipeDormAPI.Controllers;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class GoogleSignInRequestHandler : IRequestHandler<GoogleSignInRequest, IActionResult>
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /*public GoogleSignInRequestHandler(SignInManager<Users> signInManager, IUrlHelper urlHelperRFactory)
        {
            _signInManager = signInManager;
            _urlHelper = urlHelperRFactory;
        }*/

        public GoogleSignInRequestHandler(SignInManager<Users> signInManager, IUrlHelperFactory urlHelperRFactory, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _urlHelperFactory = urlHelperRFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<IActionResult> Handle(GoogleSignInRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /* public Task<IActionResult> Handle(GoogleSignInRequest request, CancellationToken cancellationToken)
         {
             var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext
             {
                 HttpContext = _httpContextAccessor.HttpContext,
                 RouteData = _httpContextAccessor.HttpContext?.GetRouteData(),
                 ActionDescriptor = new ActionDescriptor()
             });

             var redirectUrl = urlHelper.Action(nameof(AuthController.GoogleCallback), "Auth", null, _httpContextAccessor..Request.Scheme);
             var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);

             return Task.FromResult<IActionResult>(new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties));
         }*/
    }
}
