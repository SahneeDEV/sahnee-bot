using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SahneeBotModel.Migrations
{
    public partial class ExtendedGuildForRolesSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SetRoles",
                table: "GuildStates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetRoles",
                table: "GuildStates");
        }
    }
}
