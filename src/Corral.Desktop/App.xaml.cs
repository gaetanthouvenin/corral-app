// ------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using System.Windows;
using Corral.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Corral.Desktop;

/// <summary>
///   WPF Application for Corral - Virtual desktop zones manager.
/// </summary>
public partial class App : System.Windows.Application
{
  #region Properties

  /// <summary>
  ///   Gets or sets the service provider for dependency injection.
  /// </summary>
  public IServiceProvider ServiceProvider { get; set; }

  #endregion

  #region Ctors

  /// <summary>
  ///   Initializes a new instance of the App class.
  /// </summary>
  public App()
  {
    // XAML code-gen is not used since Program.cs is the startup object
  }

  #endregion

  #region Methods

  /// <summary>
  ///   Handles the startup of the application.
  /// </summary>
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    // Create MainWindow via DI - ViewModel is injected in constructor
    var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
    MainWindow = mainWindow;
    MainWindow.Show();
  }

  #endregion
}
