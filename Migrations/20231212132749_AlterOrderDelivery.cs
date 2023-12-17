using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery.Migrations
{
    /// <inheritdoc />
    public partial class AlterOrderDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDelivery_Orders_OrderId",
                table: "OrderDelivery");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderModificator_Modificators_ModificatorId",
                table: "OrderModificator");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderModificator_OrderItems_OrderItemId",
                table: "OrderModificator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderModificator",
                table: "OrderModificator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDelivery",
                table: "OrderDelivery");

            migrationBuilder.RenameTable(
                name: "OrderModificator",
                newName: "OrderModificators");

            migrationBuilder.RenameTable(
                name: "OrderDelivery",
                newName: "OrderDeliveries");

            migrationBuilder.RenameIndex(
                name: "IX_OrderModificator_OrderItemId",
                table: "OrderModificators",
                newName: "IX_OrderModificators_OrderItemId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderModificator_ModificatorId",
                table: "OrderModificators",
                newName: "IX_OrderModificators_ModificatorId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDelivery_OrderId",
                table: "OrderDeliveries",
                newName: "IX_OrderDeliveries_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderModificators",
                table: "OrderModificators",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDeliveries",
                table: "OrderDeliveries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDeliveries_Orders_OrderId",
                table: "OrderDeliveries",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderModificators_Modificators_ModificatorId",
                table: "OrderModificators",
                column: "ModificatorId",
                principalTable: "Modificators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderModificators_OrderItems_OrderItemId",
                table: "OrderModificators",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDeliveries_Orders_OrderId",
                table: "OrderDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderModificators_Modificators_ModificatorId",
                table: "OrderModificators");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderModificators_OrderItems_OrderItemId",
                table: "OrderModificators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderModificators",
                table: "OrderModificators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDeliveries",
                table: "OrderDeliveries");

            migrationBuilder.RenameTable(
                name: "OrderModificators",
                newName: "OrderModificator");

            migrationBuilder.RenameTable(
                name: "OrderDeliveries",
                newName: "OrderDelivery");

            migrationBuilder.RenameIndex(
                name: "IX_OrderModificators_OrderItemId",
                table: "OrderModificator",
                newName: "IX_OrderModificator_OrderItemId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderModificators_ModificatorId",
                table: "OrderModificator",
                newName: "IX_OrderModificator_ModificatorId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDeliveries_OrderId",
                table: "OrderDelivery",
                newName: "IX_OrderDelivery_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderModificator",
                table: "OrderModificator",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDelivery",
                table: "OrderDelivery",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDelivery_Orders_OrderId",
                table: "OrderDelivery",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderModificator_Modificators_ModificatorId",
                table: "OrderModificator",
                column: "ModificatorId",
                principalTable: "Modificators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderModificator_OrderItems_OrderItemId",
                table: "OrderModificator",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
