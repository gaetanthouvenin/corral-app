// ------------------------------------------------------------------------------------------------
// <copyright file="FenceOverlayWindow.xaml.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Corral.Desktop.ViewModels;

using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;

namespace Corral.Desktop.Views;

/// <summary>
///   A borderless, topmost overlay window representing a fence zone on the desktop.
///   The title bar is draggable; the body area passes clicks through to the desktop.
/// </summary>
public partial class FenceOverlayWindow
{
  #region Fields

  private Point _dragStartPoint;

  private bool _isDragging;
  private double _windowStartLeft;
  private double _windowStartTop;

  #endregion

  #region Ctors

  /// <summary>
  ///   Initializes a new instance of the FenceOverlayWindow.
  /// </summary>
  public FenceOverlayWindow()
  {
    InitializeComponent();
    Loaded += OnLoaded;
  }

  #endregion

  #region Methods

  #region Events

  /// <summary>
  ///   Raised when the user finishes dragging the overlay to a new position.
  /// </summary>
  public event Action<int, int> PositionChanged;

  #endregion

  /// <summary>
  ///   Updates the overlay with the specified fence properties.
  /// </summary>
  /// <param name="name">Zone display name.</param>
  /// <param name="x">X position in pixels.</param>
  /// <param name="y">Y position in pixels.</param>
  /// <param name="width">Width in pixels.</param>
  /// <param name="height">Height in pixels.</param>
  /// <param name="hexColor">Background color in #AARRGGBB format.</param>
  /// <param name="opacity">Opacity value between 0.0 and 1.0.</param>
  /// <param name="items">Optional list of item view models to show (with icon + display name).</param>
  public void UpdateFenceDisplay(
    string name,
    int x,
    int y,
    int width,
    int height,
    string hexColor,
    double opacity,
    IEnumerable<FenceItemViewModel> items = null)
  {
    Left = x;
    Top = y;
    Width = width;
    Height = height;

    ZoneNameLabel.Text = name.ToUpperInvariant();

    try
    {
      var convertedColor = (Color)ColorConverter.ConvertFromString(hexColor);
      var solidBrush = new SolidColorBrush(convertedColor);

      OverlayBorder.BorderBrush = solidBrush;
      ZoneDot.Fill = solidBrush;
      ZoneStatusLabel.Foreground = solidBrush;

      var boundedOpacity = Math.Max(0, Math.Min(100, opacity));
      var bgColor = Color.FromArgb((byte)(255 * (boundedOpacity / 100.0)), 20, 20, 22);
      OverlayBorder.Background = new SolidColorBrush(bgColor);
      OverlayBorder.Opacity = 1.0;
    }
    catch
    {
      var fallbackBrush = new SolidColorBrush(Colors.CornflowerBlue);
      OverlayBorder.BorderBrush = fallbackBrush;
      ZoneDot.Fill = fallbackBrush;
      ZoneStatusLabel.Foreground = fallbackBrush;

      var bgColor = Color.FromArgb((byte)(255 * (opacity / 100.0)), 20, 20, 22);
      OverlayBorder.Background = new SolidColorBrush(bgColor);
      OverlayBorder.Opacity = 1.0;
    }

    // Populate items
    if (items != null)
    {
      ItemsPanel.ItemsSource = items;
    }
  }

  private void OnLoaded(object sender, RoutedEventArgs e)
  {
    // Hide from Alt+Tab but keep the window interactive for the drag handle
    var hwnd = new WindowInteropHelper(this).Handle;
    var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
    SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);
  }

  private void OnDragHandleMouseDown(object sender, MouseButtonEventArgs e)
  {
    if (e.LeftButton == MouseButtonState.Pressed)
    {
      _isDragging = true;
      _dragStartPoint = PointToScreen(e.GetPosition(this));
      _windowStartLeft = Left;
      _windowStartTop = Top;
      DragHandle.CaptureMouse();
    }
  }

  private void OnDragHandleMouseUp(object sender, MouseButtonEventArgs e)
  {
    if (_isDragging)
    {
      _isDragging = false;
      DragHandle.ReleaseMouseCapture();

      PositionChanged?.Invoke((int)Left, (int)Top);
    }
  }

  private void OnDragHandleMouseMove(object sender, MouseEventArgs e)
  {
    if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
    {
      var currentPoint = PointToScreen(e.GetPosition(this));
      Left = _windowStartLeft + (currentPoint.X - _dragStartPoint.X);
      Top = _windowStartTop + (currentPoint.Y - _dragStartPoint.Y);
    }
  }

  #endregion

  #region Win32 Interop

  private const int GWL_EXSTYLE = -20;
  private const int WS_EX_TOOLWINDOW = 0x00000080;

  [DllImport("user32.dll")]
  private static extern int GetWindowLong(IntPtr hwnd, int index);

  [DllImport("user32.dll")]
  private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

  #endregion
}
