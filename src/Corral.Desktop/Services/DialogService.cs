// ------------------------------------------------------------------------------------------------
// <copyright file="DialogService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Desktop.ViewModels;
using Corral.Desktop.Views;

using Microsoft.Extensions.DependencyInjection;

namespace Corral.Desktop.Services;

/// <summary>
///   Implementation of the WPF dialog service.
///   Resolves dialog windows via the DI container.
/// </summary>
public class DialogService(IServiceProvider serviceProvider) : IDialogService
{
  #region Implementation of IDialogService

  /// <inheritdoc />
  public bool ShowCreateZoneDialog()
  {
    if (serviceProvider.GetRequiredService<CreateZoneDialog>() is { } dialog)
    {
      return dialog.ShowDialog() == true;
    }

    return false;
  }

  /// <inheritdoc />
  public bool ShowEditZoneDialog(string fenceId, string name, string color, int opacity)
  {
    if (serviceProvider.GetRequiredService<EditZoneDialogViewModel>() is { } viewModel)
    {
      viewModel.Initialize(fenceId, name, color, opacity);

      var dialog = new EditZoneDialog(viewModel);
      return dialog.ShowDialog() == true;
    }

    return false;
  }

  #endregion
}
