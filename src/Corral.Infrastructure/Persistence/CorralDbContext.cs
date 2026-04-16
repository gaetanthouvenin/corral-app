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

  /// <summary>
  ///   Gets or sets the DbSet for FenceItem entities in the database.
  /// </summary>
  public DbSet<FenceItemEntity> FenceItems { get; set; }

  /// <summary>
  ///   Gets or sets the DbSet for the singleton UserSettings row.
  /// </summary>
  public DbSet<UserSettingsEntity> UserSettings { get; set; }

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

                                       entity.Property(f => f.Id).IsRequired().HasMaxLength(36);

                                       entity.Property(f => f.Name).IsRequired().HasMaxLength(256);

                                       entity.Property(f => f.BackgroundColor)
                                             .IsRequired()
                                             .HasMaxLength(9)
                                             .HasDefaultValue("#FFFFFFFF");

                                       entity.Property(f => f.Opacity).HasDefaultValue(100);

                                       entity.Property(f => f.IsActive).HasDefaultValue(true);

                                       entity.Property(f => f.CreatedAt).IsRequired();

                                       entity.Property(f => f.UpdatedAt).IsRequired();

                                       entity.HasIndex(f => f.Name)
                                             .HasDatabaseName("IX_Fences_Name");

                                       entity.HasIndex(f => f.IsActive)
                                             .HasDatabaseName("IX_Fences_IsActive");

                                       // Set table name
                                       entity.ToTable("Fences");
                                     }
    );

    // Configure UserSettings entity (singleton row)
    modelBuilder.Entity<UserSettingsEntity>(entity =>
                                            {
                                              entity.HasKey(u => u.Id);
                                              entity.Property(u => u.Id).ValueGeneratedNever();
                                              entity.Property(u => u.ClickMode).HasDefaultValue(0);
                                              entity.Property(u => u.IconLayout).HasDefaultValue(0);
                                              entity.Property(u => u.UpdatedAt).IsRequired();
                                              entity.ToTable("UserSettings");
                                            }
    );

    // Configure FenceItem entity
    modelBuilder.Entity<FenceItemEntity>(entity =>
                                         {
                                           entity.HasKey(fi => fi.Id);

                                           entity.Property(fi => fi.Id)
                                                 .IsRequired()
                                                 .HasMaxLength(36);

                                           entity.Property(fi => fi.DisplayName)
                                                 .IsRequired()
                                                 .HasMaxLength(256);

                                           entity.Property(fi => fi.Path)
                                                 .IsRequired()
                                                 .HasMaxLength(1024);

                                           entity.Property(fi => fi.ItemType).IsRequired();

                                           entity.Property(fi => fi.SortOrder).HasDefaultValue(0);

                                           entity.Property(fi => fi.CreatedAt).IsRequired();

                                           entity.HasOne(fi => fi.Fence)
                                                 .WithMany(f => f.Items)
                                                 .HasForeignKey(fi => fi.FenceId)
                                                 .OnDelete(DeleteBehavior.Cascade);

                                           entity.HasIndex(fi => fi.FenceId)
                                                 .HasDatabaseName("IX_FenceItems_FenceId");

                                           entity.ToTable("FenceItems");
                                         }
    );
  }

  #endregion
}
