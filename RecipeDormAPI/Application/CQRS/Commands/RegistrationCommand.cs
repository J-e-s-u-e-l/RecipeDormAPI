using FluentValidation;
using MediatR;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;

namespace RecipeDormAPI.Application.CQRS.Commands
{
    /*    public class RegistrationCommand
        {
        }*/

    public class RegistrationCommand : IRequest<BaseResponse>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RegistrationCommandValidator : AbstractValidator<RegistrationCommand>
    {
        private readonly DataDbContext _dbContext;
        public RegistrationCommandValidator(DataDbContext dbContext)
        {
            _dbContext = dbContext;
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("Invalid email address.")
                .Must(BeUniqueEmail).WithMessage("Email address is already taken.");

            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty().WithMessage("Username is required.")
                .Length(3, 20).WithMessage("Username must be between 3 and 20 characters.")
                .Must(BeUniqueUsername).WithMessage("Username is already taken.");

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty().WithMessage("Password is required.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{}|;':"",<.>\/?])[A-Za-z\d!@#$%^&*()_+\-=\[\]{}|;':"",<.>\/?]{8,}$").WithMessage("Invalid password format. Your password must be at least 8 characters long and include at least one digit, one lowercase letter, one uppercase letter, and one special character.");
        }

        // checks if the username is being used by another user
        private bool BeUniqueUsername(string username)
        {
            return !_dbContext.Users.Any(u => u.UserName == username);
        }

        // checks if the Email is being used by another user 
        private bool BeUniqueEmail(string email)
        {
            return !_dbContext.Users.Any(u => u.Email == email);

        }
    }
}
