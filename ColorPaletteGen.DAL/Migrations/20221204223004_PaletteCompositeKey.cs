using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColorPaletteGen.DAL.Migrations
{
    /// <inheritdoc />
    public partial class PaletteCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Palettes",
                table: "Palettes");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Palettes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Palettes",
                table: "Palettes",
                columns: new[] { "ChatId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Palettes",
                table: "Palettes");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Palettes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Palettes",
                table: "Palettes",
                column: "Id");
        }
    }
}
