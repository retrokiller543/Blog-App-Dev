using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_App_Dev.Data.Migrations
{
    public partial class migration1508 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "guid",
                table: "BlogPosts");

            migrationBuilder.CreateTable(
                name: "CommentPosts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatePosted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PostID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentPosts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CommentPosts_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentPosts_BlogPosts_PostID",
                        column: x => x.PostID,
                        principalTable: "BlogPosts",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentPosts_PostID",
                table: "CommentPosts",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_CommentPosts_UserID",
                table: "CommentPosts",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentPosts");

            migrationBuilder.AddColumn<Guid>(
                name: "guid",
                table: "BlogPosts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
