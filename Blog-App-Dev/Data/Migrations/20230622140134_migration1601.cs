using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_App_Dev.Data.Migrations
{
    public partial class migration1601 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentPosts_BlogPosts_PostID",
                table: "CommentPosts");

            migrationBuilder.AlterColumn<int>(
                name: "PostID",
                table: "CommentPosts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentPosts_BlogPosts_PostID",
                table: "CommentPosts",
                column: "PostID",
                principalTable: "BlogPosts",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentPosts_BlogPosts_PostID",
                table: "CommentPosts");

            migrationBuilder.AlterColumn<int>(
                name: "PostID",
                table: "CommentPosts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentPosts_BlogPosts_PostID",
                table: "CommentPosts",
                column: "PostID",
                principalTable: "BlogPosts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
