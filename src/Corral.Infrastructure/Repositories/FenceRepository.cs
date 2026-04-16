// ------------------------------------------------------------------------------------------------
// <copyright file="FenceRepository.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Mappers;
using Corral.Domain.Contracts.Repositories;
using Corral.Infrastructure.Persistence;
using Corral.Infrastructure.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Corral.Infrastructure.Repositories;

/// <summary>
///   Provides an implementation of the
///   <see cref="Corral.Domain.Contracts.Repositories.IFenceRepository" /> interface
///   for managing <see cref="Corral.Domain.Aggregates.Fence" /> entities.
/// </summary>
/// <remarks>
///   This repository leverages Entity Framework Core for data persistence using the
///   <see cref="Corral.Infrastructure.Persistence.CorralDbContext" />.
///   It maps EF Core entities (FenceEntity) to domain aggregates (
///   <see cref="Corral.Domain.Aggregates.Fence" />)
///   via a mapping mechanism, ensuring that the domain layer remains decoupled from persistence
///   details.
/// </remarks>
public class FenceRepository(CorralDbContext dbContext, IMapper<FenceEntity, Fence> mapper)
  : IFenceRepository
{
  #region Methods

  /// <summary>
  ///   Maps a domain aggregate <see cref="Corral.Domain.Aggregates.Fence" /> to its corresponding
  ///   persistence entity <see cref="Corral.Infrastructure.Persistence.Entities.FenceEntity" />.
  /// </summary>
  /// <param name="fence">The domain aggregate representing the fence.</param>
  /// <returns>
  ///   An instance of <see cref="Corral.Infrastructure.Persistence.Entities.FenceEntity" />
  ///   that represents the given domain aggregate for persistence.
  /// </returns>
  /// <remarks>
  ///   This method ensures that the domain model remains decoupled from the persistence layer
  ///   by converting the domain aggregate into a format suitable for storage in the database.
  /// </remarks>
  private static FenceEntity MapFenceToDomainEntity(Fence fence)
  {
    return new FenceEntity
    {
      Id = fence.Id.Value,
      Name = fence.Name,
      PositionX = fence.Position.X,
      PositionY = fence.Position.Y,
      Width = fence.Dimensions.Width,
      Height = fence.Dimensions.Height,
      BackgroundColor = fence.BackgroundColor.ToHexString(),
      Opacity = fence.Opacity.Percentage,
      IsActive = fence.IsActive,
      CreatedAt = fence.CreatedAt,
      UpdatedAt = fence.UpdatedAt ?? DateTime.UtcNow,
      Items =
      [
        .. fence.Items.Select(i => new FenceItemEntity
          {
            Id = i.Id,
            FenceId = fence.Id.Value,
            DisplayName = i.DisplayName,
            Path = i.Path,
            ItemType = (int)i.ItemType,
            SortOrder = i.SortOrder,
            CreatedAt = i.CreatedAt
          }
        )
      ]
    };
  }

  #endregion

  #region Implementation of IFenceRepository

  /// <summary>
  ///   Retrieves a fence by its unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the fence.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The fence if found; otherwise, null.</returns>
  /// <remarks>
  ///   Retrieves the FenceEntity from the database and maps it to a Domain Aggregate Root Fence.
  /// </remarks>
  public async Task<Fence> GetByIdAsync(FenceId id, CancellationToken cancellationToken = default)
  {
    var entity = await dbContext.Fences.AsNoTracking()
                                .Include(f => f.Items)
                                .FirstOrDefaultAsync(f => f.Id == id.Value, cancellationToken);

    return entity == null ? null : mapper.Map(entity);
  }

  /// <summary>
  ///   Retrieves all fences.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A list of all fences mapped from the Domain.</returns>
  /// <remarks>
  ///   Retrieves all FenceEntities from the database and maps them to Domain Aggregates.
  /// </remarks>
  public async Task<List<Fence>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var entities = await dbContext.Fences.AsNoTracking()
                                  .Include(f => f.Items)
                                  .ToListAsync(cancellationToken);

    return entities.ConvertAll(mapper.Map);
  }

  /// <summary>
  ///   Retrieves all active fences.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A list of active fences mapped from the Domain.</returns>
  /// <remarks>
  ///   Filters the database to retrieve only active FenceEntities, then maps them.
  /// </remarks>
  public async Task<List<Fence>> GetActivesAsync(CancellationToken cancellationToken = default)
  {
    var entities = await dbContext.Fences.AsNoTracking()
                                  .Include(f => f.Items)
                                  .Where(f => f.IsActive)
                                  .ToListAsync(cancellationToken);

    return entities.ConvertAll(mapper.Map);
  }

  public async Task<List<Fence>> SearchByNameAsync(
    string searchTerm,
    CancellationToken cancellationToken = default)
  {
    var lower = searchTerm.ToLowerInvariant();

    var entities = await dbContext.Fences.AsNoTracking()
                                  .Include(f => f.Items)
                                  .Where(f => f.Name.ToLower().Contains(lower))
                                  .ToListAsync(cancellationToken);

    return entities.ConvertAll(mapper.Map);
  }

  /// <summary>
  ///   Adds a new fence to the repository.
  /// </summary>
  /// <param name="fence">The Domain fence to add.</param>
  /// <remarks>
  ///   Maps the Domain Fence to a FenceEntity and adds it to the EF Core context.
  ///   Saving to the database must be performed via the Unit of Work.
  /// </remarks>
  public void Add(Fence fence)
  {
    var entity = MapFenceToDomainEntity(fence);
    dbContext.Fences.Add(entity);
  }

  /// <summary>
  ///   Updates an existing fence and synchronizes its items collection with the database.
  /// </summary>
  /// <param name="fence">The Domain fence to update.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <remarks>
  ///   Loads the tracked entity (with its Items) from the DbContext, then synchronizes
  ///   scalar properties and reconciles the items collection:
  ///   <list type="bullet">
  ///     <item>items present in both graphs are updated in place;</item>
  ///     <item>items present only in the domain graph are added;</item>
  ///     <item>items present only in the database graph are removed.</item>
  ///   </list>
  ///   This avoids the "UPDATE … WHERE Id = &lt;new guid&gt; affected 0 rows"
  ///   concurrency exception that occurs when calling <c>DbSet.Update</c> on a
  ///   detached graph that contains freshly-created children with non-default keys.
  ///   Saving to the database must be performed via the Unit of Work.
  /// </remarks>
  public async Task UpdateAsync(Fence fence, CancellationToken cancellationToken = default)
  {
    var incoming = MapFenceToDomainEntity(fence);

    var existing =
      await dbContext.Fences.Include(f => f.Items)
                     .FirstOrDefaultAsync(f => f.Id == incoming.Id, cancellationToken)
      ?? throw new InvalidOperationException($"Fence with ID '{incoming.Id}' not found");

    // Sync scalar properties
    existing.Name = incoming.Name;
    existing.PositionX = incoming.PositionX;
    existing.PositionY = incoming.PositionY;
    existing.Width = incoming.Width;
    existing.Height = incoming.Height;
    existing.BackgroundColor = incoming.BackgroundColor;
    existing.Opacity = incoming.Opacity;
    existing.IsActive = incoming.IsActive;
    existing.UpdatedAt = incoming.UpdatedAt;

    // Sync items collection: delete missing, add new, update kept
    var incomingById = incoming.Items.ToDictionary(i => i.Id);
    var existingById = existing.Items.ToDictionary(i => i.Id);

    foreach (var removed in existing.Items.Where(i => !incomingById.ContainsKey(i.Id)).ToList())
    {
      existing.Items.Remove(removed);
    }

    foreach (var incomingItem in incoming.Items)
    {
      if (existingById.TryGetValue(incomingItem.Id, out var existingItem))
      {
        existingItem.DisplayName = incomingItem.DisplayName;
        existingItem.Path = incomingItem.Path;
        existingItem.ItemType = incomingItem.ItemType;
        existingItem.SortOrder = incomingItem.SortOrder;
      }
      else
      {
        existing.Items.Add(incomingItem);
      }
    }
  }

  /// <summary>
  ///   Deletes a fence from the repository.
  /// </summary>
  /// <param name="fence">The Domain fence to delete.</param>
  /// <remarks>
  ///   Maps the Domain Fence to a FenceEntity and marks it for deletion.
  ///   Saving to the database must be performed via the Unit of Work.
  /// </remarks>
  public void Delete(Fence fence)
  {
    var entity = MapFenceToDomainEntity(fence);
    dbContext.Fences.Remove(entity);
  }

  #endregion
}
