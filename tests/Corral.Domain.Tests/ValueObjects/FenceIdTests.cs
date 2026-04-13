// ------------------------------------------------------------------------------------------------
// <copyright file="FenceIdTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;

namespace Corral.Domain.Tests.ValueObjects;

public class FenceIdTests
{
  #region Methods

  [Fact]
  public void Create_NoArgs_ShouldGenerateUniqueGuid()
  {
    var id1 = FenceId.Create();
    var id2 = FenceId.Create();

    id1.Value.ShouldNotBeNullOrWhiteSpace();
    id2.Value.ShouldNotBeNullOrWhiteSpace();
    id1.ShouldNotBe(id2);
  }

  [Fact]
  public void Create_WithValue_ShouldReturnFenceIdWithValue()
  {
    var id = FenceId.Create("my-fence-id");

    id.Value.ShouldBe("my-fence-id");
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Create_WithEmptyOrNull_ShouldThrowArgumentException(string value)
  {
    Should.Throw<ArgumentException>(() => FenceId.Create(value));
  }

  [Fact]
  public void ToString_ShouldReturnValue()
  {
    var id = FenceId.Create("test-id");

    id.ToString().ShouldBe("test-id");
  }

  [Fact]
  public void Equality_SameValue_ShouldBeEqual()
  {
    var id1 = FenceId.Create("same-id");
    var id2 = FenceId.Create("same-id");

    id1.ShouldBe(id2);
  }

  [Fact]
  public void Equality_DifferentValue_ShouldNotBeEqual()
  {
    var id1 = FenceId.Create("id-1");
    var id2 = FenceId.Create("id-2");

    id1.ShouldNotBe(id2);
  }

  #endregion
}
