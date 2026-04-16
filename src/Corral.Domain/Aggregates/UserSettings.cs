// ------------------------------------------------------------------------------------------------
// <copyright file="UserSettings.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.Aggregates;

/// <summary>
///   Represents the global user settings for the Corral application.
/// </summary>
/// <remarks>
///   This aggregate is a singleton — there is always exactly one row in the database.
///   <see cref="Id" /> is always <c>1</c>.
/// </remarks>
public class UserSettings
{
  #region Fields

  #region Constants

  /// <summary>The fixed primary key for the singleton row.</summary>
  public const int SingletonId = 1;

  #endregion

  #endregion

  #region Ctors

  private UserSettings()
  {
    // For EF Core
  }

  #endregion

  #region Properties

  /// <summary>Gets the primary key (always 1).</summary>
  public int Id { get; private set; }

  /// <summary>
  ///   Gets the click mode for opening items in overlays.
  ///   Stored as int to keep Domain independent of presentation-layer enums.
  ///   0 = SingleClick, 1 = DoubleClick.
  /// </summary>
  public int ClickMode { get; private set; }

  /// <summary>
  ///   Gets the icon layout for overlays.
  ///   Stored as int to keep Domain independent of presentation-layer enums.
  ///   0 = LargeGrid, 1 = SmallGrid, 2 = List.
  /// </summary>
  public int IconLayout { get; private set; }

  /// <summary>Gets the last modification timestamp.</summary>
  public DateTime UpdatedAt { get; private set; }

  #endregion

  #region Methods

  /// <summary>Updates the settings with new values.</summary>
  public void Update(int clickMode, int iconLayout)
  {
    ClickMode = clickMode;
    IconLayout = iconLayout;
    UpdatedAt = DateTime.UtcNow;
  }

  #endregion

  #region Factory methods

  /// <summary>Creates the singleton settings row with default values.</summary>
  public static UserSettings Create()
  {
    return new UserSettings
    {
      Id = SingletonId, ClickMode = 0, IconLayout = 0, UpdatedAt = DateTime.UtcNow
    };
  }

  /// <summary>Reconstitutes a UserSettings from persisted data.</summary>
  public static UserSettings Reconstitute(int id, int clickMode, int iconLayout, DateTime updatedAt)
  {
    return new UserSettings
    {
      Id = id, ClickMode = clickMode, IconLayout = iconLayout, UpdatedAt = updatedAt
    };
  }

  #endregion
}
