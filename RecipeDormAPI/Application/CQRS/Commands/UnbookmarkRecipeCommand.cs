using FluentValidation;
using MediatR;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;

namespace RecipeDormAPI.Application.CQRS.Commands
{
    public class UnbookmarkRecipeCommand : IRequest<BaseResponse>
    {
        public Guid RecipeId { get; set; }

        public class UnbookmarkRecipeCommandValidator : AbstractValidator<UnbookmarkRecipeCommand>
        {
            public UnbookmarkRecipeCommandValidator()
            {
                RuleFor(x => x.RecipeId).NotNull().NotEmpty().WithMessage("RecipeId is required");
            }
        }
    }
}
