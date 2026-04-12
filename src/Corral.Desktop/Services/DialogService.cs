// ------------------------------------------------------------------------------------------------
// <copyright file="DialogService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Desktop.Views;
using Corral.Desktop.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace Corral.Desktop.Services;

/// <summary>
///   Implementation of the WPF dialog service.
///   Resolves dialog windows via the DI container.
/// </summary>
public class DialogService(IServiceProvider serviceProvider) : IDialogService
{
  /// <inheritdoc />
  public bool ShowCreateZoneDialog()
  {
    var dialog = serviceProvider.GetRequiredService<CreateZoneDialog>();
    return dialog.ShowDialog() == true;
  }

  /// <inheritdoc />
  public bool ShowEditZoneDialog(string fenceId, string name, string color, int opacity)
  {
    var viewModel = serviceProvider.GetRequiredService<EditZoneDialogViewModel>();
    viewModel.Initialize(fenceId, name, color, opacity);

    var dialog = new EditZoneDialog(viewModel);
    return dialog.ShowDialog() == true;
  }
}
