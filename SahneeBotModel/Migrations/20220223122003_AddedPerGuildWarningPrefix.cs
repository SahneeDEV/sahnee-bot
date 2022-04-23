using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SahneeBotModel.Migrations
{
    public partial class AddedPerGuildWarningPrefix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WarningRolePrefix",
                table: "GuildStates",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarningRolePrefix",
                table: "GuildStates");
        }
    }
}
