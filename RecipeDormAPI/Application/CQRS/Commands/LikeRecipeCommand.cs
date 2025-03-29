using FluentValidation;
using MediatR;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;

namespace RecipeDormAPI.Application.CQRS.Commands
{
    public class LikeRecipeCommand : IRequest<BaseResponse>
    {
        public Guid RecipeId { get; set; }

        public class LikeRecipeCommandValidator : AbstractValidator<LikeRecipeCommand>
        {
            public LikeRecipeCommandValidator()
            {
                RuleFor(x => x.RecipeId).NotNull().NotEmpty().WithMessage("RecipeId is required");
            }
        }
    }
}
