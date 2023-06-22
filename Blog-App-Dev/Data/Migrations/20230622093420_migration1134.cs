using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_App_Dev.Data.Migrations
{
    public partial class migration1134 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "BlogPosts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_UserID",
                table: "BlogPosts",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserID",
                table: "BlogPosts",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserID",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_UserID",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}
