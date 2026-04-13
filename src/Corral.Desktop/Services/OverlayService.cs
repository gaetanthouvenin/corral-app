// ------------------------------------------------------------------------------------------------
// <copyright file="OverlayService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Application.Commands.MoveFence;
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
  IServiceProvider serviceProvider) : IOverlayService
{
  #region Fields

  private readonly Dictionary<string, FenceOverlayWindow> _overlays = new();

  #endregion

  #region Properties

  /// <inheritdoc />
  public bool AreOverlaysVisible => _overlays.Count > 0;

  #endregion

  #region Methods

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
#pragma warning restore VSTHRD001, VSTHRD110

  #endregion

  #region Implementation of IOverlayService

  /// <inheritdoc />
  public void ShowOverlay(FenceViewModel fence)
  {
    if (_overlays.ContainsKey(fence.Id))
    {
      UpdateOverlay(fence);
      return;
    }

    var overlay = new FenceOverlayWindow();
    overlay.UpdateFenceDisplay(
      fence.Name,
      fence.X,
      fence.Y,
      fence.Width,
      fence.Height,
      fence.Color,
      fence.Opacity,
      fence.Items
    );

    var fenceId = fence.Id;
    overlay.PositionChanged += (newX, newY) => OnOverlayPositionChanged(fenceId, newX, newY);

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
  public void UpdateOverlay(FenceViewModel fence)
  {
    if (_overlays.TryGetValue(fence.Id, out var overlay))
    {
      overlay.UpdateFenceDisplay(
        fence.Name,
        fence.X,
        fence.Y,
        fence.Width,
        fence.Height,
        fence.Color,
        fence.Opacity,
        fence.Items
      );
    }
  }

  /// <inheritdoc />
  public void ShowAllOverlays(IEnumerable<FenceViewModel> fences)
  {
    foreach (var fence in fences)
    {
      if (fence.IsActive)
      {
        ShowOverlay(fence);
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

  #endregion
}
