// ------------------------------------------------------------------------------------------------
// <copyright file="Position.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.ValueObjects;

/// <summary>
///   Représente une position (X, Y) sur l'écran en coordonnées pixels.
/// </summary>
/// <remarks>
///   <para>
///     Position est un Value Object immuable représentant un point 2D sur le plan de l'écran.
///     Les coordonnées X et Y sont exprimées en pixels et doivent être positives ou nulles.
///   </para>
///   <para>
///     Exemples d'utilisation:
///     <code>
/// var origin = Position.Origin; // (0, 0)
/// var point = Position.Create(100, 200);
/// var distance = point.DistanceTo(Position.Origin); // ~223.6
/// </code>
///   </para>
/// </remarks>
public record Position(int X, int Y)
{
  #region Properties

  /// <summary>
  ///   Obtient la position à l'origine (0, 0).
  /// </summary>
  /// <value>Une instance de Position avec X=0 et Y=0.</value>
  public static Position Origin => new(0, 0);

  #endregion

  #region Methods

  /// <summary>
  ///   Crée une nouvelle instance de Position avec validation.
  /// </summary>
  /// <param name="x">La coordonnée X en pixels. Doit être >= 0.</param>
  /// <param name="y">La coordonnée Y en pixels. Doit être >= 0.</param>
  /// <returns>Une nouvelle instance de Position.</returns>
  /// <exception cref="InvalidOperationException">Levée si X ou Y est négatif.</exception>
  /// <example>
  ///   <code>
  /// var position = Position.Create(150, 250);
  /// </code>
  /// </example>
  public static Position Create(int x, int y)
  {
    if (x < 0)
    {
      throw new InvalidOperationException("X doit être positif ou zéro");
    }

    if (y < 0)
    {
      throw new InvalidOperationException("Y doit être positif ou zéro");
    }

    return new Position(x, y);
  }

  /// <summary>
  ///   Calcule la distance euclidienne entre cette position et une autre.
  /// </summary>
  /// <param name="other">L'autre position.</param>
  /// <returns>La distance en pixels entre les deux positions.</returns>
  /// <example>
  ///   <code>
  /// var pos1 = Position.Create(0, 0);
  /// var pos2 = Position.Create(3, 4);
  /// var distance = pos1.DistanceTo(pos2); // 5.0
  /// </code>
  /// </example>
  public double DistanceTo(Position other)
  {
    var dx = other.X - X;
    var dy = other.Y - Y;
    return Math.Sqrt(dx * dx + dy * dy);
  }

  #endregion
}
