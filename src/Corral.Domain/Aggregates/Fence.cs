// ------------------------------------------------------------------------------------------------
// <copyright file="Fence.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.DomainEvents;
using Corral.Domain.ValueObjects;

namespace Corral.Domain.Aggregates;

/// <summary>
///   Represents a virtual area on the desktop, referred to as a "Fence".
/// </summary>
/// <remarks>
///   This class serves as an aggregate root in the domain model. It encapsulates the state and
///   behavior
///   of a Fence, including its position, dimensions, background color, opacity, and associated items.
/// </remarks>
public class Fence
{
  #region Fields

  private readonly List<IDomainEvent> _domainEvents = [];
  private readonly List<FenceItem> _items = [];

  #endregion

  #region Ctors

  private Fence()
  {
    // Pour EF Core
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the unique identifier of the Fence.
  /// </summary>
  /// <remarks>
  ///   The <see cref="FenceId" /> is used to uniquely identify a Fence instance within the domain.
  ///   It is generated when the Fence is created and remains immutable throughout the lifetime of the
  ///   Fence.
  /// </remarks>
  public FenceId Id { get; private set; }

  /// <summary>
  ///   Gets the name of the Fence.
  /// </summary>
  /// <remarks>
  ///   The name uniquely identifies the Fence and is used for display purposes.
  ///   It must be a non-empty string and is validated during Fence creation or renaming.
  /// </remarks>
  public string Name { get; private set; }

  /// <summary>
  ///   Gets the position of the fence.
  /// </summary>
  /// <remarks>
  ///   The position is represented as a <see cref="Position" /> value object,
  ///   which contains the X and Y coordinates of the fence.
  /// </remarks>
  public Position Position { get; private set; }

  /// <summary>
  ///   Gets the dimensions of the fence, including its width and height.
  /// </summary>
  /// <remarks>
  ///   The <see cref="Dimensions" /> property represents the size of the fence in terms of width and
  ///   height.
  ///   It is used to define the spatial boundaries of the fence and can be updated using the
  ///   <see cref="Resize" /> method.
  /// </remarks>
  public Dimensions Dimensions { get; private set; }

  /// <summary>
  ///   Gets the background color of the fence.
  /// </summary>
  /// <remarks>
  ///   The <see cref="BackgroundColor" /> property represents the RGBA color value of the fence.
  ///   It is used to define the visual appearance of the fence in the application.
  /// </remarks>
  public Color BackgroundColor { get; private set; }

  /// <summary>
  ///   Gets the opacity level of the fence, represented as a percentage.
  /// </summary>
  /// <remarks>
  ///   The opacity value determines the transparency level of the fence, where 0 represents fully
  ///   transparent
  ///   and 100 represents fully opaque.
  /// </remarks>
  public Opacity Opacity { get; private set; }

  /// <summary>
  ///   Gets a value indicating whether the fence is currently active.
  /// </summary>
  /// <remarks>
  ///   An active fence is one that is currently enabled and operational. This property
  ///   can be used to determine the state of the fence and control its behavior accordingly.
  /// </remarks>
  public bool IsActive { get; private set; }

  /// <summary>
  ///   Gets the date and time when the fence was created.
  /// </summary>
  /// <remarks>
  ///   This property is set during the creation of the <see cref="Fence" /> instance
  ///   and represents the moment the instance was initialized.
  /// </remarks>
  public DateTime CreatedAt { get; private set; }

  /// <summary>
  ///   Gets the date and time when the fence was last updated.
  /// </summary>
  /// <remarks>
  ///   This property is updated whenever a significant change is made to the fence, such as
  ///   moving its position, changing its color, opacity, or resizing it. If the fence has not
  ///   been updated since its creation, this property will be <c>null</c>.
  /// </remarks>
  public DateTime? UpdatedAt { get; private set; }

  /// <summary>
  ///   Gets a read-only collection of <see cref="FenceItem" /> objects associated with the current
  ///   <see cref="Fence" />.
  /// </summary>
  /// <remarks>
  ///   This property provides access to the items contained within the fence.
  ///   The collection is immutable, ensuring that the items cannot be modified directly.
  /// </remarks>
  public IReadOnlyList<FenceItem> Items => _items.AsReadOnly();

  #endregion

  #region Methods

  /// <summary>
  ///   Creates a new instance of the <see cref="Fence" /> class with the specified properties.
  /// </summary>
  /// <param name="name">The name of the fence. Must not be null or whitespace.</param>
  /// <param name="position">
  ///   The position of the fence, represented as a <see cref="Position" /> object.
  ///   Must not be null.
  /// </param>
  /// <param name="dimensions">
  ///   The dimensions of the fence, represented as a <see cref="Dimensions" />
  ///   object. Must not be null.
  /// </param>
  /// <param name="backgroundColor">
  ///   The background color of the fence, represented as a
  ///   <see cref="Color" /> object. Must not be null.
  /// </param>
  /// <param name="opacity">
  ///   The opacity of the fence, represented as an <see cref="Opacity" /> object.
  ///   If not provided, defaults to <see cref="Opacity.SemiTransparent" />.
  /// </param>
  /// <returns>A new instance of the <see cref="Fence" /> class.</returns>
  /// <exception cref="ArgumentException">Thrown when <paramref name="name" /> is null or whitespace.</exception>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="position" />, <paramref name="dimensions" />, or
  ///   <paramref name="backgroundColor" /> is null.
  /// </exception>
  public static Fence Create(
    string name,
    Position position,
    Dimensions dimensions,
    Color backgroundColor,
    Opacity opacity = null!)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Name ne peut pas être vide");
    }

