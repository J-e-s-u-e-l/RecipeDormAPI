using FluentValidation;
using MediatR;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;

namespace RecipeDormAPI.Application.CQRS.Queries
{
    public class LoginRequest : IRequest<BaseResponse<LoginResponse>>
    {
#nullable disable
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email is required").EmailAddress();
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password is required");
        }
    }
}
