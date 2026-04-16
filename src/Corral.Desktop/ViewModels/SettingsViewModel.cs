// ------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corral.Desktop.Models;
using Corral.Desktop.Services;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   ViewModel for overlay preferences settings.
/// </summary>
public partial class SettingsViewModel(
  IUserPreferencesService preferencesService,
  IOverlayService overlayService) : ObservableObject
{
  #region Fields

  /// <summary>
  ///   Selected click mode for opening items.
  /// </summary>
  [ObservableProperty]
  private ClickMode _selectedClickMode = ClickMode.SingleClick;

  /// <summary>
  ///   Selected icon layout style.
  /// </summary>
  [ObservableProperty]
  private IconLayout _selectedIconLayout = IconLayout.LargeGrid;

  #endregion

  #region Methods

  #region Initialization

  /// <summary>
  ///   Initializes the view model by loading preferences from the database.
  /// </summary>
  public async Task Initialize()
  {
    var prefs = await preferencesService.GetPreferencesAsync();
    SelectedClickMode = prefs.ClickMode;
    SelectedIconLayout = prefs.IconLayout;
  }

  #endregion

  #endregion

  #region Commands

  /// <summary>
  ///   Saves the current settings to the database and refreshes all overlays.
  /// </summary>
  [RelayCommand]
  public async Task SaveSettings()
  {
    var prefs = new OverlayPreferences
    {
      ClickMode = SelectedClickMode, IconLayout = SelectedIconLayout
    };

    await preferencesService.SavePreferencesAsync(prefs);

    // Refresh all overlays with the new preferences
    await overlayService.RefreshAllOverlayLayouts();
  }

  /// <summary>
  ///   Resets settings to defaults and refreshes all overlays.
  /// </summary>
  [RelayCommand]
  public async Task ResetDefaults()
  {
    SelectedClickMode = ClickMode.SingleClick;
    SelectedIconLayout = IconLayout.LargeGrid;
    await SaveSettings();
  }

  #endregion
}
