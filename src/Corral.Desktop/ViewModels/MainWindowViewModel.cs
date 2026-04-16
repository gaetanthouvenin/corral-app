// ------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   Main window view model for the Corral Manager application.
/// </summary>
/// <remarks>
///   <para>
///     This ViewModel manages the main application window and handles navigation
///     between different content views (Zones and Settings).
///   </para>
///   <para>
///     The MainWindowViewModel acts as a navigation controller, coordinating between
///     child ViewModels (ZonesViewModel and SettingsViewModel).
///   </para>
/// </remarks>
public partial class MainWindowViewModel(
  ZonesViewModel zonesViewModel,
  SettingsViewModel settingsViewModel) : ObservableObject
{
  #region Fields

  /// <summary>
  ///   The currently displayed view model.
  /// </summary>
  [ObservableProperty]
  private ObservableObject _currentViewModel;

  #endregion

  #region Commands

  /// <summary>
  ///   Relay command to initialize the main window.
  /// </summary>
  [RelayCommand]
  public async Task Initialize()
  {
    CurrentViewModel = zonesViewModel;
    await zonesViewModel.InitializeCommand.ExecuteAsync(null);
    await settingsViewModel.Initialize();
  }

  /// <summary>
  ///   Relay command to navigate to the Zones view.
  /// </summary>
  [RelayCommand]
  public void NavigateToZones()
  {
    CurrentViewModel = zonesViewModel;
  }

  /// <summary>
  ///   Relay command to navigate to the Settings view.
  /// </summary>
  [RelayCommand]
  public void NavigateToSettings()
  {
    CurrentViewModel = settingsViewModel;
  }

  #endregion
}
