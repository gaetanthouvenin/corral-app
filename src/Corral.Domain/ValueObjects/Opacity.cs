// ------------------------------------------------------------------------------------------------
// <copyright file="Opacity.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.ValueObjects;

/// <summary>
///   Représente le niveau d'opacité (transparence) d'une Fence, exprimé en pourcentage (0-100%).
/// </summary>
/// <remarks>
///   <para>
///     Opacity est un Value Object immuable représentant le degré de transparence d'une zone Fence.
///     Les valeurs valides sont comprises entre 0 (complètement transparent) et 100 (complètement
///     opaque).
///   </para>
///   <para>
///     La classe offre également des conversions vers/depuis une représentation normalisée (0.0-1.0)
///     couramment utilisée dans les frameworks graphiques.
///   </para>
///   <para>
///     Exemples d'utilisation:
///     <code>
/// var opaque = Opacity.Opaque; // 100%
/// var transparent = Opacity.Transparent; // 0%
/// var semiOpaque = Opacity.Create(75); // 75%
/// var normalized = semiOpaque.ToNormalized(); // 0.75
/// var fromNorm = Opacity.FromNormalized(0.5); // 50%
/// </code>
///   </para>
/// </remarks>
public record Opacity(int Percentage)
{
  #region Fields

  /// <summary>
  ///   La valeur minimale d'opacité (complètement transparent).
  /// </summary>
  /// <value>0 (0%).</value>
  public const int Min = 0;

  /// <summary>
  ///   La valeur maximale d'opacité (complètement opaque).
  /// </summary>
  /// <value>100 (100%).</value>
  public const int Max = 100;

  #endregion

  #region Properties

  /// <summary>
  ///   Obtient le niveau d'opacité complètement transparent (0%).
  /// </summary>
  /// <value>Une instance de Opacity avec Percentage=0.</value>
  public static Opacity Transparent => new(0);

  /// <summary>
  ///   Obtient le niveau d'opacité semi-transparent (50%).
  /// </summary>
  /// <value>Une instance de Opacity avec Percentage=50.</value>
  public static Opacity SemiTransparent => new(50);

  /// <summary>
  ///   Obtient le niveau d'opacité complètement opaque (100%).
  /// </summary>
  /// <value>Une instance de Opacity avec Percentage=100.</value>
  public static Opacity Opaque => new(100);

  #endregion

  #region Methods

  /// <summary>
  ///   Crée un nouveau niveau d'opacité avec validation.
  /// </summary>
  /// <param name="percentage">Pourcentage d'opacité. Doit être entre 0 et 100 (inclus).</param>
  /// <returns>Une nouvelle instance de Opacity.</returns>
  /// <exception cref="InvalidOperationException">Levée si le pourcentage est hors de la plage [0, 100].</exception>
  /// <example>
  ///   <code>
  /// var opacity = Opacity.Create(75); // 75% opaque
  /// </code>
  /// </example>
  public static Opacity Create(int percentage)
  {
    if (percentage < Min || percentage > Max)
    {
      throw new InvalidOperationException($"Opacity doit être entre {Min} et {Max}");
    }

    return new Opacity(percentage);
  }

  /// <summary>
  ///   Convertit l'opacité en valeur normalisée (entre 0.0 et 1.0).
  /// </summary>
  /// <returns>Valeur normalisée où 0.0 = transparent et 1.0 = opaque.</returns>
  /// <remarks>
  ///   Cette conversion est utile pour les frameworks graphiques qui utilisent
  ///   une représentation normalisée de l'opacité.
  /// </remarks>
  /// <example>
  ///   <code>
  /// var opacity = Opacity.Create(50);
  /// var normalized = opacity.ToNormalized(); // 0.5
  /// </code>
  /// </example>
  public double ToNormalized()
  {
    return Percentage / 100.0;
  }

  /// <summary>
  ///   Crée une opacité à partir d'une valeur normalisée (entre 0.0 et 1.0).
  /// </summary>
  /// <param name="normalized">Valeur normalisée. Doit être entre 0.0 et 1.0 (inclus).</param>
  /// <returns>Une nouvelle instance de Opacity.</returns>
  /// <exception cref="InvalidOperationException">
  ///   Levée si la valeur normalisée est hors de la plage
  ///   [0.0, 1.0].
  /// </exception>
  /// <remarks>La conversion arrondit le résultat à l'entier le plus proche.</remarks>
  /// <example>
  ///   <code>
  /// var opacity = Opacity.FromNormalized(0.75); // 75% opaque
  /// </code>
  /// </example>
  public static Opacity FromNormalized(double normalized)
  {
    if (normalized < 0.0 || normalized > 1.0)
    {
      throw new InvalidOperationException("Valeur normalisée doit être entre 0.0 et 1.0");
    }

    return new Opacity((int)(normalized * 100));
  }

  #endregion
}
