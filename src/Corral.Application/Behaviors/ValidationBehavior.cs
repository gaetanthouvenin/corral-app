// ------------------------------------------------------------------------------------------------
// <copyright file="ValidationBehavior.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Corral.Application.Behaviors;

/// <summary>
///   Pipeline MediatR qui valide chaque requête via FluentValidation avant de l'envoyer au handler.
///   Lève une <see cref="ValidationException" /> si une ou plusieurs règles ne sont pas respectées.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(
  IEnumerable<IValidator<TRequest>> validators,
  ILogger<ValidationBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IBaseRequest
{
  #region Implementation of IPipelineBehavior<TRequest,TResponse>

  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
  {
    if (!validators.Any())
    {
      return await next();
    }

    var requestName = typeof(TRequest).Name;

    var context = new ValidationContext<TRequest>(request);

    var failures = validators.Select(v => v.Validate(context))
                             .SelectMany(result => result.Errors)
                             .Where(f => f != null)
                             .ToList();

    if (failures.Count == 0)
    {
      return await next();
    }

    logger.LogWarning(
      "Validation failed for {RequestName} with {ErrorCount} error(s): {Errors}",
      requestName,
      failures.Count,
      string.Join("; ", failures.Select(f => f.ErrorMessage))
    );

    throw new ValidationException(failures);
  }

  #endregion
}
