// ------------------------------------------------------------------------------------------------
// <copyright file="FenceItemEntity.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Infrastructure.Persistence.Entities;

/// <summary>
///   Represents an EF Core persistence entity for a FenceItem in the database.
/// </summary>
public class FenceItemEntity
{
  #region Properties

  /// <summary>
  ///   Gets or sets the unique identifier of the fence item.
  /// </summary>
  public string Id { get; set; } = string.Empty;

  /// <summary>
  ///   Gets or sets the ID of the parent fence.
  /// </summary>
  public string FenceId { get; set; } = string.Empty;

  /// <summary>
  ///   Gets or sets the display name of the item.
  /// </summary>
  public string DisplayName { get; set; } = string.Empty;

  /// <summary>
  ///   Gets or sets the file path, URL, or shortcut target.
  /// </summary>
  public string Path { get; set; } = string.Empty;

  /// <summary>
  ///   Gets or sets the type of item (0=Shortcut, 1=File, 2=Link).
  /// </summary>
  public int ItemType { get; set; }

  /// <summary>
  ///   Gets or sets the sort order within the fence.
  /// </summary>
  public int SortOrder { get; set; }

  /// <summary>
  ///   Gets or sets the creation date.
  /// </summary>
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  /// <summary>
  ///   Gets or sets the parent fence entity.
  /// </summary>
  public FenceEntity Fence { get; set; }

  #endregion
}
