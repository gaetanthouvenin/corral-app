// ------------------------------------------------------------------------------------------------
// <copyright file="ColorTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.ValueObjects;

namespace Corral.Domain.Tests.ValueObjects;

public class ColorTests
{
  #region Methods

  [Fact]
  public void Create_ShouldReturnColor_WithCorrectComponents()
  {
    var color = Color.Create(255, 128, 64, 32);

    color.A.ShouldBe((byte)255);
    color.R.ShouldBe((byte)128);
    color.G.ShouldBe((byte)64);
    color.B.ShouldBe((byte)32);
  }

  [Fact]
  public void FromHexString_ValidFormat_ShouldParseCorrectly()
  {
    var color = Color.FromHexString("#FF0078D4");

    color.A.ShouldBe((byte)255);
    color.R.ShouldBe((byte)0);
    color.G.ShouldBe((byte)120);
    color.B.ShouldBe((byte)212);
  }

  [Fact]
  public void FromHexString_LowercaseHex_ShouldParseCorrectly()
  {
    var color = Color.FromHexString("#ff00aabb");

    color.A.ShouldBe((byte)255);
    color.R.ShouldBe((byte)0);
    color.G.ShouldBe((byte)170);
    color.B.ShouldBe((byte)187);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  [InlineData("FF0078D4")]
  [InlineData("#FF007")]
  [InlineData("#FF0078D4FF")]
  [InlineData("not-a-color")]
  public void FromHexString_InvalidFormat_ShouldThrowArgumentException(string hex)
  {
    Should.Throw<ArgumentException>(() => Color.FromHexString(hex));
  }

  [Fact]
  public void ToHexString_ShouldReturnCorrectFormat()
  {
    var color = Color.Create(255, 0, 120, 212);

    color.ToHexString().ShouldBe("#FF0078D4");
  }

  [Fact]
  public void ToHexString_FromHexString_ShouldRoundTrip()
  {
    const string hex = "#80AABBCC";
    var color = Color.FromHexString(hex);

    color.ToHexString().ShouldBe(hex);
  }

  [Fact]
  public void Transparent_ShouldHaveZeroAlpha()
  {
    var color = Color.Transparent;

    color.A.ShouldBe((byte)0);
    color.R.ShouldBe((byte)0);
    color.G.ShouldBe((byte)0);
    color.B.ShouldBe((byte)0);
  }

  [Fact]
  public void White_ShouldBeFullyOpaqueWhite()
  {
    var color = Color.White;

    color.A.ShouldBe((byte)255);
    color.R.ShouldBe((byte)255);
    color.G.ShouldBe((byte)255);
    color.B.ShouldBe((byte)255);
  }

  [Fact]
  public void Black_ShouldBeFullyOpaqueBlack()
  {
    var color = Color.Black;

    color.A.ShouldBe((byte)255);
    color.R.ShouldBe((byte)0);
    color.G.ShouldBe((byte)0);
    color.B.ShouldBe((byte)0);
  }

  [Fact]
  public void Red_ShouldBeFullyOpaqueRed()
  {
    Color.Red.ShouldBe(new Color(255, 255, 0, 0));
  }

  [Fact]
  public void Green_ShouldBeFullyOpaqueGreen()
  {
    Color.Green.ShouldBe(new Color(255, 0, 128, 0));
  }

  [Fact]
  public void Blue_ShouldBeFullyOpaqueBlue()
  {
    Color.Blue.ShouldBe(new Color(255, 0, 0, 255));
  }

  [Fact]
  public void Equality_SameComponents_ShouldBeEqual()
  {
    var color1 = Color.Create(255, 100, 200, 50);
    var color2 = Color.Create(255, 100, 200, 50);

    color1.ShouldBe(color2);
  }

  [Fact]
  public void Equality_DifferentComponents_ShouldNotBeEqual()
  {
    var color1 = Color.Create(255, 100, 200, 50);
    var color2 = Color.Create(255, 100, 200, 51);

    color1.ShouldNotBe(color2);
  }

  #endregion
}
