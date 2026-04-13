// ------------------------------------------------------------------------------------------------
// <copyright file="TrayIconService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Drawing.Drawing2D;

using Microsoft.Extensions.Logging;

namespace Corral.Desktop.Services;

/// <summary>
///   Manages the system tray (notification area) icon using Windows Forms NotifyIcon.
///   Provides a context menu with Show, Toggle Overlays, and Exit options.
/// </summary>
public class TrayIconService(ILogger<TrayIconService> logger) : ITrayIconService
{
  #region Fields

  private bool _disposed;

  private NotifyIcon _notifyIcon;

  #endregion

  #region Methods

  /// <summary>
  ///   Loads the application icon.
  /// </summary>
  private static Icon CreateIcon()
  {
    var uri = new Uri("pack://application:,,,/Corral.Desktop;component/Assets/app_icon.ico");
    var streamInfo = System.Windows.Application.GetResourceStream(uri);
    return new Icon(streamInfo.Stream);
  }

  /// <summary>
  ///   Creates the right-click context menu for the tray icon.
  /// </summary>
  private ContextMenuStrip CreateContextMenu()
  {
    var menu = new ContextMenuStrip();

    var showItem = new ToolStripMenuItem("Show Corral");
    showItem.Font = new Font(showItem.Font, FontStyle.Bold);
    showItem.Click += (_, _) => ShowWindowRequested?.Invoke(this, EventArgs.Empty);

    var overlayItem = new ToolStripMenuItem("Toggle Overlays");
    overlayItem.Click += (_, _) => ToggleOverlaysRequested?.Invoke(this, EventArgs.Empty);

    var exitItem = new ToolStripMenuItem("Exit");
    exitItem.Click += (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty);

    menu.Items.Add(showItem);
    menu.Items.Add(overlayItem);
    menu.Items.Add(new ToolStripSeparator());
    menu.Items.Add(exitItem);

    return menu;
  }

  private void OnNotifyIconDoubleClick(object sender, EventArgs e)
  {
    ShowWindowRequested?.Invoke(this, EventArgs.Empty);
  }

  #endregion

  #region Implementation of IDisposable

  /// <inheritdoc />
  public void Dispose()
  {
    if (_disposed)
    {
      return;
    }

    Hide();
    _disposed = true;
  }

  #endregion

  #region Implementation of ITrayIconService

  /// <inheritdoc />
  public void Show()
  {
    if (_notifyIcon != null)
    {
      return;
    }

    _notifyIcon = new NotifyIcon
    {
      Icon = CreateIcon(),
      Text = "Corral — Desktop Zone Manager",
      Visible = true,
      ContextMenuStrip = CreateContextMenu()
    };

    _notifyIcon.DoubleClick += OnNotifyIconDoubleClick;

    logger.LogDebug("Tray icon shown");
  }

  /// <inheritdoc />
  public void Hide()
  {
    if (_notifyIcon == null)
    {
      return;
    }

    _notifyIcon.Visible = false;
    _notifyIcon.DoubleClick -= OnNotifyIconDoubleClick;
    _notifyIcon.Dispose();
    _notifyIcon = null;

    logger.LogDebug("Tray icon hidden");
  }

  #endregion

  #region Events

  /// <inheritdoc />
  public event EventHandler ShowWindowRequested;

  /// <inheritdoc />
  public event EventHandler ToggleOverlaysRequested;

  /// <inheritdoc />
  public event EventHandler ExitRequested;

  #endregion
}
