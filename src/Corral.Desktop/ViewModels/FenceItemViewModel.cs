// ------------------------------------------------------------------------------------------------
// <copyright file="FenceItemViewModel.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   Represents a ViewModel for an item (shortcut, file, link) within a fence.
/// </summary>
public partial class FenceItemViewModel : ObservableObject
{
  #region Fields

  /// <summary>
  ///   Display name of the element.
  /// </summary>
  [ObservableProperty]
  private string _displayName = string.Empty;

  /// <summary>
  ///   Unique identifier of the element.
  /// </summary>
  [ObservableProperty]
  private string _id = string.Empty;

  /// <summary>
  ///   Type of element (0 = Shortcut, 1 = File, 2 = Link).
  /// </summary>
  [ObservableProperty]
  private int _itemType;

  /// <summary>
  ///   Full path to the file, folder, executable, or URL.
  /// </summary>
  [ObservableProperty]
  private string _path = string.Empty;

  /// <summary>
  ///   Display order within the fence.
  /// </summary>
  [ObservableProperty]
  private int _sortOrder;

  /// <summary>
  ///   Shell-resolved icon (shortcut target, file association, etc.). Bound to the UI.
  /// </summary>
  [ObservableProperty]
  private ImageSource _icon;

  #endregion
}
