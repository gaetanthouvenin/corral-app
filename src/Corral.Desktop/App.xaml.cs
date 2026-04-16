// ------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

using Corral.Desktop.Localization;
using Corral.Desktop.Services;
using Corral.Desktop.ViewModels;
using Corral.Desktop.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MessageBox = System.Windows.MessageBox;

namespace Corral.Desktop;

/// <summary>
///   WPF Application for Corral - Virtual desktop zones manager.
/// </summary>
public partial class App : System.Windows.Application
{
  #region Fields

  private FloatingCreateButton _floatingButton;

  private ILogger<App> _logger;

  private ITrayIconService _trayIconService;

  #endregion

  #region Ctors

  /// <summary>
  ///   Initializes a new instance of the App class.
  /// </summary>
  public App()
  {
    InitializeComponent();

    // Registered here to catch exceptions that occur before OnStartup (e.g. resource loading).
    DispatcherUnhandledException += OnDispatcherUnhandledException;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets or sets the service provider for dependency injection.
  /// </summary>
  public IServiceProvider ServiceProvider { get; set; }

  #endregion

  #region Methods

  /// <summary>
  ///   Handles the startup of the application.
  /// </summary>
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    _logger = ServiceProvider.GetRequiredService<ILogger<App>>();
    AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
    TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

    // Setup tray icon
    _trayIconService = ServiceProvider.GetRequiredService<ITrayIconService>();
    _trayIconService.ShowWindowRequested += OnShowWindowRequested;
    _trayIconService.ToggleOverlaysRequested += OnToggleOverlaysRequested;
    _trayIconService.ExitRequested += OnExitRequested;
    _trayIconService.Show();

    // Create MainWindow via DI - ViewModel is injected in constructor
    if (ServiceProvider.GetRequiredService<MainWindow>() is { } mainWindow)
    {
      MainWindow = mainWindow;

      // Minimize to tray instead of closing
      MainWindow.Closing += OnMainWindowClosing;

      MainWindow.Show();
    }

    // Setup floating create button on the desktop
    _floatingButton = new FloatingCreateButton();
    _floatingButton.CreateRequested += OnFloatingCreateRequested;
    _floatingButton.Show();
  }

  /// <summary>
  ///   Handles the application exit to clean up tray icon.
  /// </summary>
  protected override void OnExit(ExitEventArgs e)
  {
    DispatcherUnhandledException -= OnDispatcherUnhandledException;
    AppDomain.CurrentDomain.UnhandledException -= OnDomainUnhandledException;
    TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

    if (_floatingButton != null)
    {
      _floatingButton.CreateRequested -= OnFloatingCreateRequested;
      _floatingButton.Close();
    }

    if (_trayIconService != null)
    {
      _trayIconService.ShowWindowRequested -= OnShowWindowRequested;
      _trayIconService.ToggleOverlaysRequested -= OnToggleOverlaysRequested;
      _trayIconService.ExitRequested -= OnExitRequested;
      _trayIconService.Dispose();
    }

    base.OnExit(e);
  }

  /// <summary>
  ///   Catches unhandled exceptions thrown on the WPF UI (Dispatcher) thread.
  ///   The application is kept alive — the exception is considered recoverable.
  /// </summary>
  private void OnDispatcherUnhandledException(
    object sender,
    DispatcherUnhandledExceptionEventArgs e)
  {
    _logger?.LogError(e.Exception, "Exception non managée sur le thread UI");

    MessageBox.Show(
      $"{Strings.Error_Unexpected}{Environment.NewLine}{e.Exception.Message}",
      Strings.Error_UnexpectedTitle,
      MessageBoxButton.OK,
      MessageBoxImage.Error
    );

    e.Handled = true;
  }

