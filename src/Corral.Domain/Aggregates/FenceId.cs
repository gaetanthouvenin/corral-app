// ------------------------------------------------------------------------------------------------
// <copyright file="FenceId.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.Aggregates;

/// <summary>
///   Strongly-typed ID pour une Fence (Entity ID)
/// </summary>
public record FenceId(string Value)
{
  #region Methods

  /// <summary>
  ///   Creates a new instance of <see cref="FenceId" /> with a unique identifier.
  /// </summary>
  /// <returns>A new <see cref="FenceId" /> instance containing a generated unique identifier.</returns>
  public static FenceId Create()
  {
    return new FenceId(Guid.NewGuid().ToString());
  }

  /// <summary>
  ///   Creates a new instance of the <see cref="FenceId" /> class with the specified value.
  /// </summary>
  /// <param name="value">The string value representing the fence identifier.</param>
  /// <returns>A new <see cref="FenceId" /> instance.</returns>
  /// <exception cref="ArgumentException">
  ///   Thrown when the provided <paramref name="value" /> is null,
  ///   empty, or consists only of whitespace.
  /// </exception>
  public static FenceId Create(string value)
  {
    return string.IsNullOrWhiteSpace(value)
             ? throw new ArgumentException("FenceId ne peut pas être vide")
             : new FenceId(value);
  }

  /// <summary>
  ///   Returns the string representation of the current <see cref="FenceId" /> instance.
  /// </summary>
  /// <returns>The string value of the fence identifier.</returns>
  public override string ToString()
  {
    return Value;
  }

  #endregion
}
