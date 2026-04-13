// ------------------------------------------------------------------------------------------------
// <copyright file="CreateZoneDialog.xaml.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Windows.Input;

using Corral.Desktop.ViewModels;

namespace Corral.Desktop.Views;

/// <summary>
///   Code-behind for the create zone dialog.
///   Handles ViewModel integration and window dragging.
/// </summary>
public partial class CreateZoneDialog
{
  #region Ctors

  public CreateZoneDialog(CreateZoneDialogViewModel viewModel)
  {
    InitializeComponent();
    DataContext = viewModel;

    // Wire the ViewModel callback to WPF's DialogResult
    viewModel.CloseDialog = result =>
                            {
                              DialogResult = result;
                              Close();
                            };

    // Focus the name input when the dialog opens
    Loaded += (_, _) => ZoneNameInput.Focus();

    MouseLeftButtonDown += OnTitleBarMouseDown;
  }

  #endregion

  #region Methods

  /// <summary>
  ///   Handles title bar mouse down to enable window dragging.
  /// </summary>
  private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
  {
    if (e.LeftButton == MouseButtonState.Pressed)
    {
      DragMove();
    }
  }

  #endregion
}
