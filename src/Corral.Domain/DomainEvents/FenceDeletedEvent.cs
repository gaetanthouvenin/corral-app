// ------------------------------------------------------------------------------------------------
// <copyright file="FenceDeletedEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;

namespace Corral.Domain.DomainEvents;

/// <summary>
///   Represents a domain event that is triggered when a fence is deleted.
/// </summary>
/// <remarks>
///   This event contains the identifier of the deleted fence and metadata such as the event ID and the
///   timestamp
///   when the event occurred. It is used to signal that a fence has been removed, allowing other parts
///   of the
///   application to react accordingly.
/// </remarks>
public record FenceDeletedEvent(FenceId FenceId) : IDomainEvent
{
  #region Properties

  /// <summary>
  ///   Gets the unique identifier for this domain event.
  /// </summary>
  /// <remarks>
  ///   The <see cref="EventId" /> is automatically generated as a new GUID string
  ///   when the event is instantiated. It is used to uniquely identify this specific
  ///   instance of the event.
  /// </remarks>
  public string EventId { get; } = Guid.NewGuid().ToString();

  /// <summary>
  ///   Gets the timestamp indicating when the event occurred.
  /// </summary>
  /// <remarks>
  ///   This property is automatically set to the current UTC time when the event is created.
  ///   It provides a consistent way to track the timing of domain events.
  /// </remarks>
  public DateTime OccurredAt { get; } = DateTime.UtcNow;

  #endregion
}
