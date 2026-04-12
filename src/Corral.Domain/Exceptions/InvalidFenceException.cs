// ------------------------------------------------------------------------------------------------
// <copyright file="InvalidFenceException.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.Exceptions;

/// <summary>
///   Exception levée quand une opération sur une Fence est invalide
/// </summary>
public class InvalidFenceException : Exception
{
  #region Ctors

  public InvalidFenceException(string message)
    : base(message)
  {
  }

  public InvalidFenceException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  #endregion
}
