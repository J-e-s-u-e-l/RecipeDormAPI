using FluentValidation;
using MediatR;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;

namespace RecipeDormAPI.Application.CQRS.Queries
{
    public class GetRecipeByIdRequest : IRequest<BaseResponse<GetRecipeByIdResponse>>
    {
        public Guid RecipeId { get; set; }

        public class GetRecipeByIdRequestValidator : AbstractValidator<GetRecipeByIdRequest>
        {
            public GetRecipeByIdRequestValidator()
            {
                RuleFor(x => x.RecipeId).NotNull().NotEmpty().WithMessage("RecipeId is required");
            }
        }
    }
}
