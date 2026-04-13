// ------------------------------------------------------------------------------------------------
// <copyright file="EditZoneDialogViewModel.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corral.Application.Commands.UpdateFence;

using MediatR;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   View model for the edit zone dialog window.
/// </summary>
/// <remarks>
///   Sends an <see cref="UpdateFenceCommand" /> via MediatR on confirmation.
///   Shared state (color presets, zone name, opacity, color preview) lives in
///   <see cref="ZoneDialogViewModelBase" />.
/// </remarks>
public partial class EditZoneDialogViewModel(IMediator mediator) : ZoneDialogViewModelBase
{
  #region Fields

  /// <summary>
  ///   ID of the fence being edited.
  /// </summary>
  [ObservableProperty]
  private string _fenceId = string.Empty;

  /// <summary>
  ///   Indicates whether an update operation is currently in progress.
  /// </summary>
  [ObservableProperty]
  private bool _isUpdating;

  #endregion

  #region Ctors

  #region Design-time constructor

  /// <summary>
  ///   Parameterless constructor for the XAML designer.
  /// </summary>
  public EditZoneDialogViewModel()
    : this(null)
  {
    FenceId = "00000000-0000-0000-0000-000000000000";
    ZoneName = "Zone de test";
  }

  #endregion

  #endregion

  #region Methods

  /// <summary>
  ///   Initializes the view model with the current fence data before opening the dialog.
  /// </summary>
  public void Initialize(string fenceId, string name, string color, int opacity)
  {
    FenceId = fenceId;
    ZoneName = name;
    SelectedColor = color;
    Opacity = opacity;
    ErrorMessage = string.Empty;
  }

  #endregion

  #region Commands

  /// <summary>
  ///   Updates the zone with the new values and closes the dialog on success.
  /// </summary>
  [RelayCommand(CanExecute = nameof(CanUpdateZone))]
#pragma warning disable VSTHRD002
  private async Task UpdateZone()
  {
    try
    {
      IsUpdating = true;
      ErrorMessage = string.Empty;

      var command = new UpdateFenceCommand(FenceId, ZoneName.Trim(), SelectedColor, Opacity);
      await mediator.Send(command);

      CloseDialog(true);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Erreur lors de la modification : {ex.Message}";
    }
    finally
    {
      IsUpdating = false;
    }
  }
#pragma warning restore VSTHRD002

  private bool CanUpdateZone()
  {
    return !string.IsNullOrWhiteSpace(ZoneName) && !IsUpdating;
  }

  #endregion

  #region Helpers

  protected override void RefreshCanExecute()
  {
    UpdateZoneCommand.NotifyCanExecuteChanged();
  }

  partial void OnIsUpdatingChanged(bool value)
  {
    UpdateZoneCommand.NotifyCanExecuteChanged();
  }

  #endregion
}
