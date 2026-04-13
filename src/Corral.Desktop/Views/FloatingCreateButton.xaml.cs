// ------------------------------------------------------------------------------------------------
// <copyright file="FloatingCreateButton.xaml.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Corral.Desktop.Views;

/// <summary>
///   A floating always-on-top circular button for quick zone creation.
///   Positioned at the bottom-right corner of the primary screen.
/// </summary>
public partial class FloatingCreateButton
{
  #region Ctors

  /// <summary>
  ///   Initializes a new instance of the FloatingCreateButton.
  /// </summary>
  public FloatingCreateButton()
  {
    InitializeComponent();
    Loaded += OnLoaded;
  }

  #endregion

  #region Methods

  #region Events

  /// <summary>
  ///   Raised when the user clicks the create button.
  /// </summary>
  public event Action CreateRequested;

  #endregion

  private void OnLoaded(object sender, RoutedEventArgs e)
  {
    // Hide from Alt+Tab
    var hwnd = new WindowInteropHelper(this).Handle;
    var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
    SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);

    // Position at bottom-right of the primary screen
    var workArea = SystemParameters.WorkArea;
    Left = workArea.Right - Width - 24;
    Top = workArea.Bottom - Height - 24;
  }

  private void OnCreateButtonClick(object sender, RoutedEventArgs e)
  {
    CreateRequested?.Invoke();
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
