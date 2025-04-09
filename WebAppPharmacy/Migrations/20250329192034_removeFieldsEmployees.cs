using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppPharmacy.Migrations
{
    /// <inheritdoc />
    public partial class removeFieldsEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
