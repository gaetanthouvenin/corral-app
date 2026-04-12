// ------------------------------------------------------------------------------------------------
// <copyright file="FencePositionChangedEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.ValueObjects;

namespace Corral.Domain.DomainEvents;

/// <summary>
///   Événement déclenché quand la position d'une fence change
/// </summary>
public record FencePositionChangedEvent(FenceId FenceId, Position NewPosition) : IDomainEvent
{
  #region Properties

  public string EventId { get; } = Guid.NewGuid().ToString();

  public DateTime OccurredAt { get; } = DateTime.UtcNow;

  #endregion
}
