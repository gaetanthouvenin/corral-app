// ------------------------------------------------------------------------------------------------
// <copyright file="FencePositionChangedEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.ValueObjects;

namespace Corral.Domain.DomainEvents;

/// <summary>
///   Represents a domain event that is triggered when the position of a fence changes.
/// </summary>
/// <remarks>
///   This event contains the identifier of the fence whose position has changed and the new position.
///   It is used to notify the system of updates to the fence's position, enabling other components
///   to react accordingly.
/// </remarks>
public record FencePositionChangedEvent(FenceId FenceId, Position NewPosition) : IDomainEvent
{
  #region Properties

  /// <summary>
  ///   Gets the unique identifier for this domain event.
  /// </summary>
  /// <remarks>
  ///   The <see cref="EventId" /> is automatically generated as a new GUID string
  ///   when the event is created. It is used to uniquely identify this specific
  ///   instance of the event.
  /// </remarks>
  public string EventId { get; } = Guid.NewGuid().ToString();

  /// <summary>
  ///   Gets the date and time when the event occurred.
  /// </summary>
  /// <remarks>
  ///   This property is automatically set to the current UTC date and time when the event is created.
  ///   It is used to track the exact moment the event was triggered.
  /// </remarks>
  public DateTime OccurredAt { get; } = DateTime.UtcNow;

  #endregion
}
