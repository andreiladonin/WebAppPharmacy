using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAppPharmacy.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSaleDetailColumnQRCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_batches_batch_statuses_StatusId",
                table: "batches");

            migrationBuilder.DropTable(
                name: "batch_statuses");

            migrationBuilder.DropIndex(
                name: "IX_batches_StatusId",
                table: "batches");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "sale_details");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "batches");

            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "sale_details",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "sale_details");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "sale_details",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                table: "batches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "batch_statuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch_statuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_batches_StatusId",
                table: "batches",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_batches_batch_statuses_StatusId",
                table: "batches",
                column: "StatusId",
                principalTable: "batch_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
