// ------------------------------------------------------------------------------------------------
// <copyright file="FenceViewModel.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corral.Application.Commands.AddItemToFence;
using Corral.Application.Commands.DeleteFence;
using Corral.Desktop.Services;

using MediatR;

namespace Corral.Desktop.ViewModels;

/// <summary>
///   Represents a fence (zone) in the Presentation layer using the MVVM pattern.
/// </summary>
/// <remarks>
///   <para>
///     This ViewModel encapsulates the state and behaviors of a fence for data binding
///     to the WPF view. It uses the Community Toolkit MVVM for automatic management of
///     property change notifications via <see cref="ObservableObject" />.
///   </para>
///   <para>
///     All business properties (position, size, appearance, state) are exposed via
///     <see cref="ObservablePropertyAttribute" /> to enable two-way binding with the UI.
///     Contained items (<see cref="FenceItemViewModel" />) are managed in an observable collection.
///   </para>
///   <para>
///     Operations (editing, deletion) are implemented via relay commands
///     (<see cref="RelayCommand" />) that delegate to the dialog service or CQRS mediator.
///   </para>
/// </remarks>
/// <remarks>
///   Initializes a new instance of <see cref="FenceViewModel" />.
/// </remarks>
/// <param name="dialogService">Service for managing dialogs used for edit operations.</param>
/// <param name="mediator">CQRS mediator for sending commands (deletion, etc.).</param>
/// <remarks>
///   Both services are injected by the DI container and cannot be <c>null</c>.
/// </remarks>
public partial class FenceViewModel(IDialogService dialogService, IMediator mediator)
  : ObservableObject
{
  #region Fields

  /// <summary>
  ///   Background color in hexadecimal format <c>#AARRGGBB</c>.
  /// </summary>
  /// <remarks>
  ///   Default value: opaque white <c>#FFFFFFFF</c>.
  /// </remarks>
  [ObservableProperty]
  private string _color = "FFFFFFFF";

  /// <summary>
  ///   Date and time when the fence was created.
  /// </summary>
  [ObservableProperty]
  private DateTime _createdAt;

  /// <summary>
  ///   Height of the fence in pixels.
  /// </summary>
  [ObservableProperty]
  private int _height;

  /// <summary>
  ///   Unique identifier of the fence.
  /// </summary>
  [ObservableProperty]
  private string _id = string.Empty;

  /// <summary>
  ///   Indicates whether the fence is currently active or visible.
  /// </summary>
  [ObservableProperty]
  private bool _isActive;

  /// <summary>
  ///   Observable collection of items (shortcuts, files, links) contained in this fence.
  /// </summary>
  /// <remarks>
  ///   This collection is observable to allow the view to react to additions, deletions,
  ///   and modifications of items.
  /// </remarks>
  [ObservableProperty]
  private ObservableCollection<FenceItemViewModel> _items = [];

  /// <summary>
  ///   Name or title of the fence.
  /// </summary>
  [ObservableProperty]
  private string _name = string.Empty;

  /// <summary>
  ///   Opacity of the fence, ranging from 0.0 (fully transparent) to 1.0 (fully opaque).
  /// </summary>
  /// <remarks>
  ///   Default value: 1.0 (full opacity).
  /// </remarks>
  [ObservableProperty]
  private double _opacity = 1.0;

  /// <summary>
  ///   Date and time of the last modification of the fence.
  /// </summary>
  [ObservableProperty]
  private DateTime _updatedAt;

  /// <summary>
  ///   Width of the fence in pixels.
  /// </summary>
  [ObservableProperty]
  private int _width;

  /// <summary>
  ///   Horizontal position (X) of the fence in pixels.
  /// </summary>
  [ObservableProperty]
  private int _x;

  /// <summary>
  ///   Vertical position (Y) of the fence in pixels.
  /// </summary>
  [ObservableProperty]
  private int _y;

  #endregion

  #region Properties

  /// <summary>
  ///   Callback invoked after an edit or delete operation to refresh the parent list.
  /// </summary>
  /// <remarks>
  ///   Can be <c>null</c>; in that case, no refresh is performed.
  /// </remarks>
  public Func<Task> OnChanged { get; set; }

  #endregion

  #region Commands

  /// <summary>
  ///   Relay command to edit the properties of this fence.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Displays an edit dialog containing the name, color, and opacity.
  ///     If the user confirms the changes, the <see cref="OnChanged" /> callback is triggered.
  ///   </para>
  ///   <para>
  ///     This ensures that the parent container can refresh after the edit.
  ///   </para>
  /// </remarks>
  [RelayCommand]
  public async Task Edit()
  {
    if (dialogService != null)
    {
      var updated = dialogService.ShowEditZoneDialog(Id, Name, Color, (int)(Opacity * 100));
      if (updated && OnChanged != null)
      {
        await OnChanged();
      }
    }
  }

  /// <summary>
  ///   Relay command to delete this fence from the database.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Sends a <see cref="DeleteFenceCommand" /> via the CQRS mediator.
  ///     After successful deletion, the <see cref="OnChanged" /> callback is triggered
  ///     to allow the parent container to update.
  ///   </para>
  ///   <para>
  ///     Errors are logged to the debugger but do not propagate as exceptions.
  ///   </para>
  /// </remarks>
  [RelayCommand]
  public async Task Delete()
  {
    if (mediator != null)
    {
      try
      {
        var command = new DeleteFenceCommand(Id);
        await mediator.Send(command);
        if (OnChanged != null)
        {
          await OnChanged();
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error deleting fence: {ex.Message}");
      }
    }
  }

  /// <summary>
  ///   Relay command invoked when files are dropped onto the zone card.
  ///   Receives the dropped file paths from <see cref="Behaviors.DropBehavior" /> and
  ///   sends an <see cref="AddItemToFenceCommand" /> for each file via MediatR.
  /// </summary>
  /// <param name="paths">Array of full file paths dropped by the user.</param>
  [RelayCommand]
  public async Task DropFiles(string[] paths)
  {
    if (mediator != null && paths != null && paths.Length != 0)
    {
      try
      {
        foreach (var path in paths)
        {
          var itemType = DetectItemType(path);
          var displayName = Path.GetFileNameWithoutExtension(path);

          if (string.IsNullOrWhiteSpace(displayName))
          {
            displayName = path;
          }

          var command = new AddItemToFenceCommand(Id, displayName, path, (int)itemType);
          await mediator.Send(command);
        }

        if (OnChanged != null)
        {
          await OnChanged();
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error dropping files onto fence {Name}: {ex.Message}");
      }
    }
  }

  #endregion

  #region Helpers

  /// <summary>
  ///   Determines the <see cref="DropItemType" /> for a given path.
  ///   <list type="bullet">
  ///     <item><c>.lnk</c> extension → Shortcut (0)</item>
  ///     <item>HTTP or HTTPS URI → Link (2)</item>
  ///     <item>Anything else → File (1)</item>
  ///   </list>
  /// </summary>
  private static DropItemType DetectItemType(string path)
  {
    if (Path.GetExtension(path).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
    {
      return DropItemType.Shortcut;
    }

    if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && !uri.IsFile)
    {
      return DropItemType.Link;
    }

    return DropItemType.File;
  }

  /// <summary>
  ///   Item type discriminator used during drop detection.
  ///   Values map to <see cref="Corral.Domain.ValueObjects.FenceItemType" />.
  /// </summary>
  private enum DropItemType
  {
    Shortcut = 0,
    File = 1,
    Link = 2,
  }

  #endregion
}
