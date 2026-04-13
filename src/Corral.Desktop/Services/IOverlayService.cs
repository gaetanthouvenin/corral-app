// ------------------------------------------------------------------------------------------------
// <copyright file="IOverlayService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Desktop.ViewModels;

namespace Corral.Desktop.Services;

/// <summary>
///   Service for managing fence overlay windows on the desktop.
/// </summary>
public interface IOverlayService
{
  #region Properties

  /// <summary>
  ///   Gets whether overlays are currently visible.
  /// </summary>
  bool AreOverlaysVisible { get; }

  #endregion

  #region Methods

  /// <summary>
  ///   Shows an overlay window for the specified fence.
  /// </summary>
  /// <param name="fence">The fence view model to display as an overlay.</param>
  void ShowOverlay(FenceViewModel fence);

  /// <summary>
  ///   Hides the overlay window for the specified fence.
  /// </summary>
  /// <param name="fenceId">The ID of the fence whose overlay should be hidden.</param>
  void HideOverlay(string fenceId);

  /// <summary>
  ///   Updates an existing overlay window with new fence data.
  /// </summary>
  /// <param name="fence">The updated fence view model.</param>
  void UpdateOverlay(FenceViewModel fence);

  /// <summary>
  ///   Shows overlays for all active fences.
  /// </summary>
  /// <param name="fences">The collection of fence view models to display.</param>
  void ShowAllOverlays(IEnumerable<FenceViewModel> fences);

  /// <summary>
  ///   Hides all overlay windows.
  /// </summary>
  void HideAllOverlays();

  #endregion
}
