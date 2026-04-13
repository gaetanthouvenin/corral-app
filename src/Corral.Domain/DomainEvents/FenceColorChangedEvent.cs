// ------------------------------------------------------------------------------------------------
// <copyright file="FenceColorChangedEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.ValueObjects;

namespace Corral.Domain.DomainEvents;

/// <summary>
///   Represents a domain event that is triggered when the color of a fence is changed.
/// </summary>
/// <remarks>
///   This event contains the unique identifier of the fence whose color has changed
///   and the new color that has been applied. It is used to notify other parts of the
///   system about this change.
/// </remarks>
public record FenceColorChangedEvent(FenceId FenceId, Color NewColor) : IDomainEvent
{
  #region Properties

  /// <summary>
  ///   Gets the unique identifier for this domain event.
  /// </summary>
  /// <remarks>
  ///   The <see cref="EventId" /> is automatically generated as a new GUID
  ///   when the event is created, ensuring uniqueness across all events.
  /// </remarks>
  public string EventId { get; } = Guid.NewGuid().ToString();

  /// <summary>
  ///   Gets the date and time at which the event occurred.
  /// </summary>
  /// <remarks>
  ///   This property is automatically set to the current UTC date and time when the event is created.
  ///   It is used to track the exact moment the event was triggered.
  /// </remarks>
  public DateTime OccurredAt { get; } = DateTime.UtcNow;

  #endregion
}
