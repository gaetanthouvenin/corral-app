// ------------------------------------------------------------------------------------------------
// <copyright file="EditZoneDialog.xaml.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Windows.Input;

using Corral.Desktop.ViewModels;

namespace Corral.Desktop.Views;

/// <summary>
///   Dialog for editing an existing zone.
/// </summary>
public partial class EditZoneDialog
{
  #region Ctors

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
