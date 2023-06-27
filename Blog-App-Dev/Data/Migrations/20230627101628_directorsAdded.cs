using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_App_Dev.Data.Migrations
{
    public partial class directorsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Director_Movies_MovieID",
                table: "Director");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Movies_MovieID",
                table: "Persons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Persons",
                table: "Persons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Director",
                table: "Director");

            migrationBuilder.RenameTable(
                name: "Persons",
                newName: "Casts");

            migrationBuilder.RenameTable(
                name: "Director",
                newName: "Directors");

            migrationBuilder.RenameIndex(
                name: "IX_Persons_MovieID",
                table: "Casts",
                newName: "IX_Casts_MovieID");

            migrationBuilder.RenameIndex(
                name: "IX_Director_MovieID",
                table: "Directors",
                newName: "IX_Directors_MovieID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Casts",
                table: "Casts",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Directors",
                table: "Directors",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Casts_Movies_MovieID",
                table: "Casts",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Directors_Movies_MovieID",
                table: "Directors",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Casts_Movies_MovieID",
                table: "Casts");

            migrationBuilder.DropForeignKey(
                name: "FK_Directors_Movies_MovieID",
                table: "Directors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Directors",
                table: "Directors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Casts",
                table: "Casts");

            migrationBuilder.RenameTable(
                name: "Directors",
                newName: "Director");

            migrationBuilder.RenameTable(
                name: "Casts",
                newName: "Persons");

            migrationBuilder.RenameIndex(
                name: "IX_Directors_MovieID",
                table: "Director",
                newName: "IX_Director_MovieID");

            migrationBuilder.RenameIndex(
                name: "IX_Casts_MovieID",
                table: "Persons",
                newName: "IX_Persons_MovieID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Director",
                table: "Director",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Persons",
                table: "Persons",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Director_Movies_MovieID",
                table: "Director",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Movies_MovieID",
                table: "Persons",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID");
        }
    }
}
