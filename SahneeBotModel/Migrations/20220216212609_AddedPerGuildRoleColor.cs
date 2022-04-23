using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SahneeBotModel.Migrations
{
    public partial class AddedPerGuildRoleColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WarningRoleColor",
                table: "GuildStates",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarningRoleColor",
                table: "GuildStates");
        }
    }
}
