// ------------------------------------------------------------------------------------------------
// <copyright file="IDomainEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.DomainEvents;

/// <summary>
///   Represents the base interface for all domain events.
/// </summary>
/// <remarks>
///   Domain events are used to signal that something of interest has occurred within the domain.
///   Implementing this interface allows events to carry metadata such as a unique identifier and
///   the timestamp of when the event occurred.
/// </remarks>
public interface IDomainEvent
{
  #region Properties

  /// <summary>
  ///   Gets the unique identifier for the domain event.
  /// </summary>
  /// <remarks>
  ///   This identifier is used to distinguish this event instance from others,
  ///   ensuring traceability and uniqueness across the system.
  /// </remarks>
  string EventId { get; }

  /// <summary>
  ///   Gets the date and time when the domain event occurred.
  /// </summary>
  /// <value>
  ///   A <see cref="DateTime" /> representing the moment the event was triggered.
  /// </value>
  DateTime OccurredAt { get; }

  #endregion
}
