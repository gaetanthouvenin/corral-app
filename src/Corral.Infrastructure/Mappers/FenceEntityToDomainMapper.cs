// ------------------------------------------------------------------------------------------------
// <copyright file="FenceEntityToDomainMapper.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Mappers;
using Corral.Domain.ValueObjects;
using Corral.Infrastructure.Persistence.Entities;

namespace Corral.Infrastructure.Mappers;

/// <summary>
///   Explicit mapper to transform FenceEntity (persistence model) into Fence (domain aggregate).
///   Ensures clean separation between Infrastructure and Domain layers.
/// </summary>
/// <remarks>
///   This mapper is used when loading fence data from the database. It reconstructs the domain
///   aggregate from the persisted entity using the Fence.Reconstitute() factory method.
/// </remarks>
public class FenceEntityToDomainMapper : IMapper<FenceEntity, Fence>
{
  #region Implementation of IMapper<FenceEntity,Fence>

  /// <summary>
  ///   Maps a FenceEntity from the database to a Fence domain aggregate.
  /// </summary>
  /// <param name="entity">The FenceEntity from the persistence layer.</param>
  /// <returns>A Fence domain aggregate reconstructed from the entity data.</returns>
  /// <remarks>
  ///   This transformation rebuilds the domain aggregate from the flat persistence model,
  ///   reconstructing Value Objects (Position, Dimensions, Color, Opacity) from entity properties.
  ///   Uses Fence.Reconstitute() to properly initialize the aggregate without triggering domain events.
  /// </remarks>
  public Fence Map(FenceEntity entity)
  {
    var fenceId = FenceId.Create(entity.Id);
    var position = new Position(entity.PositionX, entity.PositionY);
    var dimensions = new Dimensions(entity.Width, entity.Height);
    var backgroundColor = Color.FromHexString(entity.BackgroundColor);
    var opacity = new Opacity(entity.Opacity);

    var fence = Fence.Reconstitute(
      fenceId,
      entity.Name,
      position,
      dimensions,
      backgroundColor,
      opacity,
      entity.IsActive,
      entity.CreatedAt,
      entity.UpdatedAt
    );

    // Load items if present
    if (entity.Items is { Count: > 0 })
    {
      var items = entity.Items.Select(i => FenceItem.Reconstitute(
                                        i.Id,
                                        i.DisplayName,
                                        i.Path,
                                        (FenceItemType)i.ItemType,
                                        i.SortOrder,
                                        i.CreatedAt
                                      )
      );

      fence.LoadItems(items);
    }

    return fence;
  }

  /// <summary>
  ///   Maps a collection of FenceEntity objects to a list of Fence domain aggregates.
  /// </summary>
  /// <param name="entities">The enumerable of FenceEntity objects from persistence.</param>
  /// <returns>A list of Fence aggregates reconstructed from entity data.</returns>
  /// <remarks>
  ///   This method applies the single-entity mapping to each element in the collection.
  ///   Useful for transforming query results that return multiple fence instances from the database.
  /// </remarks>
  public List<Fence> MapList(IEnumerable<FenceEntity> entities)
  {
    return [.. entities.Select(Map)];
  }

  #endregion
}
