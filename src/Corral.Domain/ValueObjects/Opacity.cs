// ------------------------------------------------------------------------------------------------
// <copyright file="Opacity.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.ValueObjects;

/// <summary>
///   Represents the opacity (transparency) level of a Fence, expressed as a percentage (0-100%).
/// </summary>
/// <remarks>
///   <para>
///     Opacity is an immutable Value Object representing the degree of transparency of a Fence zone.
///     Valid values range from 0 (completely transparent) to 100 (completely opaque).
///   </para>
///   <para>
///     The record also provides conversions to and from a normalized representation (0.0-1.0)
///     commonly used in graphics frameworks.
///   </para>
///   <para>
///     Usage examples:
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
  ///   The minimum opacity value (completely transparent).
  /// </summary>
  /// <value>0 (0%).</value>
  public const int Min = 0;

  /// <summary>
  ///   The maximum opacity value (completely opaque).
  /// </summary>
  /// <value>100 (100%).</value>
  public const int Max = 100;

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the completely transparent opacity level (0%).
  /// </summary>
  /// <value>An <see cref="Opacity" /> instance with Percentage = 0.</value>
  public static Opacity Transparent => new(0);

  /// <summary>
  ///   Gets the semi-transparent opacity level (50%).
  /// </summary>
  /// <value>An <see cref="Opacity" /> instance with Percentage = 50.</value>
  public static Opacity SemiTransparent => new(50);

  /// <summary>
  ///   Gets the completely opaque opacity level (100%).
  /// </summary>
  /// <value>An <see cref="Opacity" /> instance with Percentage = 100.</value>
  public static Opacity Opaque => new(100);

  #endregion

  #region Methods

  /// <summary>
  ///   Creates a new opacity level with validation.
  /// </summary>
  /// <param name="percentage">The opacity percentage. Must be between 0 and 100 (inclusive).</param>
  /// <returns>A new <see cref="Opacity" /> instance.</returns>
  /// <exception cref="InvalidOperationException">
  ///   Thrown if the percentage is outside the range [0, 100].
  /// </exception>
  /// <example>
  ///   <code>
  /// var opacity = Opacity.Create(75); // 75% opaque
  /// </code>
  /// </example>
  public static Opacity Create(int percentage)
  {
    return percentage is < Min or > Max
             ? throw new InvalidOperationException($"Opacity must be between {Min} and {Max}")
             : new Opacity(percentage);
  }

  /// <summary>
  ///   Converts the opacity to a normalized value (between 0.0 and 1.0).
  /// </summary>
  /// <returns>
  ///   A normalized value where 0.0 = transparent and 1.0 = opaque.
  /// </returns>
  /// <remarks>
  ///   This conversion is useful for graphics frameworks that use a normalized
  ///   representation of opacity.
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
  ///   Creates an opacity level from a normalized value (between 0.0 and 1.0).
  /// </summary>
  /// <param name="normalized">
  ///   The normalized value. Must be between 0.0 and 1.0 (inclusive).
  /// </param>
  /// <returns>A new <see cref="Opacity" /> instance.</returns>
  /// <exception cref="InvalidOperationException">
  ///   Thrown if the normalized value is outside the range [0.0, 1.0].
  /// </exception>
  /// <remarks>The conversion rounds the result to the nearest integer.</remarks>
  /// <example>
  ///   <code>
  /// var opacity = Opacity.FromNormalized(0.75); // 75% opaque
  /// </code>
  /// </example>
  public static Opacity FromNormalized(double normalized)
  {
    return normalized is < 0.0 or > 1.0
             ? throw new InvalidOperationException("Normalized value must be between 0.0 and 1.0")
             : new Opacity((int)(normalized * 100));
  }

  #endregion
}
