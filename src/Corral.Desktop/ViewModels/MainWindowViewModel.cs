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
///   Main window view model for the Corral Manager application.
/// </summary>
/// <remarks>
///   <para>
///     This ViewModel manages the main application window state and coordinates between
///     the UI layer and the application layer via CQRS commands and queries.
///   </para>
///   <para>
///     Key responsibilities include:
///     <list type="bullet">
///       <item>Loading and displaying the list of fences (zones)</item>
///       <item>Searching fences by name</item>
///       <item>Managing fence creation and deletion</item>
///       <item>Toggling visibility of overlay windows for active fences</item>
///       <item>Providing status updates and loading indicators to the UI</item>
///     </list>
///   </para>
///   <para>
///     The view model uses dependency injection for the mediator, mappers, and services,
///     enabling testability and loose coupling with infrastructure concerns.
///   </para>
/// </remarks>
public partial class MainWindowViewModel(
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
  ///   Observable collection of all fences currently displayed in the main window.
  /// </summary>
  /// <remarks>
  ///   This collection is populated by the <see cref="LoadZones" /> method and updated
  ///   whenever fences are created, deleted, or search results are displayed.
  /// </remarks>
  [ObservableProperty]
  private ObservableCollection<FenceViewModel> _fences = [];

  /// <summary>
  ///   Indicates whether a loading operation is currently in progress.
  /// </summary>
  /// <remarks>
  ///   Used to display loading indicators in the UI while fetching or searching fences.
  /// </remarks>
  [ObservableProperty]
  private bool _isLoading;

  /// <summary>
  ///   Search term used to filter fences by name.
  /// </summary>
  /// <remarks>
  ///   When not empty, the <see cref="SearchZones" /> command uses this term to query fences.
  /// </remarks>
  [ObservableProperty]
  private string _searchTerm = string.Empty;

  /// <summary>
  ///   Status message displayed to the user in the UI.
  /// </summary>
  /// <remarks>
  ///   Updated by various commands to inform the user of the current operation status,
  ///   loaded count, search results, or error messages.
  /// </remarks>
  [ObservableProperty]
  private string _statusMessage = "Ready";

  /// <summary>
  ///   Title of the main window.
  /// </summary>
  [ObservableProperty]
  private string _title = "Corral Manager";

  #endregion

  #region Methods

  /// <summary>
  ///   Loads all fences from the application layer and populates the <see cref="Fences" /> collection.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This method executes a <see cref="GetAllFencesQuery" /> to retrieve all fences
  ///     from the database, maps them to <see cref="FenceViewModel" /> instances,
  ///     and updates the observable collection.
  ///   </para>
  ///   <para>
  ///     Each fence view model's <see cref="FenceViewModel.OnChanged" /> callback is set to
  ///     reload zones when the fence is edited or deleted, ensuring the UI stays synchronized
  ///     with the domain model.
  ///   </para>
  ///   <para>
  ///     In design-mode (null mediator), the method skips the query and returns early.
  ///   </para>
  /// </remarks>
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
  ///   Relay command to initialize the view model when the window is loaded.
  /// </summary>
  /// <remarks>
  ///   This command triggers the initial loading of all fences from the database
  ///   and should be invoked when the main window's <c>Loaded</c> event fires.
  /// </remarks>
  [RelayCommand]
  public async Task Initialize()
  {
    await LoadZones();
  }

  /// <summary>
  ///   Relay command to create a new zone by displaying the creation dialog.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Opens a dialog window allowing the user to enter fence details (name, color, opacity).
  ///     If the user confirms the dialog, the fences list is reloaded to display the new zone.
  ///   </para>
  ///   <para>
  ///     If the dialog is cancelled, no action is taken.
  ///   </para>
  /// </remarks>
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
  ///   Relay command to refresh the zones list from the database.
  /// </summary>
  /// <remarks>
  ///   Reloads all fences by calling <see cref="LoadZones" />, useful when external changes
  ///   may have occurred or to manually refresh the displayed list.
  /// </remarks>
  [RelayCommand]
  public async Task RefreshZones()
  {
    await LoadZones();
  }

  /// <summary>
  ///   Relay command to delete a zone by its unique identifier.
  /// </summary>
  /// <param name="fenceId">The unique identifier of the fence to delete.</param>
  /// <remarks>
  ///   <para>
  ///     Sends a <see cref="DeleteFenceCommand" /> via the CQRS mediator to remove the fence
  ///     from the database. After successful deletion, the fences list is reloaded.
  ///   </para>
  ///   <para>
  ///     Any exceptions during deletion are caught and displayed in the status message.
  ///   </para>
  /// </remarks>
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
  ///   Relay command to search zones by name using the current search term.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Executes a <see cref="SearchFencesQuery" /> with the <see cref="SearchTerm" /> value.
  ///     If the search term is empty or whitespace, the command reloads all zones instead.
  ///   </para>
  ///   <para>
  ///     Results are displayed in the <see cref="Fences" /> collection, and the status message
  ///     is updated with the number of matching zones.
  ///   </para>
  ///   <para>
  ///     Any exceptions during search are caught and displayed in the status message.
  ///   </para>
  /// </remarks>
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
  ///   Relay command to toggle the visibility of overlay windows for active fences.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     If overlays are currently visible, this command hides all of them via the overlay service.
  ///     Otherwise, it displays overlay windows for all active (IsActive = true) fences.
  ///   </para>
  ///   <para>
  ///     The <see cref="AreOverlaysVisible" /> flag is updated to reflect the current state,
  ///     and a corresponding status message is displayed.
  ///   </para>
  /// </remarks>
  [RelayCommand]
  public void ToggleOverlays()
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
        overlayService.ShowAllOverlays(activeFences);
        AreOverlaysVisible = true;
        StatusMessage = $"Showing {activeFences.Count} overlay(s)";
      }
    }
  }

  #endregion
}
