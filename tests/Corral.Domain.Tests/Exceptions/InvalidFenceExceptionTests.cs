// ------------------------------------------------------------------------------------------------
// <copyright file="InvalidFenceExceptionTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Exceptions;

namespace Corral.Domain.Tests.Exceptions;

public class InvalidFenceExceptionTests
{
  #region Methods

  [Fact]
  public void Constructor_WithMessage_ShouldSetMessage()
  {
    var exception = new InvalidFenceException("invalid");

    exception.Message.ShouldBe("invalid");
  }

  [Fact]
  public void Constructor_WithInnerException_ShouldSetProperties()
  {
    var innerException = new InvalidOperationException("boom");

    var exception = new InvalidFenceException("invalid", innerException);

    exception.Message.ShouldBe("invalid");
    exception.InnerException.ShouldBeSameAs(innerException);
  }

  #endregion
}
