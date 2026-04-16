// ------------------------------------------------------------------------------------------------
// <copyright file="OverlayService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Application.Commands.MoveFence;
using Corral.Application.Commands.ResizeFence;
using Corral.Desktop.ViewModels;
using Corral.Desktop.Views;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corral.Desktop.Services;

/// <summary>
///   Manages fence overlay windows on the desktop.
///   Creates, updates, and destroys transparent overlay windows for each active fence.
/// </summary>
public class OverlayService(
  ILogger<OverlayService> logger,
  IServiceProvider serviceProvider,
  IUserPreferencesService preferencesService) : IOverlayService
{
  #region Fields

  private readonly Dictionary<string, FenceOverlayWindow> _overlays = new();

  #endregion

  #region Properties

  /// <inheritdoc />
  public bool AreOverlaysVisible => _overlays.Count > 0;

  #endregion

  #region Implementation of IOverlayService

  /// <inheritdoc />
  public async Task ShowOverlay(FenceViewModel fence)
  {
    if (_overlays.ContainsKey(fence.Id))
    {
      await UpdateOverlay(fence);
      return;
    }

    var overlay = new FenceOverlayWindow();
    var prefs = await preferencesService.GetPreferencesAsync();
    overlay.SetViewModel(fence);
    overlay.UpdateFenceDisplay(
      fence.Name,
      fence.X,
      fence.Y,
      fence.Width,
      fence.Height,
      fence.Color,
      fence.Opacity,
      fence.Items,
      prefs.ClickMode,
      prefs.IconLayout
    );

    var fenceId = fence.Id;
    overlay.PositionChanged += (newX, newY) => OnOverlayPositionChanged(fenceId, newX, newY);
    overlay.DimensionsChanged += (newWidth, newHeight)
                                   => OnOverlayDimensionsChanged(fenceId, newWidth, newHeight);

    _overlays[fence.Id] = overlay;
    overlay.Show();

    logger.LogDebug("Overlay shown for fence '{FenceName}' ({FenceId})", fence.Name, fence.Id);
  }

  /// <inheritdoc />
  public void HideOverlay(string fenceId)
  {
    if (_overlays.TryGetValue(fenceId, out var overlay))
    {
      overlay.Close();
      _overlays.Remove(fenceId);
      logger.LogDebug("Overlay hidden for fence {FenceId}", fenceId);
    }
  }

  /// <inheritdoc />
  public async Task UpdateOverlay(FenceViewModel fence)
  {
    if (_overlays.TryGetValue(fence.Id, out var overlay))
    {
      var prefs = await preferencesService.GetPreferencesAsync();
      overlay.UpdateFenceDisplay(
        fence.Name,
        fence.X,
        fence.Y,
        fence.Width,
        fence.Height,
        fence.Color,
        fence.Opacity,
        fence.Items,
        prefs.ClickMode,
        prefs.IconLayout
      );
    }
  }

  /// <inheritdoc />
  public async Task ShowAllOverlays(IEnumerable<FenceViewModel> fences)
  {
    foreach (var fence in fences)
    {
      if (fence.IsActive)
      {
        await ShowOverlay(fence);
      }
    }

    logger.LogInformation("All overlays shown ({Count} fences)", _overlays.Count);
  }

  /// <inheritdoc />
  public void HideAllOverlays()
  {
    foreach (var overlay in _overlays.Values)
    {
      overlay.Close();
    }

    var count = _overlays.Count;
    _overlays.Clear();

    logger.LogInformation("All overlays hidden ({Count} closed)", count);
  }

  /// <summary>
  ///   Refreshes click mode and icon layout on all visible overlays when preferences change.
  /// </summary>
  public async Task RefreshAllOverlayLayouts()
  {
    var prefs = await preferencesService.GetPreferencesAsync();

    foreach (var overlay in _overlays.Values)
    {
      overlay.RefreshPreferences(prefs.ClickMode, prefs.IconLayout);
    }

    logger.LogDebug("Refreshed preferences for {OverlayCount} overlays", _overlays.Count);
  }

  #endregion

#pragma warning disable VSTHRD001, VSTHRD110
  private void OnOverlayPositionChanged(string fenceId, int newX, int newY)
  {
    logger.LogDebug("Fence {FenceId} moved to ({X}, {Y})", fenceId, newX, newY);
    System.Windows.Application.Current.Dispatcher.BeginInvoke(async () =>
                                                              {
                                                                try
                                                                {
                                                                  using var scope =
                                                                    serviceProvider.CreateScope();

                                                                  var mediator =
                                                                    scope.ServiceProvider
                                                                      .GetRequiredService<
                                                                        IMediator>();

                                                                  var command =
                                                                    new MoveFenceCommand(
                                                                      fenceId,
                                                                      newX,
                                                                      newY
                                                                    );

                                                                  await mediator.Send(command);
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                  logger.LogError(
                                                                    ex,
                                                                    "Failed to persist fence position for {FenceId}",
                                                                    fenceId
                                                                  );
                                                                }
                                                              }
    );
  }

  private void OnOverlayDimensionsChanged(string fenceId, int newWidth, int newHeight)
  {
    logger.LogDebug("Fence {FenceId} resized to ({Width}x{Height})", fenceId, newWidth, newHeight);
    System.Windows.Application.Current.Dispatcher.BeginInvoke(async () =>
                                                              {
                                                                try
                                                                {
                                                                  using var scope =
                                                                    serviceProvider.CreateScope();

                                                                  var mediator =
                                                                    scope.ServiceProvider
                                                                      .GetRequiredService<
                                                                        IMediator>();

                                                                  var command =
                                                                    new ResizeFenceCommand(
                                                                      fenceId,
                                                                      newWidth,
                                                                      newHeight
                                                                    );

                                                                  await mediator.Send(command);
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                  logger.LogError(
                                                                    ex,
                                                                    "Failed to persist fence dimensions for {FenceId}",
                                                                    fenceId
                                                                  );
                                                                }
                                                              }
    );
  }
#pragma warning restore VSTHRD001, VSTHRD110
}
