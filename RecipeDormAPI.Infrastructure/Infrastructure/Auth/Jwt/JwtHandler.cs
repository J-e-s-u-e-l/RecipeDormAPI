using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RecipeDormAPI.Infrastructure.Infrastructure.Auth.Jwt
{
    /*public class JwtHandler
    {

    }
*/

    public interface IJwtHandler
    {
        LoginResponse Create(JwtRequest request);
    }

    public class JwtHandler : IJwtHandler
    {
        private readonly SecurityKey _issuerSigningKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly IConfiguration _configuration;

        public JwtHandler(IOptions<JwtSettings> options, IConfiguration configuration)
        {
            _configuration = configuration;
            _issuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!));
            _signingCredentials = new SigningCredentials(_issuerSigningKey, SecurityAlgorithms.HmacSha256);
            _configuration = configuration;

        }

        public LoginResponse Create(JwtRequest request)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);
            var nowUtc = DateTime.UtcNow;
            var expires = nowUtc.AddDays(7);
            var claims = new List<Claim>
            {
                new Claim("sub", request.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, nowUtc.ToString(), ClaimValueTypes.Integer64),
                new Claim("unique_name", request.EmailAddress)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                //Issuer = issuer,
                //Audience = audience,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);


            return new LoginResponse
            {
                UserId = request.UserId,
                Username = request.Username,
                Email = request.EmailAddress,
                Token = jwtToken,
                Expires = expires.ToString(),
            };
        }
    }
}
