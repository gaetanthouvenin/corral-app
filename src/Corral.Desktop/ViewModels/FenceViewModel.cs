// ------------------------------------------------------------------------------------------------
// <copyright file="FenceViewModel.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corral.Desktop.Services;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   ViewModel pour représenter une clôture dans la couche Présentation.
///   Contient uniquement les propriétés nécessaires à la UI, jamais d'objets du Domain.
/// </summary>
public partial class FenceViewModel : ObservableObject
{
  #region Constructor

  /// <summary>
  ///   Initializes a new instance of FenceViewModel with dialog service and mediator.
  /// </summary>
  public FenceViewModel(IDialogService dialogService, MediatR.IMediator mediator)
  {
    _dialogService = dialogService;
    _mediator = mediator;
  }

  #endregion

  #region Services

  private readonly IDialogService _dialogService;
  private readonly MediatR.IMediator _mediator;

  #endregion
  #region Fields

  /// <summary>
  ///   Couleur de fond au format hexadécimal.
  /// </summary>
  [ObservableProperty]
  private string _color = "FFFFFFFF";

  /// <summary>
  ///   Date et heure de création.
  /// </summary>
  [ObservableProperty]
  private DateTime _createdAt;

  /// <summary>
  ///   Hauteur de la clôture en pixels.
  /// </summary>
  [ObservableProperty]
  private int _height;

  /// <summary>
  ///   Identifiant unique de la clôture.
  /// </summary>
  [ObservableProperty]
  private string _id = string.Empty;

  /// <summary>
  ///   Indique si la clôture est active.
  /// </summary>
  [ObservableProperty]
  private bool _isActive;

  /// <summary>
  ///   Nom de la clôture.
  /// </summary>
  [ObservableProperty]
  private string _name = string.Empty;

  /// <summary>
  ///   Opacité de la clôture (0.0 à 1.0 pour WinUI).
  /// </summary>
  [ObservableProperty]
  private double _opacity = 1.0;

  /// <summary>
  ///   Date et heure de dernière modification.
  /// </summary>
  [ObservableProperty]
  private DateTime _updatedAt;

  /// <summary>
  ///   Largeur de la clôture en pixels.
  /// </summary>
  [ObservableProperty]
  private int _width;

  /// <summary>
  ///   Position horizontale (X) en pixels.
  /// </summary>
  [ObservableProperty]
  private int _x;

  /// <summary>
  ///   Position verticale (Y) en pixels.
  /// </summary>
  [ObservableProperty]
  private int _y;

  #endregion

  #region Commands

  /// <summary>
  ///   Command to edit this fence.
  /// </summary>
  [RelayCommand]
  public void Edit()
  {
    if (_dialogService != null)
    {
      _dialogService.ShowEditZoneDialog(Id, Name, Color, (int)(Opacity * 100));
    }
  }

  /// <summary>
  ///   Command to delete this fence.
  /// </summary>
  [RelayCommand]
  public async Task Delete()
  {
    if (_mediator == null)
    {
      return;
    }

    try
    {
      var command = new Corral.Application.Commands.DeleteFence.DeleteFenceCommand(Id);
      await _mediator.Send(command);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Error deleting fence: {ex.Message}");
    }
  }

  #endregion
}
