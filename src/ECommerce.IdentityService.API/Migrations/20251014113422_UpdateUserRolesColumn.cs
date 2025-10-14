using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.IdentityService.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRolesColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_UserId1",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "IX_user_roles_UserId1",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "user_roles");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "roles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_UserId",
                table: "roles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_roles_users_UserId",
                table: "roles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_roles_users_UserId",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "IX_roles_UserId",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "roles");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "user_roles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_UserId1",
                table: "user_roles",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_UserId1",
                table: "user_roles",
                column: "UserId1",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
