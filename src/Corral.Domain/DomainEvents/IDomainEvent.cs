// ------------------------------------------------------------------------------------------------
// <copyright file="IDomainEvent.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.DomainEvents;

/// <summary>
///   Interface de base pour tous les événements domaine
/// </summary>
public interface IDomainEvent
{
  #region Properties

  /// <summary>
  ///   Identifiant unique de l'événement
  /// </summary>
  string EventId { get; }

  /// <summary>
  ///   Date et heure de création de l'événement
  /// </summary>
  DateTime OccurredAt { get; }

  #endregion
}
