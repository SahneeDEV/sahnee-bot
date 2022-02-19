using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SahneeBotModel.Migrations
{
    public partial class BindToChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoundChannel",
                table: "GuildStates");

            migrationBuilder.AddColumn<decimal>(
                name: "BoundChannelId",
                table: "GuildStates",
                type: "numeric(20,0)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoundChannelId",
                table: "GuildStates");

            migrationBuilder.AddColumn<string>(
                name: "BoundChannel",
                table: "GuildStates",
                type: "text",
                nullable: true);
        }
    }
}
