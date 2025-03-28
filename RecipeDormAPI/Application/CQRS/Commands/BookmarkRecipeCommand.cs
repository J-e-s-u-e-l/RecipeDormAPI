using FluentValidation;
using MediatR;
using RecipeDormAPI.Application.CQRS.Queries;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;

namespace RecipeDormAPI.Application.CQRS.Commands
{
    public class BookmarkRecipeCommand : IRequest<BaseResponse>
    {
        public Guid RecipeId { get; set; }

        public class BookmarkRecipeCommandValidator : AbstractValidator<BookmarkRecipeCommand>
        {
            public BookmarkRecipeCommandValidator()
            {
                RuleFor(x => x.RecipeId).NotNull().NotEmpty().WithMessage("RecipeId is required");
            }
        }
    }
}
