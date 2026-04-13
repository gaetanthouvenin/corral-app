// ------------------------------------------------------------------------------------------------
// <copyright file="CreateZoneDialogViewModel.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corral.Application.Commands.CreateFence;

using MediatR;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   View model for the create zone dialog window.
/// </summary>
/// <remarks>
///   Sends a <see cref="CreateFenceCommand" /> via MediatR on confirmation.
///   Shared state (color presets, zone name, opacity, color preview) lives in
///   <see cref="ZoneDialogViewModelBase" />.
/// </remarks>
public partial class CreateZoneDialogViewModel(IMediator mediator) : ZoneDialogViewModelBase
{
  #region Fields

  /// <summary>
  ///   Indicates whether a zone creation operation is currently in progress.
  /// </summary>
  [ObservableProperty]
  private bool _isCreating;

  #endregion

  #region Commands

  /// <summary>
  ///   Creates the zone and closes the dialog on success.
  /// </summary>
  [RelayCommand(CanExecute = nameof(CanCreateZone))]
#pragma warning disable VSTHRD002
  private async Task CreateZone()
  {
    try
    {
      IsCreating = true;
      ErrorMessage = string.Empty;

      var command = new CreateFenceCommand(
        ZoneName.Trim(),
        100,
        100,
        800,
        600,
        SelectedColor,
        Opacity
      );

      await mediator.Send(command);
      CloseDialog(true);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Erreur lors de la création : {ex.Message}";
    }
    finally
    {
      IsCreating = false;
    }
  }
#pragma warning restore VSTHRD002

  /// <summary>
  ///   Determines whether the zone can be created based on the current state.
  /// </summary>
  /// <returns>
  ///   <c>true</c> if the zone name is not null, not empty, and not whitespace,
  ///   and the creation process is not currently in progress; otherwise, <c>false</c>.
  /// </returns>
  /// <remarks>
  ///   This method is used as the <c>CanExecute</c> condition for the <see cref="CreateZoneCommand" />.
  /// </remarks>
  private bool CanCreateZone()
  {
    return !string.IsNullOrWhiteSpace(ZoneName) && !IsCreating;
  }

  #endregion

  #region Helpers

  /// <summary>
  ///   Notifies the associated command to reevaluate its ability to execute.
  /// </summary>
  /// <remarks>
  ///   This method overrides <see cref="ZoneDialogViewModelBase.RefreshCanExecute" />
  ///   to ensure that the <see cref="CreateZoneCommand" /> updates its execution state
  ///   based on the current conditions.
  /// </remarks>
  protected override void RefreshCanExecute()
  {
    CreateZoneCommand.NotifyCanExecuteChanged();
  }

  /// <summary>
  ///   Handles changes to the <see cref="_isCreating" /> property.
  /// </summary>
  /// <param name="value">The new value of the <see cref="_isCreating" /> property.</param>
  /// <remarks>
  ///   This method is invoked whenever the <see cref="_isCreating" /> property changes.
  ///   It ensures that the state of the <see cref="CreateZoneCommand" /> is updated accordingly.
  /// </remarks>
  partial void OnIsCreatingChanged(bool value)
  {
    CreateZoneCommand.NotifyCanExecuteChanged();
  }

  #endregion
}
