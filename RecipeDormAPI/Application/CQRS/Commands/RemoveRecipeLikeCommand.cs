using FluentValidation;
using MediatR;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;

namespace RecipeDormAPI.Application.CQRS.Commands
{
    public class RemoveRecipeLikeCommand : IRequest<BaseResponse>
    {
        public Guid RecipeId { get; set; }

        public class RemoveRecipeLikeCommandValidator : AbstractValidator<RemoveRecipeLikeCommand>
        {
            public RemoveRecipeLikeCommandValidator()
            {
                RuleFor(x => x.RecipeId).NotNull().NotEmpty().WithMessage("RecipeId is required");
            }
        }
    }
}
