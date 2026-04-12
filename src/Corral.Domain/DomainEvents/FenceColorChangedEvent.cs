// ------------------------------------------------------------------------------------------------
// <copyright file="FenceColorChangedEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.ValueObjects;

namespace Corral.Domain.DomainEvents;

/// <summary>
///   Événement déclenché quand la couleur d'une fence change
/// </summary>
public record FenceColorChangedEvent(FenceId FenceId, Color NewColor) : IDomainEvent
{
  #region Properties

  public string EventId { get; } = Guid.NewGuid().ToString();

  public DateTime OccurredAt { get; } = DateTime.UtcNow;

  #endregion
}
