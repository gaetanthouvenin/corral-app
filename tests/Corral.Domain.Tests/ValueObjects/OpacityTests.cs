// ------------------------------------------------------------------------------------------------
// <copyright file="OpacityTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.ValueObjects;

namespace Corral.Domain.Tests.ValueObjects;

public class OpacityTests
{
  #region Methods

  [Fact]
  public void Create_ValidPercentage_ShouldReturnOpacity()
  {
    var opacity = Opacity.Create(75);

    opacity.Percentage.ShouldBe(75);
  }

  [Fact]
  public void Create_MinValue_ShouldSucceed()
  {
    var opacity = Opacity.Create(0);

    opacity.Percentage.ShouldBe(0);
  }

  [Fact]
  public void Create_MaxValue_ShouldSucceed()
  {
    var opacity = Opacity.Create(100);

    opacity.Percentage.ShouldBe(100);
  }

  [Fact]
  public void Create_BelowMin_ShouldThrow()
  {
    Should.Throw<InvalidOperationException>(() => Opacity.Create(-1));
  }

  [Fact]
  public void Create_AboveMax_ShouldThrow()
  {
    Should.Throw<InvalidOperationException>(() => Opacity.Create(101));
  }

  [Fact]
  public void Transparent_ShouldBeZero()
  {
    Opacity.Transparent.Percentage.ShouldBe(0);
  }

  [Fact]
  public void SemiTransparent_ShouldBeFifty()
  {
    Opacity.SemiTransparent.Percentage.ShouldBe(50);
  }

  [Fact]
  public void Opaque_ShouldBeHundred()
  {
    Opacity.Opaque.Percentage.ShouldBe(100);
  }

  [Fact]
  public void ToNormalized_ShouldReturnCorrectValue()
  {
    var opacity = Opacity.Create(75);

    opacity.ToNormalized().ShouldBe(0.75);
  }

  [Fact]
  public void ToNormalized_Zero_ShouldReturnZero()
  {
    Opacity.Transparent.ToNormalized().ShouldBe(0.0);
  }

  [Fact]
  public void ToNormalized_Hundred_ShouldReturnOne()
  {
    Opacity.Opaque.ToNormalized().ShouldBe(1.0);
  }

  [Fact]
  public void FromNormalized_ValidValue_ShouldReturnCorrectPercentage()
  {
    var opacity = Opacity.FromNormalized(0.75);

    opacity.Percentage.ShouldBe(75);
  }

  [Fact]
  public void FromNormalized_Zero_ShouldReturnTransparent()
  {
    var opacity = Opacity.FromNormalized(0.0);

    opacity.Percentage.ShouldBe(0);
  }

  [Fact]
  public void FromNormalized_One_ShouldReturnOpaque()
  {
    var opacity = Opacity.FromNormalized(1.0);

    opacity.Percentage.ShouldBe(100);
  }

  [Fact]
  public void FromNormalized_BelowZero_ShouldThrow()
  {
    Should.Throw<InvalidOperationException>(() => Opacity.FromNormalized(-0.1));
  }

  [Fact]
  public void FromNormalized_AboveOne_ShouldThrow()
  {
    Should.Throw<InvalidOperationException>(() => Opacity.FromNormalized(1.1));
  }

  [Fact]
  public void Equality_SamePercentage_ShouldBeEqual()
  {
    var op1 = Opacity.Create(50);
    var op2 = Opacity.Create(50);

    op1.ShouldBe(op2);
  }

  #endregion
}
