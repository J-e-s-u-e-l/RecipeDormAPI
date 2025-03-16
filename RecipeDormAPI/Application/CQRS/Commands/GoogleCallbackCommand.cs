using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RecipeDormAPI.Application.CQRS.Commands
{
    public class GoogleCallbackCommand : IRequest<IActionResult> { }
}
