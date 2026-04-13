// ------------------------------------------------------------------------------------------------
// <copyright file="FenceEntity.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Infrastructure.Persistence.Entities;

/// <summary>
///   Represents an EF Core persistence entity for a Fence in the database.
///   This class is strictly tied to the Infrastructure layer and should never be exposed to other
///   layers.
/// </summary>
public class FenceEntity
{
  #region Properties

  /// <summary>
  ///   Gets or sets the unique identifier of the fence in the database.
  /// </summary>
  public string Id { get; set; } = string.Empty;

  /// <summary>
  ///   Gets or sets the descriptive name of the fence.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  ///   Gets or sets the horizontal (X) position in pixels.
  /// </summary>
  public int PositionX { get; set; }

  /// <summary>
  ///   Gets or sets the vertical (Y) position in pixels.
  /// </summary>
  public int PositionY { get; set; }

  /// <summary>
  ///   Gets or sets the width of the fence in pixels.
  /// </summary>
  public int Width { get; set; }

  /// <summary>
  ///   Gets or sets the height of the fence in pixels.
  /// </summary>
  public int Height { get; set; }

  /// <summary>
  ///   Gets or sets the background color in hexadecimal format (e.g., FFFF0000 for opaque red).
  /// </summary>
  public string BackgroundColor { get; set; } = "FFFFFFFF";

  /// <summary>
  ///   Gets or sets the opacity percentage (0-100).
  /// </summary>
  public int Opacity { get; set; } = 100;

  /// <summary>
  ///   Gets or sets a value indicating whether the fence is currently active.
  /// </summary>
  public bool IsActive { get; set; } = true;

  /// <summary>
  ///   Gets or sets the creation date and time of the fence.
  /// </summary>
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  /// <summary>
  ///   Gets or sets the last modification date and time of the fence.
  /// </summary>
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  /// <summary>
  ///   Gets or sets the collection of items within this fence.
  /// </summary>
  public List<FenceItemEntity> Items { get; set; } = [];

  #endregion
}