    opacity ??= Opacity.SemiTransparent;

    var fence = new Fence
    {
      Id = FenceId.Create(),
      Name = name,
      Position = position ?? throw new ArgumentNullException(nameof(position)),
      Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions)),
      BackgroundColor =
        backgroundColor ?? throw new ArgumentNullException(nameof(backgroundColor)),
      Opacity = opacity,
      IsActive = true,
      CreatedAt = DateTime.UtcNow
    };

    // Enregistrer l'événement domaine
    fence._domainEvents.Add(new FenceCreatedEvent(fence.Id, fence.Name));

    return fence;
  }

  /// <summary>
  ///   Reconstitutes an existing <see cref="Fence" /> instance from persisted data.
  /// </summary>
  /// <param name="id">The unique identifier of the fence.</param>
  /// <param name="name">The name of the fence.</param>
  /// <param name="position">The position of the fence in a 2D space.</param>
  /// <param name="dimensions">The dimensions of the fence.</param>
  /// <param name="backgroundColor">The background color of the fence.</param>
  /// <param name="opacity">The opacity level of the fence.</param>
  /// <param name="isActive">Indicates whether the fence is active.</param>
  /// <param name="createdAt">The date and time when the fence was created.</param>
  /// <param name="updatedAt">
  ///   The date and time when the fence was last updated, or <c>null</c> if it has
  ///   not been updated.
  /// </param>
  /// <returns>A reconstituted <see cref="Fence" /> instance.</returns>
  /// <remarks>
  ///   This factory method is intended for use by the Infrastructure layer to reconstitute
  ///   the aggregate root from persistence data without invoking the public constructor or
  ///   using the <c>Create</c> factory method. Domain events are not reconstituted as they
  ///   are managed separately.
  /// </remarks>
  public static Fence Reconstitute(
    FenceId id,
    string name,
    Position position,
    Dimensions dimensions,
    Color backgroundColor,
    Opacity opacity,
    bool isActive,
    DateTime createdAt,
    DateTime? updatedAt)
  {
    return new Fence
    {
      Id = id,
      Name = name,
      Position = position,
      Dimensions = dimensions,
      BackgroundColor = backgroundColor,
      Opacity = opacity,
      IsActive = isActive,
      CreatedAt = createdAt,
      UpdatedAt = updatedAt
    };
  }

  /// <summary>
  ///   Moves the fence to a new position.
  /// </summary>
  /// <param name="newPosition">
  ///   The new position to move the fence to. Must not be <c>null</c>.
  /// </param>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="newPosition" /> is <c>null</c>.
  /// </exception>
  /// <remarks>
  ///   This method updates the fence's position, sets the <see cref="UpdatedAt" /> property to the
  ///   current UTC time,
  ///   and raises a <see cref="FencePositionChangedEvent" />.
  ///   Returns this instance to enable method chaining.
  /// </remarks>
  public Fence Move(Position newPosition)
  {
    Position = newPosition ?? throw new ArgumentNullException(nameof(newPosition));
    UpdatedAt = DateTime.UtcNow;
    _domainEvents.Add(new FencePositionChangedEvent(Id, newPosition));
    return this;
  }

  /// <summary>
  ///   Changes the background color of the fence.
  /// </summary>
  /// <param name="newColor">
  ///   The new background color to be applied to the fence.
  ///   Must not be <c>null</c>.
  /// </param>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="newColor" /> is <c>null</c>.
  /// </exception>
  /// <remarks>
  ///   This method updates the <see cref="BackgroundColor" /> property of the fence,
  ///   sets the <see cref="UpdatedAt" /> timestamp to the current UTC time,
  ///   and raises a <see cref="FenceColorChangedEvent" /> to notify about the change.
  ///   Returns this instance to enable method chaining.
  /// </remarks>
  public Fence ChangeColor(Color newColor)
  {
    BackgroundColor = newColor ?? throw new ArgumentNullException(nameof(newColor));
    UpdatedAt = DateTime.UtcNow;
    _domainEvents.Add(new FenceColorChangedEvent(Id, newColor));
    return this;
  }

  /// <summary>
  ///   Updates the opacity of the fence.
  /// </summary>
  /// <param name="newOpacity">The new opacity value to be applied to the fence.</param>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="newOpacity" /> is <c>null</c>.
  /// </exception>
  /// <remarks>
  ///   This method modifies the <see cref="Opacity" /> property of the fence and updates the
  ///   <see cref="UpdatedAt" /> timestamp to the current UTC time.
  ///   Returns this instance to enable method chaining.
  /// </remarks>
  public Fence ChangeOpacity(Opacity newOpacity)
  {
    Opacity = newOpacity ?? throw new ArgumentNullException(nameof(newOpacity));
    UpdatedAt = DateTime.UtcNow;
    return this;
  }

  /// <summary>
  ///   Updates the dimensions of the fence to the specified new dimensions.
  /// </summary>
  /// <param name="newDimensions">The new dimensions to apply to the fence.</param>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="newDimensions" /> is
  ///   <c>null</c>.
  /// </exception>
  /// <remarks>
  ///   This method modifies the <see cref="Dimensions" /> property of the fence and updates the
  ///   <see cref="UpdatedAt" /> timestamp
  ///   to reflect the time of the change.
  ///   Returns this instance to enable method chaining.
  /// </remarks>
  public Fence Resize(Dimensions newDimensions)
  {
    Dimensions = newDimensions ?? throw new ArgumentNullException(nameof(newDimensions));
    UpdatedAt = DateTime.UtcNow;
    return this;
  }

  /// <summary>
  ///   Renames the fence by updating its name.
  /// </summary>
  /// <param name="newName">The new name to assign to the fence.</param>
  /// <exception cref="ArgumentException">
  ///   Thrown when <paramref name="newName" /> is null, empty, or consists only of white-space
  ///   characters.
  /// </exception>
  /// <remarks>
  ///   This method updates the <see cref="Name" /> property of the fence and sets the
  ///   <see cref="UpdatedAt" />
  ///   property to the current UTC time.
  ///   Returns this instance to enable method chaining.
  /// </remarks>
  public Fence Rename(string newName)
  {
    if (string.IsNullOrWhiteSpace(newName))
    {
      throw new ArgumentException("Name ne peut pas être vide");
    }

    Name = newName;
    UpdatedAt = DateTime.UtcNow;
    return this;
  }

  /// <summary>
  ///   Activates the fence, marking it as active.
  /// </summary>
  /// <remarks>
  ///   If the fence is already active, this method will not change its state.
  ///   Otherwise, it sets the <see cref="IsActive" /> property to <c>true</c>
  ///   and updates the <see cref="UpdatedAt" /> timestamp to the current UTC time.
  ///   Returns this instance to enable method chaining.
  /// </remarks>
  /// <exception cref="InvalidOperationException">
  ///   Thrown if the activation process encounters an unexpected state.
  /// </exception>
  public Fence Activate()
  {
    if (!IsActive)
    {
      IsActive = true;
      UpdatedAt = DateTime.UtcNow;
    }

    return this;
  }

  /// <summary>
  ///   Deactivates the fence, marking it as inactive.
  /// </summary>
  /// <remarks>
  ///   If the fence is currently active, this method sets <see cref="IsActive" /> to <c>false</c>,
  ///   updates the <see cref="UpdatedAt" /> timestamp to the current UTC time, and raises a
  ///   <see cref="FenceDeactivatedEvent" /> to indicate the state change.
  ///   Returns this instance to enable method chaining.
  /// </remarks>
  /// <exception cref="InvalidOperationException">
  ///   Thrown if the method is called on an already inactive fence.
  /// </exception>
  public Fence Deactivate()
  {
    if (IsActive)
    {
      IsActive = false;
      UpdatedAt = DateTime.UtcNow;
      _domainEvents.Add(new FenceDeactivatedEvent(Id));
    }

    return this;
  }

  /// <summary>
  ///   Deletes the current fence and raises a <see cref="FenceDeletedEvent" />.
  /// </summary>
  /// <remarks>
  ///   This method adds a <see cref="FenceDeletedEvent" /> to the domain events collection,
  ///   signaling that the fence has been deleted. The event can be used to trigger
  ///   additional actions or processes in the application.
  /// </remarks>
  /// <exception cref="InvalidOperationException">
  ///   Thrown if the fence is in an invalid state for deletion.
  /// </exception>
  public void Delete()
  {
    _domainEvents.Add(new FenceDeletedEvent(Id));
  }

  /// <summary>
  ///   Retrieves the list of domain events that have been recorded for this <see cref="Fence" />.
  /// </summary>
  /// <returns>
  ///   A read-only list of domain events implementing the <see cref="IDomainEvent" /> interface.
  /// </returns>
  /// <remarks>
  ///   Domain events represent significant occurrences or changes within the domain.
  ///   This method provides access to the events that have been recorded but not yet processed or
  ///   cleared.
  /// </remarks>
  public IReadOnlyList<IDomainEvent> GetDomainEvents()
  {
    return _domainEvents.AsReadOnly();
  }

  /// <summary>
  ///   Clears all domain events associated with this <see cref="Fence" /> instance.
  /// </summary>
  /// <remarks>
  ///   This method is typically called after domain events have been persisted or handled,
  ///   ensuring that the internal event list is reset for subsequent operations.
  /// </remarks>
  public void ClearDomainEvents()
  {
    _domainEvents.Clear();
  }

  /// <summary>
  ///   Adds a new item to the fence.
  /// </summary>
  /// <param name="displayName">The display name of the item to be added.</param>
  /// <param name="path">The path associated with the item.</param>
  /// <param name="itemType">The type of the item (e.g., Shortcut, File, Link).</param>
  /// <returns>The newly created <see cref="FenceItem" /> instance.</returns>
  /// <remarks>
  ///   This method creates a new <see cref="FenceItem" /> with the specified details,
  ///   assigns it a sort order based on the current number of items in the fence,
  ///   and updates the <c>UpdatedAt</c> timestamp of the fence.
  /// </remarks>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="displayName" /> or <paramref name="path" /> is <c>null</c> or empty.
  /// </exception>
  public FenceItem AddItem(string displayName, string path, FenceItemType itemType)
  {
    var sortOrder = _items.Count;
    var item = FenceItem.Create(displayName, path, itemType, sortOrder);
    _items.Add(item);
    UpdatedAt = DateTime.UtcNow;
    return item;
  }

  /// <summary>
  ///   Removes an item from the fence by its unique identifier.
  /// </summary>
  /// <param name="itemId">The unique identifier of the item to be removed.</param>
  /// <exception cref="InvalidOperationException">
  ///   Thrown when no item with the specified <paramref name="itemId" /> is found in the fence.
  /// </exception>
  /// <remarks>
  ///   This method updates the <see cref="UpdatedAt" /> property to the current UTC time
  ///   when an item is successfully removed.
  ///   Returns this instance to enable method chaining.
  /// </remarks>
  public Fence RemoveItem(string itemId)
  {
    var item = _items.Find(i => i.Id == itemId);
    if (item == null)
    {
      throw new InvalidOperationException($"Item with ID '{itemId}' not found");
    }

    _items.Remove(item);
    for (var index = 0; index < _items.Count; index++)
    {
      _items[index].UpdateSortOrder(index);
    }

    UpdatedAt = DateTime.UtcNow;
    return this;
  }

  /// <summary>
  ///   Reorders an item inside the fence.
  /// </summary>
  /// <param name="itemId">The ID of the item being moved.</param>
  /// <param name="targetItemId">
  ///   The ID of the item before which the source item is inserted. Empty means
  ///   move to the end.
  /// </param>
  /// <returns>The current fence instance.</returns>
  /// <exception cref="InvalidOperationException">Thrown when the source or target item does not exist.</exception>
  public Fence ReorderItems(string itemId, string targetItemId)
  {
    var item = _items.Find(i => i.Id == itemId);
    if (item == null)
    {
      throw new InvalidOperationException($"Item with ID '{itemId}' not found");
    }

    if (!string.IsNullOrWhiteSpace(targetItemId) && itemId == targetItemId)
    {
      return this;
    }

    _items.Remove(item);

    if (string.IsNullOrWhiteSpace(targetItemId))
    {
      _items.Add(item);
    }
    else
    {
      var targetIndex = _items.FindIndex(i => i.Id == targetItemId);
      if (targetIndex < 0)
      {
        throw new InvalidOperationException($"Target item with ID '{targetItemId}' not found");
      }

      _items.Insert(targetIndex, item);
    }

    for (var index = 0; index < _items.Count; index++)
    {
      _items[index].UpdateSortOrder(index);
    }

    UpdatedAt = DateTime.UtcNow;
    return this;
  }

  /// <summary>
  ///   Loads the specified collection of <see cref="FenceItem" /> instances into the current
  ///   <see cref="Fence" />.
  /// </summary>
  /// <param name="items">The collection of <see cref="FenceItem" /> instances to be loaded.</param>
  /// <remarks>
  ///   This method clears any existing items in the <see cref="Fence" /> and replaces them with the
  ///   provided collection.
  ///   It is typically used to reconstitute a <see cref="Fence" /> from a persisted state.
  /// </remarks>
  public void LoadItems(IEnumerable<FenceItem> items)
  {
    _items.Clear();
    _items.AddRange(items);
  }

  #endregion
}
