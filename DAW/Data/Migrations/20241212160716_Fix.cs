using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAW.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupPostId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_GroupPostId",
                table: "Comments",
                column: "GroupPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_GroupPostId",
                table: "Comments",
                column: "GroupPostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_GroupPostId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_GroupPostId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "GroupPostId",
                table: "Comments");
        }
    }
}
