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
  Task ShowOverlay(FenceViewModel fence);

  /// <summary>
  ///   Hides the overlay window for the specified fence.
  /// </summary>
  void HideOverlay(string fenceId);

  /// <summary>
  ///   Updates an existing overlay window with new fence data.
  /// </summary>
  Task UpdateOverlay(FenceViewModel fence);

  /// <summary>
  ///   Shows overlays for all active fences.
  /// </summary>
  Task ShowAllOverlays(IEnumerable<FenceViewModel> fences);

  /// <summary>
  ///   Hides all overlay windows.
  /// </summary>
  void HideAllOverlays();

  /// <summary>
  ///   Refreshes the layout of all visible overlays (called when user preferences change).
  /// </summary>
  Task RefreshAllOverlayLayouts();

  #endregion
}
