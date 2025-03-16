using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RecipeDormAPI.Application.CQRS.Queries
{
    public class GoogleSignInRequest : IRequest<IActionResult> { }
}
