// ------------------------------------------------------------------------------------------------
// <copyright file="EditZoneDialogViewModel.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using System.Collections.ObjectModel;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corral.Application.Commands.UpdateFence;
using Corral.Desktop.Converters;

using MediatR;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   ViewModel for the Edit Zone dialog.
/// </summary>
public partial class EditZoneDialogViewModel(IMediator mediator) : ObservableObject
{
  #region Design-time constructor

  /// <summary>
  ///   Design-time constructor for XAML preview support.
  /// </summary>
  public EditZoneDialogViewModel() : this(null)
  {
    // Populate with design-time data
    ZoneName = "Sample Zone";
    SelectedColor = "#FF0078D4";
    Opacity = 85;
    UpdateColorPreview();
  }

  #endregion

  #region Fields

  [ObservableProperty]
  private string _zoneName = string.Empty;

  [ObservableProperty]
  private string _selectedColor = "#FFFFFFFF";

  [ObservableProperty]
  private Color _colorPreview = Colors.White;

  [ObservableProperty]
  private int _opacity = 100;

  [ObservableProperty]
  private bool _isUpdating;

  [ObservableProperty]
  private string _errorMessage = string.Empty;

  [ObservableProperty]
  private string _fenceId = string.Empty;

  #endregion

  #region Color Presets

  /// <summary>
  ///   Predefined color options for quick selection.
  /// </summary>
  public ReadOnlyCollection<string> ColorPresets => new(new[]
  {
    "#FF0078D4", // Blue
    "#FFFF8C00", // Orange
    "#FF9966CC", // Purple
    "#FF00B050", // Green
    "#FFFF0000", // Red
    "#FFFFFF00", // Yellow
  });

  #endregion

  #region Methods

  /// <summary>
  ///   Updates the color preview based on the selected color.
  /// </summary>
  private void UpdateColorPreview()
  {
    if (string.IsNullOrEmpty(SelectedColor))
    {
      ColorPreview = Colors.White;
      return;
    }

    try
    {
      if (ColorConverter.ConvertFromString(SelectedColor) is Color color)
      {
        ColorPreview = color;
      }
    }
    catch
    {
      ColorPreview = Colors.White;
    }
  }

  /// <summary>
  ///   Initializes the view model with fence data for editing.
  /// </summary>
  /// <param name="fenceId">The ID of the fence to edit.</param>
  /// <param name="name">The current name of the fence.</param>
  /// <param name="color">The current color in #AARRGGBB format.</param>
  /// <param name="opacity">The current opacity (0-100).</param>
  public void Initialize(string fenceId, string name, string color, int opacity)
  {
    FenceId = fenceId;
    ZoneName = name;
    SelectedColor = color;
    Opacity = opacity;
    ErrorMessage = string.Empty;
    UpdateColorPreview();
  }

  #endregion

  #region Commands

  /// <summary>
  ///   Command to select a color from the preset list.
  /// </summary>
  [RelayCommand]
  public void SelectColor(string color)
  {
    SelectedColor = color;
    UpdateColorPreview();
  }

  /// <summary>
  ///   Command to update the zone with new values.
  /// </summary>
  [RelayCommand]
  public async Task UpdateZone()
  {
    if (string.IsNullOrEmpty(ZoneName))
    {
      ErrorMessage = "Zone name is required";
      return;
    }

    if (string.IsNullOrEmpty(SelectedColor))
    {
      ErrorMessage = "Color is required";
      return;
    }

    if (mediator == null)
    {
      ErrorMessage = "Mediator not available";
      return;
    }

    try
    {
      IsUpdating = true;
      ErrorMessage = string.Empty;

      var command = new UpdateFenceCommand(FenceId, ZoneName, SelectedColor, Opacity);
      await mediator.Send(command);

      CloseDialog(true);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Error updating zone: {ex.Message}";
      IsUpdating = false;
    }
  }

  /// <summary>
  ///   Command to cancel the dialog without saving.
  /// </summary>
  [RelayCommand]
  public void Cancel()
  {
    CloseDialog(false);
  }

  #endregion

  #region Dialog Result

  /// <summary>
  ///   Callback to close the dialog and return a result.
  /// </summary>
  public Action<bool> CloseDialog { get; set; } = _ => { };

  #endregion
}
