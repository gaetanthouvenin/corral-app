// ------------------------------------------------------------------------------------------------
// <copyright file="FenceDeactivatedEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;

namespace Corral.Domain.DomainEvents;

/// <summary>
///   Represents a domain event that is triggered when a fence is deactivated.
/// </summary>
public record FenceDeactivatedEvent(FenceId FenceId) : IDomainEvent
{
  #region Properties

  /// <summary>
  ///   Gets the unique identifier for this domain event.
  /// </summary>
  /// <remarks>
  ///   This identifier is automatically generated when the event is created
  ///   and is used to distinguish this event instance from others.
  /// </remarks>
  public string EventId { get; } = Guid.NewGuid().ToString();

  /// <summary>
  ///   Gets the date and time when the event occurred.
  /// </summary>
  /// <value>
  ///   The timestamp indicating when the <see cref="FenceDeactivatedEvent" /> was triggered.
  /// </value>
  public DateTime OccurredAt { get; } = DateTime.UtcNow;

  #endregion
}
