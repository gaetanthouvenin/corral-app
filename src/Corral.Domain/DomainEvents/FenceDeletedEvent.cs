// ------------------------------------------------------------------------------------------------
// <copyright file="FenceDeletedEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;

namespace Corral.Domain.DomainEvents;

/// <summary>
///   Événement déclenché quand une fence est supprimée
/// </summary>
public record FenceDeletedEvent(FenceId FenceId) : IDomainEvent
{
  #region Properties

  public string EventId { get; } = Guid.NewGuid().ToString();

  public DateTime OccurredAt { get; } = DateTime.UtcNow;

  #endregion
}
