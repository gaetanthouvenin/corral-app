// ------------------------------------------------------------------------------------------------
// <copyright file="PipelineBehaviorTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Application.Behaviors;
using Corral.Application.Commands.CreateFence;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace Corral.Application.Tests.Behaviors;

public class PipelineBehaviorTests
{
  #region Methods

  [Fact]
  public async Task LoggingBehavior_ShouldCallNextAndReturnResponse()
  {
    var logger = new Mock<ILogger<LoggingBehavior<CreateFenceCommand, string>>>();
    var behavior = new LoggingBehavior<CreateFenceCommand, string>(logger.Object);

    var result = await behavior.Handle(
                   new CreateFenceCommand("Fence", 0, 0, 200, 200, "#FFFFFFFF", 100),
                   _ => Task.FromResult("ok"),
                   CancellationToken.None
                 );

    result.ShouldBe("ok");
  }

  [Fact]
  public async Task LoggingBehavior_WhenNextThrows_ShouldRethrow()
  {
    var logger = new Mock<ILogger<LoggingBehavior<CreateFenceCommand, string>>>();
    var behavior = new LoggingBehavior<CreateFenceCommand, string>(logger.Object);

    await Should.ThrowAsync<InvalidOperationException>(() => behavior.Handle(
                                                         new CreateFenceCommand(
                                                           "Fence",
                                                           0,
                                                           0,
                                                           200,
                                                           200,
                                                           "#FFFFFFFF",
                                                           100
                                                         ),
                                                         _ => Task.FromException<string>(
                                                           new InvalidOperationException("boom")
                                                         ),
                                                         CancellationToken.None
                                                       )
    );
  }

  [Fact]
  public async Task ValidationBehavior_WithoutValidators_ShouldCallNext()
  {
    var logger = new Mock<ILogger<ValidationBehavior<CreateFenceCommand, string>>>();
    var behavior = new ValidationBehavior<CreateFenceCommand, string>([], logger.Object);

    var result = await behavior.Handle(
                   new CreateFenceCommand("Fence", 0, 0, 200, 200, "#FFFFFFFF", 100),
                   _ => Task.FromResult("ok"),
                   CancellationToken.None
                 );

    result.ShouldBe("ok");
  }

  [Fact]
  public async Task ValidationBehavior_WithValidRequest_ShouldCallNext()
  {
    var logger = new Mock<ILogger<ValidationBehavior<CreateFenceCommand, string>>>();
    var validators = new IValidator<CreateFenceCommand>[] { new CreateFenceCommandValidator() };
    var behavior = new ValidationBehavior<CreateFenceCommand, string>(validators, logger.Object);

    var result = await behavior.Handle(
                   new CreateFenceCommand("Fence", 0, 0, 200, 200, "#FFFFFFFF", 100),
                   _ => Task.FromResult("ok"),
                   CancellationToken.None
                 );

    result.ShouldBe("ok");
  }

  [Fact]
  public async Task ValidationBehavior_WithInvalidRequest_ShouldThrowValidationException()
  {
    var logger = new Mock<ILogger<ValidationBehavior<CreateFenceCommand, string>>>();
    var validators = new IValidator<CreateFenceCommand>[] { new CreateFenceCommandValidator() };
    var behavior = new ValidationBehavior<CreateFenceCommand, string>(validators, logger.Object);

    var exception = await Should.ThrowAsync<ValidationException>(() => behavior.Handle(
                                                                   new CreateFenceCommand(
                                                                     "",
                                                                     0,
                                                                     0,
                                                                     10,
                                                                     10,
                                                                     "bad",
                                                                     101
                                                                   ),
                                                                   _ => Task.FromResult("ok"),
                                                                   CancellationToken.None
                                                                 )
                    );

    exception.Errors.ShouldNotBeEmpty();
  }

  #endregion
}
