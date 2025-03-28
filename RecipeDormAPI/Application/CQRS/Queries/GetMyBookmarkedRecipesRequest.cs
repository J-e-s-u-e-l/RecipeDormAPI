using FluentValidation;
using MediatR;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;

namespace RecipeDormAPI.Application.CQRS.Queries
{
    public class GetMyBookmarkedRecipesRequest : IRequest<BaseResponse<GetMyBookmarkedRecipesResponse>>
    {
        public int PageNumber { get; set; }
    }

    public class GetMyBookmarkedRecipesRequestValidator : AbstractValidator<GetMyBookmarkedRecipesRequest>
    {
        public GetMyBookmarkedRecipesRequestValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0");
        }
    }
}
