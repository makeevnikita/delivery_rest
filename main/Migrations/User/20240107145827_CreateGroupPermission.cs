using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace delivery.Migrations.User
{
    /// <inheritdoc />
    public partial class CreateGroupPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPermission_Groups_GroupsId",
                table: "GroupPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupPermission_Permissions_PermissionsId",
                table: "GroupPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupPermission",
                table: "GroupPermission");

            migrationBuilder.RenameColumn(
                name: "PermissionsId",
                table: "GroupPermission",
                newName: "PermissionId");

            migrationBuilder.RenameColumn(
                name: "GroupsId",
                table: "GroupPermission",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupPermission_PermissionsId",
                table: "GroupPermission",
                newName: "IX_GroupPermission_PermissionId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "GroupPermission",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupPermission",
                table: "GroupPermission",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermission_GroupId",
                table: "GroupPermission",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPermission_Groups_GroupId",
                table: "GroupPermission",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPermission_Permissions_PermissionId",
                table: "GroupPermission",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupPermission_Groups_GroupId",
                table: "GroupPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupPermission_Permissions_PermissionId",
                table: "GroupPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupPermission",
                table: "GroupPermission");

            migrationBuilder.DropIndex(
                name: "IX_GroupPermission_GroupId",
                table: "GroupPermission");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GroupPermission");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "GroupPermission",
                newName: "PermissionsId");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "GroupPermission",
                newName: "GroupsId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupPermission_PermissionId",
                table: "GroupPermission",
                newName: "IX_GroupPermission_PermissionsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupPermission",
                table: "GroupPermission",
                columns: new[] { "GroupsId", "PermissionsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPermission_Groups_GroupsId",
                table: "GroupPermission",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupPermission_Permissions_PermissionsId",
                table: "GroupPermission",
                column: "PermissionsId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
