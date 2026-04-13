// ------------------------------------------------------------------------------------------------
// <copyright file="AddFenceItems.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore.Migrations;

namespace Corral.Infrastructure.Migrations
{
  /// <inheritdoc />
  public partial class AddFenceItems : Migration
  {
    #region Methods

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        "FenceItems",
        table => new
        {
          Id = table.Column<string>("TEXT", maxLength: 36, nullable: false),
          FenceId = table.Column<string>("TEXT", nullable: true),
          DisplayName = table.Column<string>("TEXT", maxLength: 256, nullable: false),
          Path = table.Column<string>("TEXT", maxLength: 1024, nullable: false),
          ItemType = table.Column<int>("INTEGER", nullable: false),
          SortOrder = table.Column<int>("INTEGER", nullable: false, defaultValue: 0),
          CreatedAt = table.Column<DateTime>("TEXT", nullable: false)
        },
        constraints: table =>
                     {
                       table.PrimaryKey("PK_FenceItems", x => x.Id);
                       table.ForeignKey(
                         "FK_FenceItems_Fences_FenceId",
                         x => x.FenceId,
                         "Fences",
                         "Id",
                         onDelete: ReferentialAction.Cascade
                       );
                     }
      );

      migrationBuilder.CreateIndex("IX_FenceItems_FenceId", "FenceItems", "FenceId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable("FenceItems");
    }

    #endregion
  }
}
