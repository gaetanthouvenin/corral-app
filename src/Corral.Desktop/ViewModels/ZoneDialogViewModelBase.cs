// ------------------------------------------------------------------------------------------------
// <copyright file="ZoneDialogViewModelBase.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   Base class for zone dialog view models (create and edit).
///   Contains all shared state, color preset logic, and the cancel command.
/// </summary>
public abstract partial class ZoneDialogViewModelBase : ObservableObject
{
  #region Fields

  /// <summary>
  ///   Preview brush showing the currently selected color in the UI.
  /// </summary>
  [ObservableProperty]
  private SolidColorBrush _colorPreview = new(Colors.DodgerBlue);

  /// <summary>
  ///   Error message displayed to the user if the operation fails.
  /// </summary>
  [ObservableProperty]
  private string _errorMessage = string.Empty;

  /// <summary>
  ///   Opacity of the zone as a percentage from 0 to 100.
  /// </summary>
  [ObservableProperty]
  private int _opacity = 100;

  /// <summary>
  ///   Selected color in hexadecimal format <c>#AARRGGBB</c>.
  /// </summary>
  [ObservableProperty]
  private string _selectedColor = "#FF1A80E5";

  /// <summary>
  ///   Name of the zone.
  /// </summary>
  [ObservableProperty]
  private string _zoneName = string.Empty;

  #endregion

  #region Properties

  /// <summary>
  ///   Read-only collection of predefined color options.
  /// </summary>
  public IReadOnlyList<ColorPreset> ColorPresets { get; } =
  [
    new("#FF1A80E5", "Bleu"),
    new("#FFFF5277", "Rose"),
    new("#FF2BD2D2", "Cyan"),
    new("#FFFF9800", "Orange"),
    new("#FFA15BD7", "Violet"),
    new("#FF737373", "Gris"),
  ];

  /// <summary>
  ///   Callback invoked when the dialog should close.
  ///   <c>true</c> = operation succeeded; <c>false</c> = cancelled or failed.
  /// </summary>
  public Action<bool> CloseDialog { get; set; } = _ => { };

  #endregion

  #region Commands

  /// <summary>
  ///   Selects a predefined color and refreshes the preview brush.
  /// </summary>
  [RelayCommand]
  private void SelectColor(string colorHex)
  {
    SelectedColor = colorHex;
  }

  /// <summary>
  ///   Closes the dialog without saving.
  /// </summary>
  [RelayCommand]
  private void Cancel()
  {
    CloseDialog(false);
  }

  #endregion

  #region Helpers

  /// <summary>
  ///   Invoked when the <see cref="_selectedColor" /> property changes.
  /// </summary>
  /// <param name="value">The new value of the selected color.</param>
  partial void OnSelectedColorChanged(string value)
  {
    UpdateColorPreview();
  }

  /// <summary>
  ///   Invoked when the value of the <see cref="_zoneName" /> property changes.
  /// </summary>
  /// <param name="value">The new value of the <see cref="_zoneName" /> property.</param>
  partial void OnZoneNameChanged(string value)
  {
    RefreshCanExecute();
  }

  /// <summary>
  ///   Called when zone name changes to allow derived classes to re-evaluate their command's
  ///   CanExecute. Override in each derived class to call
  ///   <c>YourCommand.NotifyCanExecuteChanged()</c>.
  /// </summary>
  protected virtual void RefreshCanExecute()
  {
  }

  private void UpdateColorPreview()
  {
    try
    {
      if (ColorConverter.ConvertFromString(SelectedColor) is Color color)
      {
        ColorPreview = new SolidColorBrush(color);
      }
      else
      {
        ColorPreview = new SolidColorBrush(Colors.Gray);
      }
    }
    catch
    {
      ColorPreview = new SolidColorBrush(Colors.Gray);
    }
  }

  #endregion
}
