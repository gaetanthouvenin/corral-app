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

  public static FenceId Create()
  {
    return new FenceId(Guid.NewGuid().ToString());
  }

  public static FenceId Create(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("FenceId ne peut pas être vide");
    }

    return new FenceId(value);
  }

  public override string ToString()
  {
    return Value;
  }

  #endregion
}
