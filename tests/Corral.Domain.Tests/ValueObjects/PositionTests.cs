// ------------------------------------------------------------------------------------------------
// <copyright file="PositionTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.ValueObjects;

namespace Corral.Domain.Tests.ValueObjects;

public class PositionTests
{
  #region Methods

  [Fact]
  public void Create_ValidCoordinates_ShouldReturnPosition()
  {
    var position = Position.Create(100, 200);

    position.X.ShouldBe(100);
    position.Y.ShouldBe(200);
  }

  [Fact]
  public void Create_ZeroCoordinates_ShouldReturnPosition()
  {
    var position = Position.Create(0, 0);

    position.X.ShouldBe(0);
    position.Y.ShouldBe(0);
  }

  [Fact]
  public void Create_NegativeX_ShouldThrowInvalidOperationException()
  {
    Should.Throw<InvalidOperationException>(() => Position.Create(-1, 0));
  }

  [Fact]
  public void Create_NegativeY_ShouldThrowInvalidOperationException()
  {
    Should.Throw<InvalidOperationException>(() => Position.Create(0, -1));
  }

  [Fact]
  public void Origin_ShouldBeZeroZero()
  {
    var origin = Position.Origin;

    origin.X.ShouldBe(0);
    origin.Y.ShouldBe(0);
  }

  [Fact]
  public void DistanceTo_SamePosition_ShouldBeZero()
  {
    var pos = Position.Create(100, 200);

    pos.DistanceTo(pos).ShouldBe(0.0);
  }

  [Fact]
  public void DistanceTo_KnownDistance_ShouldReturnCorrectValue()
  {
    var pos1 = Position.Create(0, 0);
    var pos2 = Position.Create(3, 4);

    pos1.DistanceTo(pos2).ShouldBe(5.0);
  }

  [Fact]
  public void DistanceTo_ShouldBeSymmetric()
  {
    var pos1 = Position.Create(10, 20);
    var pos2 = Position.Create(30, 40);

    pos1.DistanceTo(pos2).ShouldBe(pos2.DistanceTo(pos1));
  }

  [Fact]
  public void Equality_SameCoordinates_ShouldBeEqual()
  {
    var pos1 = Position.Create(100, 200);
    var pos2 = Position.Create(100, 200);

    pos1.ShouldBe(pos2);
  }

  [Fact]
  public void Equality_DifferentCoordinates_ShouldNotBeEqual()
  {
    var pos1 = Position.Create(100, 200);
    var pos2 = Position.Create(100, 201);

    pos1.ShouldNotBe(pos2);
  }

  #endregion
}
