// ------------------------------------------------------------------------------------------------
// <copyright file="Dimensions.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.ValueObjects;

/// <summary>
///   Représente les dimensions (largeur x hauteur) d'une zone de Fence en pixels.
/// </summary>
/// <remarks>
///   <para>
///     Dimensions est un Value Object immuable qui encapsule les contraintes de taille
///     pour les zones virtuelles (Fences). Les dimensions doivent être comprises dans les limites
///     autorisées
///     définie par les constantes MinWidth, MinHeight, MaxWidth et MaxHeight.
///   </para>
///   <para>
///     Exemples d'utilisation:
///     <code>
/// var defaultSize = Dimensions.Default; // 200x200
/// var customSize = Dimensions.Create(400, 300);
/// var area = customSize.Area; // 120000
/// var isInside = customSize.Contains(Position.Create(100, 100)); // true
/// </code>
///   </para>
/// </remarks>
public record Dimensions(int Width, int Height)
{
  #region Fields

  /// <summary>
  ///   La largeur minimale autorisée pour une zone (en pixels).
  /// </summary>
  /// <value>50 pixels.</value>
  public const int MinWidth = 50;

  /// <summary>
  ///   La hauteur minimale autorisée pour une zone (en pixels).
  /// </summary>
  /// <value>50 pixels.</value>
  public const int MinHeight = 50;

  /// <summary>
  ///   La largeur maximale autorisée pour une zone (en pixels).
  /// </summary>
  /// <value>2560 pixels (résolution 4K).</value>
  public const int MaxWidth = 2560;

  /// <summary>
  ///   La hauteur maximale autorisée pour une zone (en pixels).
  /// </summary>
  /// <value>1440 pixels (résolution 2K).</value>
  public const int MaxHeight = 1440;

  #endregion

  #region Properties

  /// <summary>
  ///   Obtient les dimensions par défaut (200x200 pixels).
  /// </summary>
  /// <value>Une instance de Dimensions avec Width=200 et Height=200.</value>
  public static Dimensions Default => new(200, 200);

  /// <summary>
  ///   Calcule l'aire de la zone en pixels carrés.
  /// </summary>
  /// <value>L'aire égale à Width × Height.</value>
  /// <example>
  ///   <code>
  /// var size = Dimensions.Create(400, 300);
  /// var area = size.Area; // 120000
  /// </code>
  /// </example>
  public int Area => Width * Height;

  #endregion

  #region Methods

  /// <summary>
  ///   Crée de nouvelles dimensions avec validation des contraintes.
  /// </summary>
  /// <param name="width">Largeur en pixels. Doit être entre MinWidth et MaxWidth.</param>
  /// <param name="height">Hauteur en pixels. Doit être entre MinHeight et MaxHeight.</param>
  /// <returns>Une nouvelle instance de Dimensions.</returns>
  /// <exception cref="InvalidOperationException">
  ///   Levée si les dimensions ne respectent pas les
  ///   contraintes.
  /// </exception>
  /// <example>
  ///   <code>
  /// var size = Dimensions.Create(400, 300); // Valide
  /// var invalid = Dimensions.Create(30, 40); // Lève InvalidOperationException
  /// </code>
  /// </example>
  public static Dimensions Create(int width, int height)
  {
    if (width < MinWidth)
    {
      throw new InvalidOperationException($"Width doit être >= {MinWidth}");
    }

    if (height < MinHeight)
    {
      throw new InvalidOperationException($"Height doit être >= {MinHeight}");
    }

    if (width > MaxWidth)
    {
      throw new InvalidOperationException($"Width doit être <= {MaxWidth}");
    }

    if (height > MaxHeight)
    {
      throw new InvalidOperationException($"Height doit être <= {MaxHeight}");
    }

    return new Dimensions(width, height);
  }

  /// <summary>
  ///   Détermine si une position donnée se trouve à l'intérieur des limites de cette zone.
  /// </summary>
  /// <param name="position">La position à tester.</param>
  /// <returns>true si (0 ≤ position.X &lt; Width) et (0 ≤ position.Y &lt; Height); sinon, false.</returns>
  /// <remarks>
  ///   La vérification utilise des bornes inclusives pour X et Y (minimum) et exclusives
  ///   (maximum).
  /// </remarks>
  /// <example>
  ///   <code>
  /// var size = Dimensions.Create(200, 200);
  /// var inside = size.Contains(Position.Create(100, 100)); // true
  /// var outside = size.Contains(Position.Create(250, 150)); // false
  /// </code>
  /// </example>
  public bool Contains(Position position)
  {
    return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
  }

  #endregion
}
