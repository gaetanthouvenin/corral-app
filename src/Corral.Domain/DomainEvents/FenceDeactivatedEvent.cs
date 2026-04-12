// ------------------------------------------------------------------------------------------------
// <copyright file="FenceDeactivatedEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;

namespace Corral.Domain.DomainEvents;

/// <summary>
///   Événement déclenché quand une fence est désactivée
/// </summary>
public record FenceDeactivatedEvent(FenceId FenceId) : IDomainEvent
{
  #region Properties

  public string EventId { get; } = Guid.NewGuid().ToString();

  public DateTime OccurredAt { get; } = DateTime.UtcNow;

  #endregion
}
