using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CW.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuantityFromProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Products",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Products",
                newName: "ImageUrl");
        }
    }
}
