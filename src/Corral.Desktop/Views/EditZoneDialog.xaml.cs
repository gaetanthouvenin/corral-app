// ------------------------------------------------------------------------------------------------
// <copyright file="EditZoneDialog.xaml.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Desktop.ViewModels;

namespace Corral.Desktop.Views;

/// <summary>
///   Dialog for editing an existing zone.
/// </summary>
public partial class EditZoneDialog
{
  /// <summary>
  ///   Initializes a new instance of the EditZoneDialog class.
  /// </summary>
  /// <param name="viewModel">The view model to bind to this dialog.</param>
  public EditZoneDialog(EditZoneDialogViewModel viewModel)
  {
    InitializeComponent();
    DataContext = viewModel;

    // Wire up the close dialog callback
    viewModel.CloseDialog = result =>
    {
      DialogResult = result;
      Close();
    };
  }

  /// <summary>
  ///   Handles title bar mouse down to enable window dragging.
  /// </summary>
  private void OnTitleBarMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
  {
    if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
    {
      DragMove();
    }
  }
}
