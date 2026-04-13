// ------------------------------------------------------------------------------------------------
// <copyright file="Position.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.ValueObjects;

/// <summary>
///   Represents a position (X, Y) on the screen in pixel coordinates.
/// </summary>
/// <remarks>
///   <para>
///     Position is an immutable Value Object representing a point in 2D space on the screen.
///     The X and Y coordinates are expressed in pixels and must be non-negative.
///   </para>
///   <para>
///     Usage examples:
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
  ///   Gets the position at the origin (0, 0).
  /// </summary>
  /// <value>A <see cref="Position" /> instance with X = 0 and Y = 0.</value>
  public static Position Origin => new(0, 0);

  #endregion

  #region Methods

  /// <summary>
  ///   Creates a new position instance with validation.
  /// </summary>
  /// <param name="x">The X coordinate in pixels. Must be >= 0.</param>
  /// <param name="y">The Y coordinate in pixels. Must be >= 0.</param>
  /// <returns>A new <see cref="Position" /> instance.</returns>
  /// <exception cref="InvalidOperationException">
  ///   Thrown if X or Y is negative.
  /// </exception>
  /// <example>
  ///   <code>
  /// var position = Position.Create(150, 250);
  /// </code>
  /// </example>
  public static Position Create(int x, int y)
  {
    if (x < 0)
    {
      throw new InvalidOperationException("X must be non-negative");
    }

    if (y < 0)
    {
      throw new InvalidOperationException("Y must be non-negative");
    }

    return new Position(x, y);
  }

  /// <summary>
  ///   Calculates the Euclidean distance between this position and another.
  /// </summary>
  /// <param name="other">The other position.</param>
  /// <returns>The distance in pixels between the two positions.</returns>
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
