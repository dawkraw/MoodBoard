using Application.Exceptions;
using Microsoft.AspNetCore.SignalR;

namespace Web.Filters;

public class BoardHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (ValidationException ex)
        {
            return ex.Errors;
        }
    }
}