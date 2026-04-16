// ------------------------------------------------------------------------------------------------
// <copyright file="ReorderItemsBehavior.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;

namespace Corral.Desktop.Behaviors;

/// <summary>
///   Attached behavior that enables drag-and-drop reordering inside an <see cref="ItemsControl" />.
/// </summary>
public static class ReorderItemsBehavior
{
  #region Dependencies

  #region Property changed callback

  private static void OnReorderCommandChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (d is not ItemsControl itemsControl)
    {
      return;
    }

    itemsControl.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
    itemsControl.PreviewMouseMove -= OnPreviewMouseMove;
    itemsControl.PreviewDragOver -= OnPreviewDragOver;
    itemsControl.PreviewDrop -= OnPreviewDrop;
    itemsControl.DragLeave -= OnDragLeave;

    if (e.NewValue is ICommand)
    {
      itemsControl.AllowDrop = true;
      itemsControl.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
      itemsControl.PreviewMouseMove += OnPreviewMouseMove;
      itemsControl.PreviewDragOver += OnPreviewDragOver;
      itemsControl.PreviewDrop += OnPreviewDrop;
      itemsControl.DragLeave += OnDragLeave;
    }
  }

  #endregion

  #endregion

  #region Fields

  #region Constants

  private const string DraggedItemDataFormat = "Corral.Desktop.DraggedFenceItem";

  #endregion

  #endregion

  #region Attached properties

  /// <summary>
  ///   Command invoked when an item is reordered.
  ///   Receives an <see cref="object" />[] containing source item and target item (or null for end).
  /// </summary>
  public static readonly DependencyProperty ReorderCommandProperty =
    DependencyProperty.RegisterAttached(
      "ReorderCommand",
      typeof(ICommand),
      typeof(ReorderItemsBehavior),
      new PropertyMetadata(null, OnReorderCommandChanged)
    );

  private static readonly DependencyProperty DragStartPointProperty =
    DependencyProperty.RegisterAttached(
      "DragStartPoint",
      typeof(Point),
      typeof(ReorderItemsBehavior),
      new PropertyMetadata(default(Point))
    );

  private static readonly DependencyProperty DraggedItemProperty =
    DependencyProperty.RegisterAttached(
      "DraggedItem",
      typeof(object),
      typeof(ReorderItemsBehavior),
      new PropertyMetadata(null)
    );

  private static readonly DependencyProperty CurrentDropContainerProperty =
    DependencyProperty.RegisterAttached(
      "CurrentDropContainer",
      typeof(FrameworkElement),
      typeof(ReorderItemsBehavior),
      new PropertyMetadata(null)
    );

  /// <summary>
  ///   Gets or sets whether this item container is the current drag-and-drop target.
  ///   Used to show a visual drop indicator.
  /// </summary>
  public static readonly DependencyProperty IsDropTargetProperty =
    DependencyProperty.RegisterAttached(
      "IsDropTarget",
      typeof(bool),
      typeof(ReorderItemsBehavior),
      new PropertyMetadata(false)
    );

  private static readonly DependencyProperty IsItemBeingDraggedProperty =
    DependencyProperty.RegisterAttached(
      "IsItemBeingDragged",
      typeof(bool),
      typeof(ReorderItemsBehavior),
      new PropertyMetadata(false)
    );

  /// <summary>Gets the reorder command for the specified items control.</summary>
  public static ICommand GetReorderCommand(DependencyObject obj)
  {
    return (ICommand)obj.GetValue(ReorderCommandProperty);
  }

  /// <summary>Sets the reorder command for the specified items control.</summary>
  public static void SetReorderCommand(DependencyObject obj, ICommand value)
  {
    obj.SetValue(ReorderCommandProperty, value);
  }

  /// <summary>Gets whether the element is the current drag-and-drop target.</summary>
  public static bool GetIsDropTarget(DependencyObject obj)
  {
    return (bool)obj.GetValue(IsDropTargetProperty);
  }

  /// <summary>Sets whether the element is the current drag-and-drop target.</summary>
  public static void SetIsDropTarget(DependencyObject obj, bool value)
  {
    obj.SetValue(IsDropTargetProperty, value);
  }

  private static bool GetIsItemBeingDragged(DependencyObject obj)
  {
    return (bool)obj.GetValue(IsItemBeingDraggedProperty);
  }

  private static void SetIsItemBeingDragged(DependencyObject obj, bool value)
  {
    obj.SetValue(IsItemBeingDraggedProperty, value);
  }

  /// <summary>
  ///   Checks if an ItemsControl is currently dragging an item (used to prevent clicks during drag).
  /// </summary>
  internal static bool IsItemsControlDraggingItem(ItemsControl itemsControl)
  {
    return itemsControl != null && GetIsItemBeingDragged(itemsControl);
  }

  #endregion

  #region Event handlers

  private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    if (sender is not ItemsControl itemsControl)
    {
      return;
    }

    itemsControl.SetValue(DragStartPointProperty, e.GetPosition(itemsControl));

    var container =
      itemsControl.ContainerFromElement((DependencyObject)e.OriginalSource) as FrameworkElement;

    itemsControl.SetValue(DraggedItemProperty, container != null ? container.DataContext : null);
  }

  private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
  {
    if (sender is not ItemsControl itemsControl || e.LeftButton != MouseButtonState.Pressed)
    {
      return;
    }

    var draggedItem = itemsControl.GetValue(DraggedItemProperty);
    if (draggedItem == null)
    {
      return;
    }

    var startPoint = (Point)itemsControl.GetValue(DragStartPointProperty);
    var currentPoint = e.GetPosition(itemsControl);

    if (Math.Abs(currentPoint.X - startPoint.X) < SystemParameters.MinimumHorizontalDragDistance
        && Math.Abs(currentPoint.Y - startPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
    {
      return;
    }

    var dataObject = new DataObject(DraggedItemDataFormat, draggedItem);
    SetIsItemBeingDragged(itemsControl, true);
    DragDrop.DoDragDrop(itemsControl, dataObject, DragDropEffects.Move);
    SetIsItemBeingDragged(itemsControl, false);
    itemsControl.SetValue(DraggedItemProperty, null);
  }

  private static void OnPreviewDragOver(object sender, DragEventArgs e)
  {
    if (e.Data.GetDataPresent(DraggedItemDataFormat))
    {
      e.Effects = DragDropEffects.Move;

      if (sender is ItemsControl itemsControl)
      {
        var prev = (FrameworkElement)itemsControl.GetValue(CurrentDropContainerProperty);
        var container =
          itemsControl.ContainerFromElement((DependencyObject)e.OriginalSource) as FrameworkElement;

        if (prev != null && prev != container)
        {
          SetIsDropTarget(prev, false);
        }

        if (container != null)
        {
          SetIsDropTarget(container, true);
          itemsControl.SetValue(CurrentDropContainerProperty, container);
        }
      }

      e.Handled = true;
      return;
    }

    e.Effects = DragDropEffects.None;
    e.Handled = true;
  }

  private static void OnDragLeave(object sender, DragEventArgs e)
  {
    if (sender is not ItemsControl itemsControl)
    {
      return;
    }

    ClearDropTarget(itemsControl);
  }

  private static void ClearDropTarget(ItemsControl itemsControl)
  {
    var prev = (FrameworkElement)itemsControl.GetValue(CurrentDropContainerProperty);
    if (prev != null)
    {
      SetIsDropTarget(prev, false);
      itemsControl.SetValue(CurrentDropContainerProperty, null);
    }
  }

  private static void OnPreviewDrop(object sender, DragEventArgs e)
  {
    if (sender is not ItemsControl itemsControl)
    {
      return;
    }

    ClearDropTarget(itemsControl);

    if (!e.Data.GetDataPresent(DraggedItemDataFormat))
    {
      return;
    }

    var sourceItem = e.Data.GetData(DraggedItemDataFormat);
    if (sourceItem == null)
    {
      return;
    }

    var targetContainer =
      itemsControl.ContainerFromElement((DependencyObject)e.OriginalSource) as FrameworkElement;

    var targetItem = targetContainer != null ? targetContainer.DataContext : null;

    var command = GetReorderCommand(itemsControl);
    object[] parameter = [sourceItem, targetItem];

    if (command?.CanExecute(parameter) == true)
    {
      command.Execute(parameter);
    }

    e.Handled = true;
  }

  #endregion
}
