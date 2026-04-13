// ------------------------------------------------------------------------------------------------
// <copyright file="FenceCreatedEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;

namespace Corral.Domain.DomainEvents;

/// <summary>
///   Represents a domain event that is triggered when a fence is created.
/// </summary>
public record FenceCreatedEvent(FenceId FenceId, string Name) : IDomainEvent
{
  #region Properties

  /// <summary>
  ///   Gets the unique identifier for the domain event.
  /// </summary>
  /// <value>
  ///   A <see cref="string" /> representing the unique identifier of the event.
  /// </value>
  public string EventId { get; } = Guid.NewGuid().ToString();

  /// <summary>
  ///   Gets the date and time when the event occurred.
  /// </summary>
  /// <value>
  ///   A <see cref="DateTime" /> representing the UTC timestamp of the event occurrence.
  /// </value>
  public DateTime OccurredAt { get; } = DateTime.UtcNow;

  #endregion
}
