using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class User_Add_HasConfirmedEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasConfirmedEmail",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasConfirmedEmail",
                table: "Users");
        }
    }
}
