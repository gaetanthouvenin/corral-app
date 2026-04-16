// ------------------------------------------------------------------------------------------------
// <copyright file="FenceOverlayWindow.xaml.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Corral.Desktop.Controls;
using Corral.Desktop.Models;
using Corral.Desktop.ViewModels;

using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using ClickMode = Corral.Desktop.Models.ClickMode;
using DragCompletedEventArgs = System.Windows.Controls.Primitives.DragCompletedEventArgs;
using DragDeltaEventArgs = System.Windows.Controls.Primitives.DragDeltaEventArgs;
using DragStartedEventArgs = System.Windows.Controls.Primitives.DragStartedEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Orientation = System.Windows.Controls.Orientation;
using Point = System.Windows.Point;

namespace Corral.Desktop.Views;

/// <summary>
///   A borderless, topmost overlay window representing a fence zone on the desktop.
///   The title bar is draggable; the body area passes clicks through to the desktop.
/// </summary>
public partial class FenceOverlayWindow
{
  #region Fields

  private const int DoubleClickTimeMs = 300;
  private int _clickCount;
  private ClickMode _currentClickMode = ClickMode.SingleClick;
  private IconLayout _currentIconLayout = IconLayout.LargeGrid;

  private Point _dragStartPoint;

  private bool _isDragging;
  private bool _isResizing;
  private FrameworkElement _lastClickedElement;
  private DateTime _lastClickTime = DateTime.MinValue;
  private double _resizeStartHeight;
  private double _resizeStartLeft;
  private double _resizeStartTop;
  private double _resizeStartWidth;
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

  /// <summary>
  ///   Raised when the user finishes resizing the overlay to new dimensions.
  /// </summary>
  public event Action<int, int> DimensionsChanged;

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
  /// <param name="clickMode">Click mode for opening items (single or double click).</param>
  /// <param name="iconLayout">Icon layout style for displaying items.</param>
  public void UpdateFenceDisplay(
    string name,
    int x,
    int y,
    int width,
    int height,
    string hexColor,
    double opacity,
    IEnumerable<FenceItemViewModel> items = null,
    ClickMode clickMode = ClickMode.SingleClick,
    IconLayout iconLayout = IconLayout.LargeGrid)
  {
    Left = x;
    Top = y;

    Width = width;
    Height = height;

    ZoneNameLabel.Text = name.ToUpperInvariant();
    _currentClickMode = clickMode;

    ApplyLayout(iconLayout);

    try
    {
      var convertedColor = (Color)ColorConverter.ConvertFromString(hexColor);
      var borderBrush = new SolidColorBrush(convertedColor);

      OverlayBorder.BorderBrush = borderBrush;
      ZoneNameLabel.Foreground = borderBrush;

      // Opacity is between 0.0 and 1.0
      var boundedOpacity = Math.Max(0.0, Math.Min(1.0, opacity));
      var alpha = (byte)(255 * boundedOpacity);

      // Background: blend BackgroundColor with #202020 dark color
      var bgColor = Color.FromArgb(alpha, 32, 32, 32);
      OverlayBorder.Background = new SolidColorBrush(bgColor);
      OverlayBorder.Opacity = 1.0;
    }
    catch
    {
      var fallbackBrush = new SolidColorBrush(Colors.CornflowerBlue);
      OverlayBorder.BorderBrush = fallbackBrush;
      ZoneNameLabel.Foreground = fallbackBrush;

      // Opacity is between 0.0 and 1.0
      var boundedOpacity = Math.Max(0.0, Math.Min(1.0, opacity));
      var alpha = (byte)(255 * boundedOpacity);

      // Background: dark color with opacity
      var bgColor = Color.FromArgb(alpha, 32, 32, 32);
      OverlayBorder.Background = new SolidColorBrush(bgColor);
      OverlayBorder.Opacity = 1.0;
    }

    // Populate items
    if (items != null)
    {
      ItemsPanel.ItemsSource = items;
    }
  }

  /// <summary>
  ///   Applies the specified layout style to the items display.
  /// </summary>
  private void ApplyLayout(IconLayout layout)
  {
    _currentIconLayout = layout;

    // Update the template selector
    if (ItemsPanel.ItemTemplateSelector is IconLayoutDataTemplateSelector selector)
    {
      selector.CurrentLayout = layout;
    }

    // Switch container panel: StackPanel vertical for List, WrapPanel for grids
    if (layout == IconLayout.List)
    {
      var factory = new FrameworkElementFactory(typeof(StackPanel));
      factory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
      ItemsPanel.ItemsPanel = new ItemsPanelTemplate(factory);
    }
    else
    {
      var factory = new FrameworkElementFactory(typeof(WrapPanel));
      factory.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
      ItemsPanel.ItemsPanel = new ItemsPanelTemplate(factory);
    }

    // Recycler ItemsSource pour forcer WPF à re-générer tous les item containers
    var source = ItemsPanel.ItemsSource;
    ItemsPanel.ItemsSource = null;
    ItemsPanel.ItemsSource = source;
  }

