// ------------------------------------------------------------------------------------------------
// <copyright file="DropBehavior.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Brush = System.Windows.Media.Brush;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;

namespace Corral.Desktop.Behaviors;

/// <summary>
///   Attached-property behavior that enables file drag-and-drop on any <see cref="UIElement" />.
/// </summary>
/// <remarks>
///   <para>
///     Set <see cref="DropCommandProperty" /> on a <see cref="UIElement" /> to enable dropping
///     files from Windows Explorer. When files are dropped, the command is invoked with a
///     <see cref="string" />[] containing the dropped file paths.
///   </para>
///   <para>
///     Unlike <c>Behavior&lt;T&gt;</c>, attached properties work reliably inside DataTemplates
///     defined in ResourceDictionaries because each element gets its own property value.
///   </para>
/// </remarks>
public static class DropBehavior
{
  #region Dependencies

  #region Property changed callback

  /// <summary>
  ///   Handles changes to the <see cref="DropCommandProperty" /> attached property.
  /// </summary>
  /// <param name="d">The <see cref="DependencyObject" /> on which the property value has changed.</param>
  /// <param name="e">The event data containing information about the property change.</param>
  /// <remarks>
  ///   This method is invoked when the value of the <see cref="DropCommandProperty" /> changes.
  ///   It attaches or detaches drag-and-drop event handlers on the target <see cref="UIElement" />
  ///   based on the new value of the property. If a valid <see cref="ICommand" /> is set,
  ///   drag-and-drop functionality is enabled for the element; otherwise, it is disabled.
  /// </remarks>
  private static void OnDropCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is UIElement element)
    {
      element.PreviewDragOver -= OnDragOver;
      element.PreviewDragLeave -= OnDragLeave;
      element.PreviewDrop -= OnDrop;

      if (e.NewValue is ICommand)
      {
        element.AllowDrop = true;
        element.PreviewDragOver += OnDragOver;
        element.PreviewDragLeave += OnDragLeave;
        element.PreviewDrop += OnDrop;
      }
      else
      {
        element.AllowDrop = false;
      }
    }
  }

  #endregion

  #endregion

  #region Attached properties

  /// <summary>
  ///   Command invoked when files are dropped. Setting this property automatically enables
  ///   <see cref="UIElement.AllowDrop" /> and wires up drag-and-drop event handlers.
  /// </summary>
  public static readonly DependencyProperty DropCommandProperty =
    DependencyProperty.RegisterAttached(
      "DropCommand",
      typeof(ICommand),
      typeof(DropBehavior),
      new PropertyMetadata(null, OnDropCommandChanged)
    );

  /// <summary>
  ///   Gets the drop command for the specified element.
  /// </summary>
  public static ICommand GetDropCommand(DependencyObject obj)
  {
    return (ICommand)obj.GetValue(DropCommandProperty);
  }

  /// <summary>
  ///   Sets the drop command for the specified element.
  /// </summary>
  public static void SetDropCommand(DependencyObject obj, ICommand value)
  {
    obj.SetValue(DropCommandProperty, value);
  }

  /// <summary>
  ///   Optional brush applied to the target element while the user drags files over it.
  /// </summary>
  public static readonly DependencyProperty DragOverHighlightProperty =
    DependencyProperty.RegisterAttached(
      "DragOverHighlight",
      typeof(Brush),
      typeof(DropBehavior),
      new PropertyMetadata(null)
    );

  /// <summary>
  ///   Gets the drag-over highlight brush for the specified element.
  /// </summary>
  public static Brush GetDragOverHighlight(DependencyObject obj)
  {
    return (Brush)obj.GetValue(DragOverHighlightProperty);
  }

  /// <summary>
  ///   Sets the drag-over highlight brush for the specified element.
  /// </summary>
  public static void SetDragOverHighlight(DependencyObject obj, Brush value)
  {
    obj.SetValue(DragOverHighlightProperty, value);
  }

  /// <summary>
  ///   Stores the original background brush so it can be restored after a drag-over.
  /// </summary>
  private static readonly DependencyProperty OriginalBackgroundProperty =
    DependencyProperty.RegisterAttached(
      "OriginalBackground",
      typeof(Brush),
      typeof(DropBehavior),
      new PropertyMetadata(null)
    );

  #endregion

  #region Event handlers

  /// <summary>
  ///   Handles the <see cref="UIElement.PreviewDragOver" /> event to provide feedback during a
  ///   drag-and-drop operation.
  /// </summary>
  /// <param name="sender">
  ///   The source of the event, typically the <see cref="UIElement" /> being dragged
  ///   over.
  /// </param>
  /// <param name="e">The event data containing information about the drag event.</param>
  /// <remarks>
  ///   This method checks if the dragged data contains file paths and sets the appropriate drag-and-drop
  ///   effects.
  ///   If the data is valid, it applies a highlight to the target element.
  /// </remarks>
  private static void OnDragOver(object sender, DragEventArgs e)
  {
    if (e.Data.GetDataPresent(DataFormats.FileDrop))
    {
      e.Effects = DragDropEffects.Copy;
      if (sender is UIElement element)
      {
        ApplyHighlight(element);
      }
    }
    else
    {
      e.Effects = DragDropEffects.None;
    }

    e.Handled = true;
  }

  /// <summary>
  ///   Handles the <see cref="UIElement.PreviewDragLeave" /> event for the target element.
  /// </summary>
  /// <param name="sender">
  ///   The source of the event, typically the <see cref="UIElement" /> where the drag operation is
  ///   leaving.
  /// </param>
  /// <param name="e">
  ///   The event data containing information about the drag operation.
  /// </param>
  /// <remarks>
  ///   This method restores the background of the target element when a drag operation leaves its
  ///   bounds.
  /// </remarks>
  private static void OnDragLeave(object sender, DragEventArgs e)
  {
    if (sender is UIElement element)
    {
      RestoreBackground(element);
    }
  }

  /// <summary>
  ///   Handles the <see cref="UIElement.PreviewDrop" /> event to process dropped files.
  /// </summary>
  /// <param name="sender">
  ///   The source of the event, expected to be a <see cref="UIElement" />.
  /// </param>
  /// <param name="e">
  ///   The event data containing information about the drag-and-drop operation.
  /// </param>
  /// <remarks>
  ///   This method restores the background of the target element, checks if the dropped data contains
  ///   file paths,
  ///   and invokes the associated <see cref="ICommand" /> with the file paths if applicable.
  /// </remarks>
  private static void OnDrop(object sender, DragEventArgs e)
  {
    if (sender is UIElement element)
    {
      RestoreBackground(element);
    }

    if (!e.Data.GetDataPresent(DataFormats.FileDrop))
    {
      return;
    }

    if (e.Data.GetData(DataFormats.FileDrop) is not string[] paths || paths.Length == 0)
    {
      return;
    }

    if (sender is DependencyObject dep)
    {
      var command = GetDropCommand(dep);
      if (command?.CanExecute(paths) == true)
      {
        command.Execute(paths);
      }
    }

    e.Handled = true;
  }

  #endregion

  #region Helpers

  /// <summary>
  ///   Applies a highlight effect to the specified <see cref="UIElement" /> during a drag-and-drop
  ///   operation.
  /// </summary>
  /// <param name="element">
  ///   The <see cref="UIElement" /> to which the highlight effect will be applied. Typically, this is
  ///   the element
  ///   over which files are being dragged.
  /// </param>
  /// <remarks>
  ///   If the <paramref name="element" /> is a <see cref="Border" /> and a highlight brush is defined
  ///   via the
  ///   <see cref="DragOverHighlightProperty" />, the method temporarily replaces the border's background
  ///   with the
  ///   highlight brush. The original background is preserved and can be restored later.
  /// </remarks>
  private static void ApplyHighlight(UIElement element)
  {
    if (element is Border border && GetDragOverHighlight(border) is { } highlight)
    {
      if (border.GetValue(OriginalBackgroundProperty) == null)
      {
        border.SetValue(OriginalBackgroundProperty, border.Background);
      }

      border.Background = highlight;
    }
  }

  /// <summary>
  ///   Restores the original background of the specified <see cref="UIElement" />.
  /// </summary>
  /// <param name="element">
  ///   The <see cref="UIElement" /> whose background is to be restored. Typically, this is a
  ///   <see cref="Border" />
  ///   that had its background temporarily changed during a drag-and-drop operation.
  /// </param>
  /// <remarks>
  ///   If the <paramref name="element" /> is a <see cref="Border" /> and an original background was
  ///   previously
  ///   stored using the <c>OriginalBackgroundProperty</c>, this method restores the background to its
  ///   original value
  ///   and clears the stored property.
  /// </remarks>
  private static void RestoreBackground(UIElement element)
  {
    if (element is Border border && border.GetValue(OriginalBackgroundProperty) is Brush original)
    {
      border.Background = original;
      border.ClearValue(OriginalBackgroundProperty);
    }
  }

  #endregion
}
