using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeAPI.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

//using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RecipeDormAPI.Infrastructure.Infrastructure.Auth.Jwt
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
               /* context.Request.EnableBuffering(); // Allow reading request body multiple times

                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                _logger.LogInformation("Incoming Request: {RequestBody}", body);
*/
                var token = context.Request.Cookies["authToken"];

                if (token != null)
                    await attachUserToContext(context, token);
                else
                {
                    token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    if (token != null)
                        await attachUserToContext(context, token);
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"JWT_MIDDLEWARE => Something went wrong\n{ex.StackTrace}: {ex.Message}");
            }
        }

        private async Task attachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "sub").Value);

                context.Items["UserId"] = userId;

                var identity = context.User.Identity as ClaimsIdentity;

                if (!identity.HasClaim(c => c.Type == "UserId"))
                {
                    identity.AddClaim(new Claim("UserId", userId.ToString()));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"JWT_MIDDLEWARE => Something went wrong\n{ex.StackTrace}: {ex.Message}");
            }
        }
    }
}