  /// <summary>
  ///   Refreshes click mode and icon layout when preferences change (live update).
  /// </summary>
  public void RefreshPreferences(ClickMode clickMode, IconLayout iconLayout)
  {
    _currentClickMode = clickMode;
    ApplyLayout(iconLayout);
  }

  private void OnLoaded(object sender, RoutedEventArgs e)
  {
    // Hide from Alt+Tab but keep the window interactive for the drag handle
    var hwnd = new WindowInteropHelper(this).Handle;
    var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
    SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);
  }

  private void OnResizeThumbDragStarted(object sender, DragStartedEventArgs e)
  {
    _isResizing = true;
    _resizeStartLeft = Left;
    _resizeStartTop = Top;
    _resizeStartWidth = Width;
    _resizeStartHeight = Height;
  }

  private void OnResizeThumbDragDelta(object sender, DragDeltaEventArgs e)
  {
    if (sender is not Thumb thumb || thumb.Tag is not string direction)
    {
      return;
    }

    var newLeft = Left;
    var newTop = Top;
    var newWidth = Width;
    var newHeight = Height;

    if (direction.Contains("Right", StringComparison.Ordinal))
    {
      newWidth = Math.Max(MinWidth, Width + e.HorizontalChange);
    }

    if (direction.Contains("Left", StringComparison.Ordinal))
    {
      var candidateWidth = Width - e.HorizontalChange;
      newWidth = Math.Max(MinWidth, candidateWidth);
      newLeft = Left + (Width - newWidth);
    }

    if (direction.Contains("Bottom", StringComparison.Ordinal))
    {
      newHeight = Math.Max(MinHeight, Height + e.VerticalChange);
    }

    if (direction.Contains("Top", StringComparison.Ordinal))
    {
      var candidateHeight = Height - e.VerticalChange;
      newHeight = Math.Max(MinHeight, candidateHeight);
      newTop = Top + (Height - newHeight);
    }

    Left = newLeft;
    Top = newTop;
    Width = newWidth;
    Height = newHeight;
  }

  private void OnResizeThumbDragCompleted(object sender, DragCompletedEventArgs e)
  {
    if (!_isResizing)
    {
      return;
    }

    _isResizing = false;

    if ((int)_resizeStartLeft != (int)Left || (int)_resizeStartTop != (int)Top)
    {
      PositionChanged?.Invoke((int)Left, (int)Top);
    }

    if ((int)_resizeStartWidth != (int)Width || (int)_resizeStartHeight != (int)Height)
    {
      DimensionsChanged?.Invoke((int)Width, (int)Height);
    }
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

  private void OnItemMouseDown(object sender, MouseButtonEventArgs e)
  {
    if (e.LeftButton != MouseButtonState.Pressed)
    {
      return;
    }

    var now = DateTime.Now;
    var timeSinceLastClick = (now - _lastClickTime).TotalMilliseconds;

    // Check if it's the same element and within double-click time
    if (sender == _lastClickedElement && timeSinceLastClick < DoubleClickTimeMs)
    {
      _clickCount++;
    }
    else
    {
      _clickCount = 1;
      _lastClickedElement = sender as FrameworkElement;
    }

    _lastClickTime = now;

    // Single click
    if (_clickCount == 1 && _currentClickMode == ClickMode.SingleClick)
    {
      OpenItem(sender, e);
    }

    // Double click
    if (_clickCount >= 2 && _currentClickMode == ClickMode.DoubleClick)
    {
      OpenItem(sender, e);
      _clickCount = 0; // Reset after opening
    }

    e.Handled = true;
  }

  private void OpenItem(object sender, MouseButtonEventArgs e)
  {
    if (sender is not FrameworkElement element)
    {
      return;
    }

    if (element.DataContext is not FenceItemViewModel item)
    {
      return;
    }

    try
    {
      var processInfo = new ProcessStartInfo
      {
        FileName = item.Path,
        UseShellExecute = true,
        CreateNoWindow = true
      };

      Process.Start(processInfo);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error opening item '{item.DisplayName}': {ex.Message}");
    }

    e.Handled = true;
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
