// ------------------------------------------------------------------------------------------------
// <copyright file="FenceItem.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.ValueObjects;

namespace Corral.Domain.Aggregates;

/// <summary>
///   Represents an item (shortcut, file, link) contained within a Fence.
/// </summary>
/// <remarks>
///   <para>
///     FenceItem is an entity within the Fence aggregate root. It encapsulates
///     the metadata of a single item such as its display name, path, type, and order.
///   </para>
///   <para>
///     Items are mutable and support fluent method chaining for convenient state modifications.
///   </para>
/// </remarks>
public class FenceItem
{
  #region Ctors

  private FenceItem()
  {
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the unique identifier of the item.
  /// </summary>
  public string Id { get; private set; }

  /// <summary>
  ///   Gets the display name of the item.
  /// </summary>
  public string DisplayName { get; private set; }

  /// <summary>
  ///   Gets the full path to the file, folder, executable, or URL.
  /// </summary>
  public string Path { get; private set; }

  /// <summary>
  ///   Gets the type of item (Shortcut, File, Link).
  /// </summary>
  public FenceItemType ItemType { get; private set; }

  /// <summary>
  ///   Gets the display order of the item within the fence.
  /// </summary>
  public int SortOrder { get; private set; }

  /// <summary>
  ///   Gets the date and time when the item was created.
  /// </summary>
  public DateTime CreatedAt { get; private set; }

  #endregion

  #region Methods

  /// <summary>
  ///   Creates a new fence item with the specified properties.
  /// </summary>
  /// <param name="displayName">The display name of the item. Must not be null or whitespace.</param>
  /// <param name="path">The path associated with the item. Must not be null or whitespace.</param>
  /// <param name="itemType">The type of the item.</param>
  /// <param name="sortOrder">The display order within the fence. Defaults to 0.</param>
  /// <returns>A new instance of <see cref="FenceItem" />.</returns>
  /// <exception cref="ArgumentException">
  ///   Thrown when <paramref name="displayName" /> or <paramref name="path" /> is null or whitespace.
  /// </exception>
  public static FenceItem Create(
    string displayName,
    string path,
    FenceItemType itemType,
    int sortOrder = 0)
  {
    if (string.IsNullOrWhiteSpace(displayName))
    {
      throw new ArgumentException("DisplayName ne peut pas être vide");
    }

    if (string.IsNullOrWhiteSpace(path))
    {
      throw new ArgumentException("Path ne peut pas être vide");
    }

    return new FenceItem
    {
      Id = Guid.NewGuid().ToString(),
      DisplayName = displayName,
      Path = path,
      ItemType = itemType,
      SortOrder = sortOrder,
      CreatedAt = DateTime.UtcNow
    };
  }

  /// <summary>
  ///   Reconstitutes a persisted fence item from database data.
  /// </summary>
  /// <param name="id">The unique identifier of the item.</param>
  /// <param name="displayName">The display name of the item.</param>
  /// <param name="path">The path associated with the item.</param>
  /// <param name="itemType">The type of the item.</param>
  /// <param name="sortOrder">The display order within the fence.</param>
  /// <param name="createdAt">The date and time when the item was created.</param>
  /// <returns>A reconstituted <see cref="FenceItem" /> instance.</returns>
  /// <remarks>
  ///   This method is used by the Infrastructure layer to restore an item from persistence.
  ///   Domain events are not reconstituted as they are managed separately.
  /// </remarks>
  public static FenceItem Reconstitute(
    string id,
    string displayName,
    string path,
    FenceItemType itemType,
    int sortOrder,
    DateTime createdAt)
  {
    return new FenceItem
    {
      Id = id,
      DisplayName = displayName,
      Path = path,
      ItemType = itemType,
      SortOrder = sortOrder,
      CreatedAt = createdAt
    };
  }

  /// <summary>
  ///   Updates the display name of the item.
  /// </summary>
  /// <param name="newDisplayName">The new display name. Must not be null or whitespace.</param>
  /// <returns>This instance to enable method chaining.</returns>
  /// <exception cref="ArgumentException">
  ///   Thrown when <paramref name="newDisplayName" /> is null or whitespace.
  /// </exception>
  public FenceItem Rename(string newDisplayName)
  {
    if (string.IsNullOrWhiteSpace(newDisplayName))
    {
      throw new ArgumentException("DisplayName ne peut pas être vide");
    }

    DisplayName = newDisplayName;
    return this;
  }

  /// <summary>
  ///   Updates the display order of the item within the fence.
  /// </summary>
  /// <param name="newSortOrder">The new sort order index.</param>
  /// <returns>This instance to enable method chaining.</returns>
  public FenceItem UpdateSortOrder(int newSortOrder)
  {
    SortOrder = newSortOrder;
    return this;
  }

  #endregion
}
