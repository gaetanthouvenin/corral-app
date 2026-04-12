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
      UpdatedAt = fence.UpdatedAt ?? DateTime.UtcNow
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
    var entity =
      await dbContext.Fences.FirstOrDefaultAsync(f => f.Id == id.Value, cancellationToken);

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
    var entities = await dbContext.Fences.ToListAsync(cancellationToken);

    return entities.ConvertAll(e => mapper.Map(e));
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
    var entities = await dbContext.Fences.Where(f => f.IsActive).ToListAsync(cancellationToken);

    return entities.ConvertAll(e => mapper.Map(e));
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
  ///   Updates an existing fence.
  /// </summary>
  /// <param name="fence">The Domain fence to update.</param>
  /// <remarks>
  ///   Maps the Domain Fence to a FenceEntity and marks the EF Core context as modified.
  ///   Saving to the database must be performed via the Unit of Work.
  /// </remarks>
  public void Update(Fence fence)
  {
    var entity = MapFenceToDomainEntity(fence);
    dbContext.Fences.Update(entity);
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
