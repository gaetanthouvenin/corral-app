// ------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Gaëtan THOUVENIN">
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
///   ViewModel pour la fenêtre principale de Corral Manager.
/// </summary>
/// <remarks>
///   Initializes a new instance of the MainWindowViewModel class.
/// </remarks>
/// <param name="mediator">MediatR mediator for CQRS operations.</param>
/// <param name="fenceMapper">Mapper for transforming Fence aggregates to ViewModels.</param>
/// <param name="dialogService">Service for opening dialog windows.</param>
public partial class MainWindowViewModel(
  IMediator mediator,
  IMapper<Fence, FenceViewModel> fenceMapper,
  IDialogService dialogService) : ObservableObject
{
  #region Fields

  [ObservableProperty]
  private ObservableCollection<FenceViewModel> _fences = new();

  [ObservableProperty]
  private bool _isLoading;

  [ObservableProperty]
  private string _statusMessage = "Ready";

  [ObservableProperty]
  private string _title = "Corral Manager";

  [ObservableProperty]
  private string _searchTerm = string.Empty;

  #endregion

  #region Methods

  private async Task LoadZones()
  {
    try
    {
      IsLoading = true;
      StatusMessage = "Loading zones...";

      // Skip loading if running in design mode (null mediator)
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
  ///   Command to initialize the view model and load zones on window loaded event.
  /// </summary>
  [RelayCommand]
  public async Task Initialize()
  {
    await LoadZones();
  }

  /// <summary>
  ///   Command to create a new zone — opens the creation dialog.
  /// </summary>
  [RelayCommand]
  public async Task CreateNewZone()
  {
    bool created = dialogService.ShowCreateZoneDialog();
    if (created)
    {
      await LoadZones();
    }
  }

  /// <summary>
  ///   Command to refresh the zones list.
  /// </summary>
  [RelayCommand]
  public async Task RefreshZones()
  {
    await LoadZones();
  }

  /// <summary>
  ///   Command to delete a zone by ID.
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
  ///   Command to search zones by name.
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

  #endregion
}
