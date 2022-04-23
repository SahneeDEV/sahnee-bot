using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SahneeBotModel.Migrations
{
    public partial class Add_Index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Warnings_GuildId",
                table: "Warnings",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Warnings_GuildId_IssuerUserId",
                table: "Warnings",
                columns: new[] { "GuildId", "IssuerUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Warnings_GuildId_IssuerUserId_Time",
                table: "Warnings",
                columns: new[] { "GuildId", "IssuerUserId", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_Warnings_GuildId_Time",
                table: "Warnings",
                columns: new[] { "GuildId", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_Warnings_GuildId_UserId",
                table: "Warnings",
                columns: new[] { "GuildId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Warnings_GuildId_UserId_Time",
                table: "Warnings",
                columns: new[] { "GuildId", "UserId", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_Warnings_UserId",
                table: "Warnings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStates_UserId",
                table: "UserStates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_GuildId",
                table: "Roles",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_GuildId_RoleId",
                table: "Roles",
                columns: new[] { "GuildId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleId",
                table: "Roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildStates_GuildId_BoundChannelId",
                table: "GuildStates",
                columns: new[] { "GuildId", "BoundChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_GuildStates_GuildId_SetRoles",
                table: "GuildStates",
                columns: new[] { "GuildId", "SetRoles" });

            migrationBuilder.CreateIndex(
                name: "IX_GuildStates_GuildId_WarningRoleColor",
                table: "GuildStates",
                columns: new[] { "GuildId", "WarningRoleColor" });

            migrationBuilder.CreateIndex(
                name: "IX_GuildStates_GuildId_WarningRolePrefix",
                table: "GuildStates",
                columns: new[] { "GuildId", "WarningRolePrefix" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warnings_GuildId",
                table: "Warnings");

            migrationBuilder.DropIndex(
                name: "IX_Warnings_GuildId_IssuerUserId",
                table: "Warnings");

            migrationBuilder.DropIndex(
                name: "IX_Warnings_GuildId_IssuerUserId_Time",
                table: "Warnings");

            migrationBuilder.DropIndex(
                name: "IX_Warnings_GuildId_Time",
                table: "Warnings");

            migrationBuilder.DropIndex(
                name: "IX_Warnings_GuildId_UserId",
                table: "Warnings");

            migrationBuilder.DropIndex(
                name: "IX_Warnings_GuildId_UserId_Time",
                table: "Warnings");

            migrationBuilder.DropIndex(
                name: "IX_Warnings_UserId",
                table: "Warnings");

            migrationBuilder.DropIndex(
                name: "IX_UserStates_UserId",
                table: "UserStates");

            migrationBuilder.DropIndex(
                name: "IX_Roles_GuildId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_GuildId_RoleId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_RoleId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_GuildStates_GuildId_BoundChannelId",
                table: "GuildStates");

            migrationBuilder.DropIndex(
                name: "IX_GuildStates_GuildId_SetRoles",
                table: "GuildStates");

            migrationBuilder.DropIndex(
                name: "IX_GuildStates_GuildId_WarningRoleColor",
                table: "GuildStates");

            migrationBuilder.DropIndex(
                name: "IX_GuildStates_GuildId_WarningRolePrefix",
                table: "GuildStates");
        }
    }
}
