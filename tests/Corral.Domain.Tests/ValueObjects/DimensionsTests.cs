// ------------------------------------------------------------------------------------------------
// <copyright file="DimensionsTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.ValueObjects;

namespace Corral.Domain.Tests.ValueObjects;

public class DimensionsTests
{
  #region Methods

  [Fact]
  public void Create_ValidDimensions_ShouldReturnDimensions()
  {
    var dim = Dimensions.Create(400, 300);

    dim.Width.ShouldBe(400);
    dim.Height.ShouldBe(300);
  }

  [Fact]
  public void Create_MinimumDimensions_ShouldSucceed()
  {
    var dim = Dimensions.Create(Dimensions.MinWidth, Dimensions.MinHeight);

    dim.Width.ShouldBe(50);
    dim.Height.ShouldBe(50);
  }

  [Fact]
  public void Create_MaximumDimensions_ShouldSucceed()
  {
    var dim = Dimensions.Create(Dimensions.MaxWidth, Dimensions.MaxHeight);

    dim.Width.ShouldBe(2560);
    dim.Height.ShouldBe(1440);
  }

  [Fact]
  public void Create_WidthBelowMinimum_ShouldThrow()
  {
    Should.Throw<InvalidOperationException>(() => Dimensions.Create(49, 100));
  }

  [Fact]
  public void Create_HeightBelowMinimum_ShouldThrow()
  {
    Should.Throw<InvalidOperationException>(() => Dimensions.Create(100, 49));
  }

  [Fact]
  public void Create_WidthAboveMaximum_ShouldThrow()
  {
    Should.Throw<InvalidOperationException>(() => Dimensions.Create(2561, 100));
  }

  [Fact]
  public void Create_HeightAboveMaximum_ShouldThrow()
  {
    Should.Throw<InvalidOperationException>(() => Dimensions.Create(100, 1441));
  }

  [Fact]
  public void Default_ShouldBe200x200()
  {
    var dim = Dimensions.Default;

    dim.Width.ShouldBe(200);
    dim.Height.ShouldBe(200);
  }

  [Fact]
  public void Area_ShouldReturnWidthTimesHeight()
  {
    var dim = Dimensions.Create(400, 300);

    dim.Area.ShouldBe(120000);
  }

  [Fact]
  public void Contains_PositionInside_ShouldReturnTrue()
  {
    var dim = Dimensions.Create(200, 200);
    var pos = Position.Create(100, 100);

    dim.Contains(pos).ShouldBeTrue();
  }

  [Fact]
  public void Contains_PositionAtOrigin_ShouldReturnTrue()
  {
    var dim = Dimensions.Create(200, 200);

    dim.Contains(Position.Origin).ShouldBeTrue();
  }

  [Fact]
  public void Contains_PositionOutside_ShouldReturnFalse()
  {
    var dim = Dimensions.Create(200, 200);
    var pos = Position.Create(250, 150);

    dim.Contains(pos).ShouldBeFalse();
  }

  [Fact]
  public void Contains_PositionAtBoundary_ShouldReturnFalse()
  {
    var dim = Dimensions.Create(200, 200);
    var pos = Position.Create(200, 200);

    dim.Contains(pos).ShouldBeFalse();
  }

  [Fact]
  public void Equality_SameDimensions_ShouldBeEqual()
  {
    var dim1 = Dimensions.Create(400, 300);
    var dim2 = Dimensions.Create(400, 300);

    dim1.ShouldBe(dim2);
  }

  #endregion
}