  /// <summary>
  ///   Catches unhandled exceptions thrown on background (non-Dispatcher) threads.
  ///   The CLR will terminate the process after this handler returns when
  ///   <see cref="UnhandledExceptionEventArgs.IsTerminating" /> is <c>true</c>.
  /// </summary>
  private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
  {
    var ex = e.ExceptionObject as Exception;

    if (_logger != null && ex != null)
    {
      _logger.LogCritical(
        ex,
        "Exception non managée sur un thread de fond (IsTerminating={IsTerminating})",
        e.IsTerminating
      );
    }

    if (ex != null)
    {
      MessageBox.Show(
        $"{Strings.Error_Fatal}{Environment.NewLine}{ex.Message}",
        Strings.Error_FatalTitle,
        MessageBoxButton.OK,
        MessageBoxImage.Error
      );
    }
  }

  /// <summary>
  ///   Catches exceptions from fire-and-forget <see cref="Task" />s that were never awaited.
  ///   Marks the exception as observed to prevent the finalizer thread from re-throwing it.
  /// </summary>
  private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
  {
    if (_logger != null)
    {
      _logger.LogError(e.Exception, "Exception non observée dans une tâche en arrière-plan");
    }

    e.SetObserved();
  }

  private void OnMainWindowClosing(object sender, CancelEventArgs e)
  {
    // Minimize to tray instead of exiting
    e.Cancel = true;
    MainWindow?.Hide();
  }

  #endregion

  // Tray icon callbacks are invoked from WinForms thread — Dispatcher is required to marshal to UI thread.
#pragma warning disable VSTHRD001
  /// <summary>
  ///   Handles the event triggered when a request to show the main application window is received.
  /// </summary>
  /// <param name="sender">
  ///   The source of the event, typically the tray icon service.
  /// </param>
  /// <param name="e">
  ///   An <see cref="EventArgs" /> instance containing the event data.
  /// </param>
  private void OnShowWindowRequested(object sender, EventArgs e)
  {
    _ = Dispatcher.BeginInvoke(() =>
                               {
                                 if (MainWindow != null)
                                 {
                                   MainWindow.Show();
                                   MainWindow.WindowState = WindowState.Normal;
                                   MainWindow.Activate();
                                 }
                               }
    );
  }

  /// <summary>
  ///   Handles the <see cref="ITrayIconService.ToggleOverlaysRequested" /> event.
  ///   Toggles the visibility of overlays by invoking the <c>ToggleOverlaysCommand</c>
  ///   on the <see cref="MainWindowViewModel" /> if it is set as the <c>DataContext</c>
  ///   of the <see cref="MainWindow" />.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  private void OnToggleOverlaysRequested(object sender, EventArgs e)
  {
    _ = Dispatcher.BeginInvoke(() =>
                               {
                                 if (MainWindow?.DataContext is MainWindowViewModel mainVm
                                     && mainVm.CurrentViewModel is ZonesViewModel zonesVm)
                                 {
                                   zonesVm.ToggleOverlaysCommand.Execute(null);
                                 }
                               }
    );
  }

  /// <summary>
  ///   Handles the exit request from the tray icon service by detaching the main window's closing
  ///   handler
  ///   and initiating the application shutdown process.
  /// </summary>
  /// <param name="sender">The source of the event, typically the tray icon service.</param>
  /// <param name="e">The event data associated with the exit request.</param>
  private void OnExitRequested(object sender, EventArgs e)
  {
    // Detach closing handler to allow real exit
    if (MainWindow != null)
    {
      MainWindow.Closing -= OnMainWindowClosing;
    }

    _ = Dispatcher.BeginInvoke(() => Shutdown());
  }

  /// <summary>
  ///   Handles the event triggered when the floating create button requests the creation of a new zone.
  /// </summary>
  /// <remarks>
  ///   This method invokes the <see cref="ZonesViewModel.CreateNewZoneCommand" /> command
  ///   to initiate the creation of a new virtual desktop zone.
  /// </remarks>
  private void OnFloatingCreateRequested()
  {
    _ = Dispatcher.BeginInvoke(() =>
                               {
                                 if (MainWindow?.DataContext is MainWindowViewModel mainVm
                                     && mainVm.CurrentViewModel is ZonesViewModel zonesVm)
                                 {
                                   zonesVm.CreateNewZoneCommand.Execute(null);
                                 }
                               }
    );
  }
#pragma warning restore VSTHRD001
}
