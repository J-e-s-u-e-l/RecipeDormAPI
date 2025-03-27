using FluentValidation;
using MediatR;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;

namespace RecipeDormAPI.Application.CQRS.Queries
{
    public class SearchForRecipeRequest : IRequest<BaseResponse<SearchForRecipeResponse>>
    {
        public string SearchQuery { get; set; }
        public int page { get; set; }
    }

    public class SearchForRecipeRequestValidator : AbstractValidator<SearchForRecipeRequest>
    {
        public SearchForRecipeRequestValidator()
        {
            RuleFor(x => x.SearchQuery).NotEmpty().NotNull().WithMessage("Search query is required");
        }
    }
}
