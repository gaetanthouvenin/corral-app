// ------------------------------------------------------------------------------------------------
// <copyright file="Color.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using System.Globalization;

namespace Corral.Domain.ValueObjects;

/// <summary>
///   Represents an immutable ARGB (Alpha, Red, Green, Blue) color.
/// </summary>
/// <remarks>
///   <para>
///     Color is an immutable Value Object representing a color in ARGB format (32-bit).
///     Each component (Alpha, Red, Green, Blue) is a byte (0-255).
///     The Alpha component represents transparency: 0 = transparent, 255 = opaque.
///   </para>
///   <para>
///     Usage examples:
///     <code>
/// var red = Color.Red; // Opaque red
/// var customColor = Color.Create(128, 255, 0, 0); // Semi-transparent red
/// var hexColor = Color.FromHexString("#FF0000FF"); // Opaque blue
/// var hex = customColor.ToHexString(); // "#80FF0000"
/// </code>
///   </para>
/// </remarks>
public record Color(byte A, byte R, byte G, byte B)
{
  #region Properties

  /// <summary>
  ///   Gets a transparent color (ARGB: 0, 0, 0, 0).
  /// </summary>
  /// <value>Transparent color (Alpha = 0).</value>
  public static Color Transparent => new(0, 0, 0, 0);

  /// <summary>
  ///   Gets opaque white (ARGB: 255, 255, 255, 255).
  /// </summary>
  /// <value>Pure white.</value>
  public static Color White => new(255, 255, 255, 255);

  /// <summary>
  ///   Gets opaque black (ARGB: 255, 0, 0, 0).
  /// </summary>
  /// <value>Pure black.</value>
  public static Color Black => new(255, 0, 0, 0);

  /// <summary>
  ///   Gets opaque red (ARGB: 255, 255, 0, 0).
  /// </summary>
  /// <value>Pure red.</value>
  public static Color Red => new(255, 255, 0, 0);

  /// <summary>
  ///   Gets opaque green (ARGB: 255, 0, 128, 0).
  /// </summary>
  /// <value>Standard green.</value>
  public static Color Green => new(255, 0, 128, 0);

  /// <summary>
  ///   Gets opaque blue (ARGB: 255, 0, 0, 255).
  /// </summary>
  /// <value>Pure blue.</value>
  public static Color Blue => new(255, 0, 0, 255);

  /// <summary>
  ///   Gets opaque yellow (ARGB: 255, 255, 255, 0).
  /// </summary>
  /// <value>Pure yellow.</value>
  public static Color Yellow => new(255, 255, 255, 0);

  /// <summary>
  ///   Gets opaque cyan (ARGB: 255, 0, 255, 255).
  /// </summary>
  /// <value>Pure cyan.</value>
  public static Color Cyan => new(255, 0, 255, 255);

  /// <summary>
  ///   Gets opaque magenta (ARGB: 255, 255, 0, 255).
  /// </summary>
  /// <value>Pure magenta.</value>
  public static Color Magenta => new(255, 255, 0, 255);

  /// <summary>
  ///   Gets opaque gray (ARGB: 255, 128, 128, 128).
  /// </summary>
  /// <value>Medium gray.</value>
  public static Color Gray => new(255, 128, 128, 128);

  #endregion

  #region Methods

  /// <summary>
  ///   Creates a new color with the specified component values.
  /// </summary>
  /// <param name="a">Alpha component (transparency): 0 (transparent) to 255 (opaque).</param>
  /// <param name="r">Red component: 0 to 255.</param>
  /// <param name="g">Green component: 0 to 255.</param>
  /// <param name="b">Blue component: 0 to 255.</param>
  /// <returns>A new <see cref="Color" /> instance with the specified ARGB values.</returns>
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
  ///   Converts the color to its hexadecimal string representation.
  /// </summary>
  /// <returns>
  ///   A string in <c>#AARRGGBB</c> format (e.g., <c>#FF0000FF</c> for opaque blue).
  /// </returns>
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
  ///   Creates a color from a hexadecimal string representation.
  /// </summary>
  /// <param name="hex">
  ///   A string in <c>#AARRGGBB</c> format (e.g., <c>#FF0000FF</c> for opaque blue).
  /// </param>
  /// <returns>A new <see cref="Color" /> instance parsed from the hexadecimal string.</returns>
  /// <exception cref="ArgumentException">
  ///   Thrown if the format is invalid. The string must be exactly 9 characters long,
  ///   start with '#', and contain valid hexadecimal digits.
  /// </exception>
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
      throw new ArgumentException("Invalid hex format. Use #AARRGGBB");
    }

    var a = byte.Parse(hex.Substring(1, 2), NumberStyles.HexNumber);
    var r = byte.Parse(hex.Substring(3, 2), NumberStyles.HexNumber);
    var g = byte.Parse(hex.Substring(5, 2), NumberStyles.HexNumber);
    var b = byte.Parse(hex.Substring(7, 2), NumberStyles.HexNumber);

    return new Color(a, r, g, b);
  }

  #endregion
}
