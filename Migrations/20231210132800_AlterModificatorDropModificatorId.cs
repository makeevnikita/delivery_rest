using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery.Migrations
{
    /// <inheritdoc />
    public partial class AlterModificatorDropModificatorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Modificators_ModificatorId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ModificatorId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ModificatorId",
                table: "OrderItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModificatorId",
                table: "OrderItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ModificatorId",
                table: "OrderItems",
                column: "ModificatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Modificators_ModificatorId",
                table: "OrderItems",
                column: "ModificatorId",
                principalTable: "Modificators",
                principalColumn: "Id");
        }
    }
}
