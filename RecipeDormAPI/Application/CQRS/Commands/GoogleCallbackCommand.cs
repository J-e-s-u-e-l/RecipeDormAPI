using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;

namespace RecipeDormAPI.Application.CQRS.Commands
{
    //public class GoogleCallbackCommand : IRequest<IActionResult> { }
    public class GoogleCallbackCommand : IRequest<BaseResponse<LoginResponse>> { }
    //public class GoogleCallbackCommand : IRequest<object> { }
}
