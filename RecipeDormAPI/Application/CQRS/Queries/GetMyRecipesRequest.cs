using FluentValidation;
using MediatR;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;

namespace RecipeDormAPI.Application.CQRS.Queries
{
    public class GetMyRecipesRequest : IRequest<BaseResponse<GetMyRecipesResponse>>
    {
        public int page { get; set; }


    }

    public class GetMyRecipesRequestValidator : AbstractValidator<GetMyRecipesRequest>
    {
        public GetMyRecipesRequestValidator()
        {
            RuleFor(x => x.page).GreaterThan(0).WithMessage("Page number must be greater than 0");
        }
    }
}

