using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidationException = FluentValidation.ValidationException;

namespace Web.Filters;

public class ExceptionFilter : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public ExceptionFilter()
    {
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            {typeof(ValidationException), HandleValidationException},
            {typeof(FluentValidationException), HandleFluentValidationException}
        };
    }


    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetType();
        if (_exceptionHandlers.ContainsKey(exceptionType))
        {
            _exceptionHandlers[exceptionType].Invoke(context);
            return;
        }

        if (!context.ModelState.IsValid)
        {
            HandleInvalidStateException(context);
        }
    }

    private void HandleInvalidStateException(ExceptionContext context)
    {
        var info = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Invalid input"
        };
        context.Result = new BadRequestObjectResult(info);

        context.ExceptionHandled = true;
    }

    private void HandleValidationException(ExceptionContext context)
    {
        if (context.Exception is not ValidationException exception) return;
        var info = new ValidationProblemDetails(exception.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Invalid input"
        };
        context.Result = new BadRequestObjectResult(info);
        context.ExceptionHandled = true;
    }

    private void HandleFluentValidationException(ExceptionContext context)
    {
        var exception = context.Exception as FluentValidationException;
        context.Result = new BadRequestObjectResult(exception);

        context.ExceptionHandled = true;
    }
}