// ------------------------------------------------------------------------------------------------
// <copyright file="InvalidFenceException.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.Exceptions;

/// <summary>
///   Represents an exception that is thrown when an operation on a <c>Fence</c> is invalid.
/// </summary>
public class InvalidFenceException : Exception
{
  #region Ctors

  /// <summary>
  ///   Initializes a new instance of the <see cref="InvalidFenceException" /> class with a specified
  ///   error message.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  public InvalidFenceException(string message)
    : base(message)
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="InvalidFenceException" /> class with a specified
  ///   error message and a reference to the inner exception that is the cause of this exception.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  /// <param name="innerException">
  ///   The exception that is the cause of the current exception, or a null reference
  ///   (<c>null</c>) if no inner exception is specified.
  /// </param>
  public InvalidFenceException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  #endregion
}
