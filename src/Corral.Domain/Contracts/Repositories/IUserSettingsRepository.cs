// ------------------------------------------------------------------------------------------------
// <copyright file="IUserSettingsRepository.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;

namespace Corral.Domain.Contracts.Repositories;

/// <summary>
///   Defines the contract for persisting <see cref="UserSettings" />.
/// </summary>
public interface IUserSettingsRepository
{
  #region Methods

  /// <summary>
  ///   Retrieves the singleton user settings row, or <c>null</c> if it does not yet exist.
  /// </summary>
  Task<UserSettings> GetAsync(CancellationToken cancellationToken = default);

  /// <summary>
  ///   Inserts or updates the singleton user settings row.
  /// </summary>
  void Upsert(UserSettings settings);

  #endregion
}
