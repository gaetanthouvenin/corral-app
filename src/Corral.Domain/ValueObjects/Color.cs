// ------------------------------------------------------------------------------------------------
// <copyright file="Color.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using System.Globalization;

namespace Corral.Domain.ValueObjects;

/// <summary>
///   Représente une couleur ARGB (Alpha, Red, Green, Blue) immuable.
/// </summary>
/// <remarks>
///   <para>
///     Color est un Value Object immuable représentant une couleur en format ARGB (32 bits).
///     Chaque composant (Alpha, Red, Green, Blue) est un octet (0-255).
///     L'Alpha représente la transparence: 0 = transparent, 255 = opaque.
///   </para>
///   <para>
///     Exemples d'utilisation:
///     <code>
/// var red = Color.Red; // Rouge opaque
/// var customColor = Color.Create(128, 255, 0, 0); // Rouge semi-transparent
/// var hexColor = Color.FromHexString("#FF0000FF"); // Bleu opaque
/// var hex = customColor.ToHexString(); // "#80FF0000"
/// </code>
///   </para>
/// </remarks>
public record Color(byte A, byte R, byte G, byte B)
{
  #region Properties

  /// <summary>
  ///   Obtient une couleur transparente (ARGB: 0, 0, 0, 0).
  /// </summary>
  /// <value>Couleur transparent (Alpha=0).</value>
  public static Color Transparent => new(0, 0, 0, 0);

  /// <summary>
  ///   Obtient la couleur blanc opaque (ARGB: 255, 255, 255, 255).
  /// </summary>
  /// <value>Couleur blanc pur.</value>
  public static Color White => new(255, 255, 255, 255);

  /// <summary>
  ///   Obtient la couleur noir opaque (ARGB: 255, 0, 0, 0).
  /// </summary>
  /// <value>Couleur noir pur.</value>
  public static Color Black => new(255, 0, 0, 0);

  /// <summary>
  ///   Obtient la couleur rouge opaque (ARGB: 255, 255, 0, 0).
  /// </summary>
  /// <value>Couleur rouge pur.</value>
  public static Color Red => new(255, 255, 0, 0);

  /// <summary>
  ///   Obtient la couleur vert opaque (ARGB: 255, 0, 128, 0).
  /// </summary>
  /// <value>Couleur vert standard.</value>
  public static Color Green => new(255, 0, 128, 0);

  /// <summary>
  ///   Obtient la couleur bleu opaque (ARGB: 255, 0, 0, 255).
  /// </summary>
  /// <value>Couleur bleu pur.</value>
  public static Color Blue => new(255, 0, 0, 255);

  /// <summary>
  ///   Obtient la couleur jaune opaque (ARGB: 255, 255, 255, 0).
  /// </summary>
  /// <value>Couleur jaune.</value>
  public static Color Yellow => new(255, 255, 255, 0);

  /// <summary>
  ///   Obtient la couleur cyan opaque (ARGB: 255, 0, 255, 255).
  /// </summary>
  /// <value>Couleur cyan.</value>
  public static Color Cyan => new(255, 0, 255, 255);

  /// <summary>
  ///   Obtient la couleur magenta opaque (ARGB: 255, 255, 0, 255).
  /// </summary>
  /// <value>Couleur magenta.</value>
  public static Color Magenta => new(255, 255, 0, 255);

  /// <summary>
  ///   Obtient la couleur gris opaque (ARGB: 255, 128, 128, 128).
  /// </summary>
  /// <value>Couleur gris moyen.</value>
  public static Color Gray => new(255, 128, 128, 128);

  #endregion

  #region Methods

  /// <summary>
  ///   Crée une nouvelle couleur avec les composants spécifiés.
  /// </summary>
  /// <param name="a">Composant Alpha (transparence): 0 (transparent) à 255 (opaque).</param>
  /// <param name="r">Composant Red (rouge): 0 à 255.</param>
  /// <param name="g">Composant Green (vert): 0 à 255.</param>
  /// <param name="b">Composant Blue (bleu): 0 à 255.</param>
  /// <returns>Une nouvelle instance de Color.</returns>
  /// <example>
  ///   <code>
  /// var semiTransparentRed = Color.Create(128, 255, 0, 0);
  /// </code>
  /// </example>
  public static Color Create(byte a, byte r, byte g, byte b)
  {
    return new Color(a, r, g, b);
  }

  /// <summary>
  ///   Convertit la couleur en sa représentation hexadécimale.
  /// </summary>
  /// <returns>Chaîne au format "#AARRGGBB" (ex: "#FF0000FF" pour le bleu opaque).</returns>
  /// <example>
  ///   <code>
  /// var color = Color.Red;
  /// var hex = color.ToHexString(); // "#FFFF0000"
  /// </code>
  /// </example>
  public string ToHexString()
  {
    return $"#{A:X2}{R:X2}{G:X2}{B:X2}";
  }

  /// <summary>
  ///   Crée une couleur à partir d'une chaîne hexadécimale.
  /// </summary>
  /// <param name="hex">Chaîne au format "#AARRGGBB" (ex: "#FF0000FF").</param>
  /// <returns>Une nouvelle instance de Color.</returns>
  /// <exception cref="ArgumentException">Levée si le format n'est pas valide.</exception>
  /// <example>
  ///   <code>
  /// var blue = Color.FromHexString("#FF0000FF");
  /// var transparent = Color.FromHexString("#00FFFFFF");
  /// </code>
  /// </example>
  public static Color FromHexString(string hex)
  {
    if (string.IsNullOrWhiteSpace(hex) || !hex.StartsWith("#") || hex.Length != 9)
    {
      throw new ArgumentException("Format hex invalide. Utilisez #AARRGGBB");
    }

    var a = byte.Parse(hex.Substring(1, 2), NumberStyles.HexNumber);
    var r = byte.Parse(hex.Substring(3, 2), NumberStyles.HexNumber);
    var g = byte.Parse(hex.Substring(5, 2), NumberStyles.HexNumber);
    var b = byte.Parse(hex.Substring(7, 2), NumberStyles.HexNumber);

    return new Color(a, r, g, b);
  }

  #endregion
}
