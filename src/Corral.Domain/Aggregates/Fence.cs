// ------------------------------------------------------------------------------------------------
// <copyright file="Fence.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.DomainEvents;
using Corral.Domain.ValueObjects;

namespace Corral.Domain.Aggregates;

/// <summary>
///   Aggregate Root - représente une zone virtuelle sur le bureau (Fence)
/// </summary>
public class Fence
{
  #region Fields

  private readonly List<IDomainEvent> _domainEvents = new();

  #endregion

  #region Ctors

  private Fence()
  {
    // Pour EF Core
  }

  #endregion

  #region Properties

  public FenceId Id { get; private set; }

  public string Name { get; private set; }

  public Position Position { get; private set; }

  public Dimensions Dimensions { get; private set; }

  public Color BackgroundColor { get; private set; }

  public Opacity Opacity { get; private set; }

  public bool IsActive { get; private set; }

  public DateTime CreatedAt { get; private set; }

  public DateTime? UpdatedAt { get; private set; }

  #endregion

  #region Methods

  /// <summary>
  ///   Factory method pour créer une nouvelle Fence
  /// </summary>
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
  ///   Factory method pour reconstituer une Fence existante depuis les données persistées
  ///   (Infrastructure).
  ///   Utilisée uniquement lors du chargement depuis la base de données.
  /// </summary>
  /// <remarks>
  ///   Cette méthode permet à la couche Infrastructure de reconstituer l'Aggregate Root
  ///   à partir des données de persistance sans faire appel au constructeur public ou passer par le
  ///   factory Create.
  ///   Les domain events ne sont pas reconstitués car ils sont gérés séparément.
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
  ///   Déplace la fence à une nouvelle position
  /// </summary>
  public void Move(Position newPosition)
  {
    if (newPosition == null)
    {
      throw new ArgumentNullException(nameof(newPosition));
    }

    Position = newPosition;
    UpdatedAt = DateTime.UtcNow;
    _domainEvents.Add(new FencePositionChangedEvent(Id, newPosition));
  }

  /// <summary>
  ///   Change la couleur de fond de la fence
  /// </summary>
  public void ChangeColor(Color newColor)
  {
    if (newColor == null)
    {
      throw new ArgumentNullException(nameof(newColor));
    }

    BackgroundColor = newColor;
    UpdatedAt = DateTime.UtcNow;
    _domainEvents.Add(new FenceColorChangedEvent(Id, newColor));
  }

  /// <summary>
  ///   Change l'opacité de la fence
  /// </summary>
  public void ChangeOpacity(Opacity newOpacity)
  {
    if (newOpacity == null)
    {
      throw new ArgumentNullException(nameof(newOpacity));
    }

    Opacity = newOpacity;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  ///   Change les dimensions de la fence
  /// </summary>
  public void Resize(Dimensions newDimensions)
  {
    if (newDimensions == null)
    {
      throw new ArgumentNullException(nameof(newDimensions));
    }

    Dimensions = newDimensions;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  ///   Change le nom de la fence
  /// </summary>
  public void Rename(string newName)
  {
    if (string.IsNullOrWhiteSpace(newName))
    {
      throw new ArgumentException("Name ne peut pas être vide");
    }

    Name = newName;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  ///   Active la fence
  /// </summary>
  public void Activate()
  {
    if (IsActive)
    {
      return;
    }

    IsActive = true;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  ///   Désactive la fence
  /// </summary>
  public void Deactivate()
  {
    if (!IsActive)
    {
      return;
    }

    IsActive = false;
    UpdatedAt = DateTime.UtcNow;
    _domainEvents.Add(new FenceDeactivatedEvent(Id));
  }

  /// <summary>
  ///   Supprime la fence (événement domaine)
  /// </summary>
  public void Delete()
  {
    _domainEvents.Add(new FenceDeletedEvent(Id));
  }

  /// <summary>
  ///   Retourne les événements domaine enregistrés
  /// </summary>
  public IReadOnlyList<IDomainEvent> GetDomainEvents()
  {
    return _domainEvents.AsReadOnly();
  }

  /// <summary>
  ///   Efface les événements domaine (après persistance)
  /// </summary>
  public void ClearDomainEvents()
  {
    _domainEvents.Clear();
  }

  #endregion
}
