// ------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Windows;

using Corral.Desktop.ViewModels;

namespace Corral.Desktop;

/// <summary>
///   Represents the main window of the Corral application.
/// </summary>
/// <remarks>
///   This class is the entry point for the application's user interface. It is initialized with
///   dependency injection,
///   which provides the required <see cref="MainWindowViewModel" /> as its data context.
/// </remarks>
public sealed partial class MainWindow
{
  #region Ctors

  /// <summary>
  ///   Initializes a new instance of the MainWindow class with dependency injection.
  /// </summary>
  /// <param name="viewModel">The MainWindowViewModel injected via DI.</param>
  /// <param name="settingsViewModel">The SettingsViewModel injected via DI.</param>
  public MainWindow(MainWindowViewModel viewModel, SettingsViewModel settingsViewModel)
  {
    InitializeComponent();
    DataContext = viewModel;

    // Initialize settings view model (fire-and-forget — runs on UI thread)
    _ = settingsViewModel.Initialize();

    // Store settings VM in resources for binding
    Resources["SettingsVM"] = settingsViewModel;
  }

  #endregion

  #region Methods

  /// <summary>
  ///   Handles the click event for the minimize button in the main window.
  /// </summary>
  /// <param name="sender">The source of the event, typically the minimize button.</param>
  /// <param name="e">The event data associated with the click event.</param>
  /// <remarks>
  ///   This method minimizes the main window by setting its <see cref="Window.WindowState" /> to
  ///   <see cref="WindowState.Minimized" />.
  /// </remarks>
  private void OnMinimizeClicked(object sender, RoutedEventArgs e)
  {
    WindowState = WindowState.Minimized;
  }

  /// <summary>
  ///   Handles the click event for the maximize/restore button in the window controls.
  /// </summary>
  /// <param name="sender">The source of the event, typically the button that was clicked.</param>
  /// <param name="e">The event data associated with the click event.</param>
  /// <remarks>
  ///   Toggles the window state between <see cref="WindowState.Maximized" /> and
  ///   <see cref="WindowState.Normal" />.
  ///   This method is linked to the maximize/restore button in the window's title bar.
  /// </remarks>
  private void OnMaximizeRestoreClicked(object sender, RoutedEventArgs e)
  {
    WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
  }

  /// <summary>
  ///   Handles the click event for the Close button in the main window.
  /// </summary>
  /// <param name="sender">The source of the event, typically the Close button.</param>
  /// <param name="e">The event data associated with the click event.</param>
  /// <remarks>
  ///   This method closes the main window when the Close button is clicked.
  /// </remarks>
  private void OnCloseClicked(object sender, RoutedEventArgs e)
  {
    Close();
  }

  #endregion
}
