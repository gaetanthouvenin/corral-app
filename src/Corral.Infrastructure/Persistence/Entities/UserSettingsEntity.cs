// ------------------------------------------------------------------------------------------------
// <copyright file="UserSettingsEntity.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Infrastructure.Persistence.Entities;

/// <summary>
///   Represents the persistence entity for user settings in the database.
/// </summary>
/// <remarks>
///   This class is used by Entity Framework Core to map the singleton row of user settings.
///   It contains properties that correspond to the columns of the user settings table.
/// </remarks>
public class UserSettingsEntity
{
  #region Properties

  /// <summary>
  ///   Gets or sets the unique identifier for the user settings entity.
  /// </summary>
  /// <remarks>
  ///   This property serves as the primary key for the <see cref="UserSettingsEntity" /> table.
  ///   It is configured to never have its value generated automatically.
  /// </remarks>
  public int Id { get; set; }

  /// <summary>
  ///   Gets or sets the click mode setting for the user.
  /// </summary>
  /// <remarks>
  ///   This property represents the user's preferred click mode, which is stored in the database.
  ///   It is mapped to the corresponding column in the user settings table.
  /// </remarks>
  public int ClickMode { get; set; }

  /// <summary>
  ///   Gets or sets the layout configuration for icons in the user interface.
  /// </summary>
  /// <remarks>
  ///   This property represents the user's preferred arrangement of icons,
  ///   stored as an integer value in the database. It is used to persist and retrieve
  ///   the icon layout settings for the application.
  /// </remarks>
  public int IconLayout { get; set; }

  /// <summary>
  ///   Gets or sets the timestamp of the last update to the user settings.
  /// </summary>
  /// <remarks>
  ///   This property is automatically set to the current UTC time when a new instance is created.
  ///   It is used to track when the user settings were last modified.
  /// </remarks>
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  #endregion
}
