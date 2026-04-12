// ------------------------------------------------------------------------------------------------
// <copyright file="CorralDbContext.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Infrastructure.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Corral.Infrastructure.Persistence;

/// <summary>
///   Represents the Entity Framework Core DbContext for the Corral application.
///   Manages the persistence of EF Core entities. Domain entities are never directly stored in the
///   database.
/// </summary>
public class CorralDbContext(DbContextOptions<CorralDbContext> options) : DbContext(options)
{
  #region Properties

  /// <summary>
  ///   Gets or sets the DbSet for Fence entities in the database.
  /// </summary>
  public DbSet<FenceEntity> Fences { get; set; }

  #endregion

  #region Methods

  /// <summary>
  ///   Configures the EF Core model for the application.
  /// </summary>
  /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
  /// <remarks>
  ///   This method is used to configure the mappings for EF Core entities.
  /// </remarks>
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Configure Fence entity
    modelBuilder.Entity<FenceEntity>(entity =>
    {
      entity.HasKey(f => f.Id);

      entity.Property(f => f.Id)
        .IsRequired()
        .HasMaxLength(36);

      entity.Property(f => f.Name)
        .IsRequired()
        .HasMaxLength(256);

      entity.Property(f => f.BackgroundColor)
        .IsRequired()
        .HasMaxLength(8)
        .HasDefaultValue("FFFFFFFF");

      entity.Property(f => f.Opacity)
        .HasDefaultValue(100);

      entity.Property(f => f.IsActive)
        .HasDefaultValue(true);

      entity.Property(f => f.CreatedAt)
        .IsRequired();

      entity.Property(f => f.UpdatedAt)
        .IsRequired();

      // Set table name
      entity.ToTable("Fences");
    });
  }

  #endregion
}
