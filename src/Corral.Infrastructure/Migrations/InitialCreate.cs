// ------------------------------------------------------------------------------------------------
// <copyright file="InitialCreate.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore.Migrations;

namespace Corral.Infrastructure.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
    #region Methods

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        "Fences",
        table => new
        {
          Id = table.Column<string>("TEXT", maxLength: 36, nullable: false),
          Name = table.Column<string>("TEXT", maxLength: 256, nullable: false),
          PositionX = table.Column<int>("INTEGER", nullable: false),
          PositionY = table.Column<int>("INTEGER", nullable: false),
          Width = table.Column<int>("INTEGER", nullable: false),
          Height = table.Column<int>("INTEGER", nullable: false),
          BackgroundColor =
            table.Column<string>("TEXT", maxLength: 8, nullable: false, defaultValue: "FFFFFFFF"),
          Opacity = table.Column<int>("INTEGER", nullable: false, defaultValue: 100),
          IsActive = table.Column<bool>("INTEGER", nullable: false, defaultValue: true),
          CreatedAt = table.Column<DateTime>("TEXT", nullable: false),
          UpdatedAt = table.Column<DateTime>("TEXT", nullable: false)
        },
        constraints: table => { table.PrimaryKey("PK_Fences", x => x.Id); }
      );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable("Fences");
    }

    #endregion
  }
}
