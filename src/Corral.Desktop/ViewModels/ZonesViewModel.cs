// ------------------------------------------------------------------------------------------------
// <copyright file="ZonesViewModel.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corral.Application.Commands.DeleteFence;
using Corral.Application.Queries.GetAllFences;
using Corral.Application.Queries.SearchFences;
using Corral.Desktop.Services;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Mappers;

using MediatR;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   View model for managing fence zones and their overlays.
/// </summary>
public partial class ZonesViewModel(
  IMediator mediator,
  IMapper<Fence, FenceViewModel> fenceMapper,
  IDialogService dialogService,
  IOverlayService overlayService) : ObservableObject
{
  #region Fields

  /// <summary>
  ///   Indicates whether overlay windows for active fences are currently visible.
  /// </summary>
  [ObservableProperty]
  private bool _areOverlaysVisible;

  /// <summary>
  ///   Observable collection of all fences currently displayed.
  /// </summary>
  [ObservableProperty]
  private ObservableCollection<FenceViewModel> _fences = [];

  /// <summary>
  ///   Indicates whether a loading operation is currently in progress.
  /// </summary>
  [ObservableProperty]
  private bool _isLoading;

  /// <summary>
  ///   Search term used to filter fences by name.
  /// </summary>
  [ObservableProperty]
  private string _searchTerm = string.Empty;

  /// <summary>
  ///   Status message displayed to the user.
  /// </summary>
  [ObservableProperty]
  private string _statusMessage = "Ready";

  #endregion

  #region Methods

  /// <summary>
  ///   Loads all fences from the application layer.
  /// </summary>
  private async Task LoadZones()
  {
    try
    {
      IsLoading = true;
      StatusMessage = "Loading zones...";

      if (mediator == null)
      {
        StatusMessage = $"Loaded {Fences.Count} zone(s) [Design-time]";
        return;
      }

      var query = new GetAllFencesQuery();
      var fences = await mediator.Send(query);
      var fenceViewModels = fenceMapper.MapList(fences);

      Fences.Clear();
      foreach (var fence in fenceViewModels)
      {
        fence.OnChanged = async () => await LoadZones();
        Fences.Add(fence);
      }

      StatusMessage = $"Loaded {Fences.Count} zone(s)";
    }
    catch (Exception ex)
    {
      StatusMessage = $"Error loading zones: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  #endregion

  #region Commands

  /// <summary>
  ///   Relay command to initialize the zones view.
  /// </summary>
  [RelayCommand]
  public async Task Initialize()
  {
    await LoadZones();
  }

  /// <summary>
  ///   Relay command to create a new zone.
  /// </summary>
  [RelayCommand]
  public async Task CreateNewZone()
  {
    var created = dialogService.ShowCreateZoneDialog();
    if (created)
    {
      await LoadZones();
    }
  }

  /// <summary>
  ///   Relay command to refresh the zones list.
  /// </summary>
  [RelayCommand]
  public async Task RefreshZones()
  {
    await LoadZones();
  }

  /// <summary>
  ///   Relay command to delete a zone.
  /// </summary>
  [RelayCommand]
  public async Task DeleteZone(string fenceId)
  {
    try
    {
      var command = new DeleteFenceCommand(fenceId);
      await mediator.Send(command);
      await LoadZones();
    }
    catch (Exception ex)
    {
      StatusMessage = $"Error deleting zone: {ex.Message}";
    }
  }

  /// <summary>
  ///   Relay command to search zones by name.
  /// </summary>
  [RelayCommand]
  public async Task SearchZones()
  {
    try
    {
      IsLoading = true;

      if (mediator == null)
      {
        StatusMessage = "Mediator not available";
        return;
      }

      if (string.IsNullOrWhiteSpace(SearchTerm))
      {
        await LoadZones();
        return;
      }

      var query = new SearchFencesQuery(SearchTerm);
      var fences = await mediator.Send(query);
      var fenceViewModels = fenceMapper.MapList(fences);

      Fences.Clear();
      foreach (var fence in fenceViewModels)
      {
        fence.OnChanged = async () => await LoadZones();
        Fences.Add(fence);
      }

      StatusMessage = $"Found {Fences.Count} zone(s) matching '{SearchTerm}'";
    }
    catch (Exception ex)
    {
      StatusMessage = $"Error searching zones: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  /// <summary>
  ///   Relay command to toggle the visibility of overlay windows.
  /// </summary>
  [RelayCommand]
  public async Task ToggleOverlays()
  {
    if (overlayService != null)
    {
      if (AreOverlaysVisible)
      {
        overlayService.HideAllOverlays();
        AreOverlaysVisible = false;
        StatusMessage = "Overlays hidden";
      }
      else
      {
        var activeFences = Fences.Where(f => f.IsActive).ToList();
        await overlayService.ShowAllOverlays(activeFences);
        AreOverlaysVisible = true;
        StatusMessage = $"Showing {activeFences.Count} overlay(s)";
      }
    }
  }

  #endregion
}
