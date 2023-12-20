using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery.Migrations
{
    /// <inheritdoc />
    public partial class CreateOrderConstraintsAndProductConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "varchar(100)",
                oldType: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "numeric(6, 2)",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Orders",
                type: "varchar(400)",
                oldType: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTimeCreation",
                table: "Orders",
                type: "timestamp",
                oldType: "timestamptz",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "Orders",
                type: "timestamp",
                oldType: "timestamptz",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Modificators",
                type: "numeric(6,2)",
                oldType: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "numeric(6, 2)",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DateTimeCreation",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Modificators");
        }
    }
}
