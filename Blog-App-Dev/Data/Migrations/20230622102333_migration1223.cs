using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_App_Dev.Data.Migrations
{
    public partial class migration1223 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserID",
                table: "BlogPosts");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "BlogPosts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "BlogPosts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserID",
                table: "BlogPosts",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
