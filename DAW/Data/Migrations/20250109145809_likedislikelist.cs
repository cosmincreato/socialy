using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAW.Data.Migrations
{
    /// <inheritdoc />
    public partial class likedislikelist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DislikedBy",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LikedBy",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DislikedBy",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LikedBy",
                table: "Posts");
        }
    }
}
