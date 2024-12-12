using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAW.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClearsGroupAndGroupPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Posts");
            migrationBuilder.Sql("DELETE FROM UserGroups");
            migrationBuilder.Sql("DELETE FROM Groups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
