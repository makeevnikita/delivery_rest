using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery.Migrations
{
    /// <inheritdoc />
    public partial class AlterOrderItemDropAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "OrderItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "OrderItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
