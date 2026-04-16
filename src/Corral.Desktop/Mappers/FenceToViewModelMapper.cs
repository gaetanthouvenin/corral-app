// ------------------------------------------------------------------------------------------------
// <copyright file="FenceToViewModelMapper.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;

using Corral.Desktop.Services;
using Corral.Desktop.ViewModels;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Mappers;

using MediatR;

namespace Corral.Desktop.Mappers;

/// <summary>
///   Provides functionality to map a <see cref="Corral.Domain.Aggregates.Fence" /> object
///   from the domain layer to a <see cref="Corral.Desktop.ViewModels.FenceViewModel" />
///   object for use in the presentation layer.
/// </summary>
/// <remarks>
///   This mapper ensures that domain objects are transformed into a format suitable
///   for UI binding by adapting complex domain types (e.g., value objects) into
///   simpler types (e.g., integers, strings, doubles). This approach maintains
///   separation of concerns between the domain and presentation layers.
/// </remarks>
public class FenceToViewModelMapper(
  IDialogService dialogService,
  IMediator mediator,
  IIconService iconService) : IMapper<Fence, FenceViewModel>
{
  #region Implementation of IMapper<Fence,FenceViewModel>

  /// <summary>
  ///   Maps a <see cref="Fence" /> domain aggregate to a <see cref="FenceViewModel" /> for presentation
  ///   purposes.
  /// </summary>
  /// <param name="fence">The <see cref="Fence" /> domain aggregate to be mapped.</param>
  /// <returns>A <see cref="FenceViewModel" /> ready for UI binding.</returns>
  /// <remarks>
  ///   This mapping process transforms domain value objects into simple types (e.g., integers, strings,
  ///   doubles)
  ///   expected by the UI. It ensures that the presentation layer remains decoupled from the domain
  ///   structure.
  /// </remarks>
  public FenceViewModel Map(Fence fence)
  {
    return new FenceViewModel(dialogService, mediator)
    {
      Id = fence.Id.Value,
      Name = fence.Name,
      X = fence.Position.X,
      Y = fence.Position.Y,
      Width = fence.Dimensions.Width,
      Height = fence.Dimensions.Height,
      Color = fence.BackgroundColor.ToHexString(),
      Opacity = fence.Opacity.Percentage / 100.0, // Conversion WPF (0-1 au lieu de 0-100)
      IsActive = fence.IsActive,
      CreatedAt = fence.CreatedAt,
      UpdatedAt = fence.UpdatedAt ?? DateTime.UtcNow,
      Items = new ObservableCollection<FenceItemViewModel>(
        fence.Items.OrderBy(i => i.SortOrder)
             .Select(i => new FenceItemViewModel
               {
                 Id = i.Id,
                 DisplayName = i.DisplayName,
                 Path = i.Path,
                 ItemType = (int)i.ItemType,
                 SortOrder = i.SortOrder,
                 Icon = iconService.GetIcon(i.Path)
               }
             )
      )
    };
  }

  /// <summary>
  ///   Maps a collection of <see cref="Fence" /> domain aggregates to a list of
  ///   <see cref="FenceViewModel" /> for presentation purposes.
  /// </summary>
  /// <param name="fences">The enumerable of <see cref="Fence" /> aggregates to be mapped.</param>
  /// <returns>A list of <see cref="FenceViewModel" /> objects ready for UI binding.</returns>
  /// <remarks>
  ///   This method applies the single-object mapping to each element in the collection.
  ///   Useful for transforming query results that return multiple fence instances.
  /// </remarks>
  public List<FenceViewModel> MapList(IEnumerable<Fence> fences)
  {
    return [.. fences.Select(Map)];
  }

  #endregion
}
