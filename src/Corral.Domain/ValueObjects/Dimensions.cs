// ------------------------------------------------------------------------------------------------
// <copyright file="Dimensions.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.ValueObjects;

/// <summary>
///   Represents the dimensions (width × height) of a Fence zone in pixels.
/// </summary>
/// <remarks>
///   <para>
///     Dimensions is an immutable Value Object that encapsulates size constraints
///     for virtual zones (Fences). Dimensions must fall within the allowed limits
///     defined by the MinWidth, MinHeight, MaxWidth, and MaxHeight constants.
///   </para>
///   <para>
///     Usage examples:
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
  ///   The minimum allowed width for a zone in pixels.
  /// </summary>
  /// <value>50 pixels.</value>
  public const int MinWidth = 50;

  /// <summary>
  ///   The minimum allowed height for a zone in pixels.
  /// </summary>
  /// <value>50 pixels.</value>
  public const int MinHeight = 50;

  /// <summary>
  ///   The maximum allowed width for a zone in pixels.
  /// </summary>
  /// <value>2560 pixels (4K resolution).</value>
  public const int MaxWidth = 2560;

  /// <summary>
  ///   The maximum allowed height for a zone in pixels.
  /// </summary>
  /// <value>1440 pixels (2K resolution).</value>
  public const int MaxHeight = 1440;

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the default dimensions (200×200 pixels).
  /// </summary>
  /// <value>A <see cref="Dimensions" /> instance with Width = 200 and Height = 200.</value>
  public static Dimensions Default => new(200, 200);

  /// <summary>
  ///   Calculates the area of the zone in square pixels.
  /// </summary>
  /// <value>The area equal to Width × Height.</value>
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
  ///   Creates new dimensions with constraint validation.
  /// </summary>
  /// <param name="width">
  ///   Width in pixels. Must be between <see cref="MinWidth" /> and
  ///   <see cref="MaxWidth" />.
  /// </param>
  /// <param name="height">
  ///   Height in pixels. Must be between <see cref="MinHeight" /> and
  ///   <see cref="MaxHeight" />.
  /// </param>
  /// <returns>A new <see cref="Dimensions" /> instance.</returns>
  /// <exception cref="InvalidOperationException">
  ///   Thrown if the dimensions do not meet the constraints.
  /// </exception>
  /// <example>
  ///   <code>
  /// var size = Dimensions.Create(400, 300); // Valid
  /// var invalid = Dimensions.Create(30, 40); // Throws InvalidOperationException
  /// </code>
  /// </example>
  public static Dimensions Create(int width, int height)
  {
    if (width < MinWidth)
    {
      throw new InvalidOperationException($"Width must be >= {MinWidth}");
    }

    if (height < MinHeight)
    {
      throw new InvalidOperationException($"Height must be >= {MinHeight}");
    }

    if (width > MaxWidth)
    {
      throw new InvalidOperationException($"Width must be <= {MaxWidth}");
    }

    if (height > MaxHeight)
    {
      throw new InvalidOperationException($"Height must be <= {MaxHeight}");
    }

    return new Dimensions(width, height);
  }

  /// <summary>
  ///   Determines whether a given position is contained within the bounds of this zone.
  /// </summary>
  /// <param name="position">The position to test.</param>
  /// <returns>
  ///   <c>true</c> if (0 ≤ position.X &lt; Width) and (0 ≤ position.Y &lt; Height); otherwise,
  ///   <c>false</c>.
  /// </returns>
  /// <remarks>
  ///   The check uses inclusive bounds for X and Y (minimum) and exclusive bounds (maximum).
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
