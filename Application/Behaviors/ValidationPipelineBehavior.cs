using FluentValidation;
using MediatR;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResult =
                await Task.WhenAll(_validators.Select(validator =>
                    validator.ValidateAsync(context, cancellationToken)));

            var failures = validationResult.Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToArray();
            if (failures.Length > 0)
            {
                var errors = failures
                    .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
                    .ToDictionary(k => k.Key, v => v.ToArray());

                throw new ValidationException(errors);
            }
        }

        return await next();
    }
}