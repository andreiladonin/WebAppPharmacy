using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppPharmacy.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldUnitItemms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "unit_items");

            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "unit_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "unit_items");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "unit_items",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
