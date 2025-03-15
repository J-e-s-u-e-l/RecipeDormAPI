using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeDormAPI.Infrastructure.Data.Models.Responses
{
    public class ValidationResultModel
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Expires { get; set; }
    }


    public class JwtRequest
    {
#nullable disable
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public Guid UserId { get; set; }
    }
}
