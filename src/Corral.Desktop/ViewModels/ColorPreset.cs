// ------------------------------------------------------------------------------------------------
// <copyright file="ColorPreset.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Desktop.ViewModels;

/// <summary>
///   Represents a predefined color with its hexadecimal code and label.
/// </summary>
/// <param name="Hex">The color code in the format #AARRGGBB.</param>
/// <param name="Label">The human-readable name of the color.</param>
public record ColorPreset(string Hex, string Label);
