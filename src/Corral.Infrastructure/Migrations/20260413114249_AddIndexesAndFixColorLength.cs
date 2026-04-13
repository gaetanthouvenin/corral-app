using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndFixColorLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BackgroundColor",
                table: "Fences",
                type: "TEXT",
                maxLength: 9,
                nullable: false,
                defaultValue: "#FFFFFFFF",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 8,
                oldDefaultValue: "FFFFFFFF");

            migrationBuilder.CreateIndex(
                name: "IX_Fences_IsActive",
                table: "Fences",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Fences_Name",
                table: "Fences",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Fences_IsActive",
                table: "Fences");

            migrationBuilder.DropIndex(
                name: "IX_Fences_Name",
                table: "Fences");

            migrationBuilder.AlterColumn<string>(
                name: "BackgroundColor",
                table: "Fences",
                type: "TEXT",
                maxLength: 8,
                nullable: false,
                defaultValue: "FFFFFFFF",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 9,
                oldDefaultValue: "#FFFFFFFF");
        }
    }
}
