using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net;

namespace TextGame.Api.Middleware.Exceptions;

public class SecurityTokenExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is SecurityTokenException exception)
        {
            context.Result = new ObjectResult(exception.Message) // TODO (Roman): don't show this
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };

            context.ExceptionHandled = true;
        }
    }
}

