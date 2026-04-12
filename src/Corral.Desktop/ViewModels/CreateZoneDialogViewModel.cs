// ------------------------------------------------------------------------------------------------
// <copyright file="CreateZoneDialogViewModel.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corral.Application.Commands.CreateFence;

using MediatR;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   ViewModel pour le dialog de création d'une nouvelle zone.
/// </summary>
public partial class CreateZoneDialogViewModel(IMediator mediator) : ObservableObject
{
  #region Constructor

  /// <summary>
  ///   Constructeur sans paramètre pour le design-time XAML.
  /// </summary>
  public CreateZoneDialogViewModel() : this(null)
  {
    ZoneName = "Ma Nouvelle Zone";
  }

  #endregion

  #region Observable Properties

  /// <summary>Nom de la zone à créer.</summary>
  [ObservableProperty]
  [NotifyCanExecuteChangedFor(nameof(CreateZoneCommand))]
  private string _zoneName = string.Empty;

  /// <summary>Couleur sélectionnée au format #AARRGGBB.</summary>
  [ObservableProperty]
  private string _selectedColor = "#FF1A80E5";

  /// <summary>Aperçu de la couleur sélectionnée pour la UI.</summary>
  [ObservableProperty]
  private SolidColorBrush _colorPreview = new(Colors.DodgerBlue);

  /// <summary>Opacité de la zone (0-100).</summary>
  [ObservableProperty]
  private int _opacity = 100;

  /// <summary>Indique si la création est en cours.</summary>
  [ObservableProperty]
  private bool _isCreating;

  /// <summary>Message d'erreur à afficher si la création échoue.</summary>
  [ObservableProperty]
  private string _errorMessage = string.Empty;

  #endregion

  #region Color Presets

  /// <summary>Liste des couleurs prédéfinies proposées à l'utilisateur.</summary>
  public IReadOnlyList<ColorPreset> ColorPresets { get; } =
  [
    new("#FF1A80E5", "Bleu"),
    new("#FFFF5277", "Rose"),
    new("#FF2BD2D2", "Cyan"),
    new("#FFFF9800", "Orange"),
    new("#FFA15BD7", "Violet"),
    new("#FF737373", "Gris"),
  ];

  #endregion

  #region Callbacks (set by code-behind)

  /// <summary>
  ///   Callback appelé quand le dialog doit se fermer.
  ///   Le paramètre bool indique si la création a réussi.
  /// </summary>
  public Action<bool> CloseDialog { get; set; }

  #endregion

  #region Commands

  /// <summary>Sélectionne une couleur prédéfinie.</summary>
  [RelayCommand]
  private void SelectColor(string colorHex)
  {
    SelectedColor = colorHex;
    UpdateColorPreview();
  }

  /// <summary>Crée la zone et ferme le dialog.</summary>
  [RelayCommand(CanExecute = nameof(CanCreateZone))]
#pragma warning disable VSTHRD002
  private async Task CreateZone()
  {
    if (mediator == null)
    {
      if (CloseDialog != null) CloseDialog(false);
      return;
    }

    try
    {
      IsCreating = true;
      ErrorMessage = string.Empty;

      var command = new CreateFenceCommand(
        Name: ZoneName.Trim(),
        PositionX: 100,
        PositionY: 100,
        Width: 800,
        Height: 600,
        BackgroundColor: SelectedColor,
        Opacity: Opacity);

      await mediator.Send(command);
      if (CloseDialog != null) CloseDialog(true);
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

  private bool CanCreateZone() => !string.IsNullOrWhiteSpace(ZoneName) && !IsCreating;

  /// <summary>Annule et ferme le dialog.</summary>
  [RelayCommand]
  private void Cancel()
  {
    if (CloseDialog != null) CloseDialog(false);
  }

  #endregion

  #region Helpers

  partial void OnSelectedColorChanged(string value) => UpdateColorPreview();

  private void UpdateColorPreview()
  {
    try
    {
      var color = (Color)ColorConverter.ConvertFromString(SelectedColor);
      ColorPreview = new SolidColorBrush(color);
    }
    catch
    {
      ColorPreview = new SolidColorBrush(Colors.Gray);
    }
  }

  #endregion
}

/// <summary>
///   Représente une couleur prédéfinie avec son code hex et son label.
/// </summary>
/// <param name="Hex">Code couleur au format #AARRGGBB.</param>
/// <param name="Label">Nom lisible de la couleur.</param>
public record ColorPreset(string Hex, string Label);
