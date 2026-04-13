// ------------------------------------------------------------------------------------------------
// <copyright file="LoggingBehavior.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Diagnostics;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Corral.Application.Behaviors;

/// <summary>
///   Pipeline MediatR qui logue l'exécution de chaque command et query.
///   Enregistre le nom de la requête, sa durée d'exécution et les erreurs éventuelles.
/// </summary>
public class LoggingBehavior<TRequest, TResponse>(
  ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IBaseRequest
{
  #region Implementation of IPipelineBehavior<TRequest,TResponse>

  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
  {
    var requestName = typeof(TRequest).Name;

    logger.LogInformation("Handling {RequestName}", requestName);

    var stopwatch = Stopwatch.StartNew();

    try
    {
      var response = await next();
      stopwatch.Stop();

      logger.LogInformation(
        "Handled {RequestName} in {ElapsedMs}ms",
        requestName,
        stopwatch.ElapsedMilliseconds
      );

      return response;
    }
    catch (Exception ex)
    {
      stopwatch.Stop();

      logger.LogError(
        ex,
        "Error handling {RequestName} after {ElapsedMs}ms",
        requestName,
        stopwatch.ElapsedMilliseconds
      );

      throw;
    }
  }

  #endregion
}
