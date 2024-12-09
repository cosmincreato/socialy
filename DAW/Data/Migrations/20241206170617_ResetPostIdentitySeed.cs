using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAW.Data.Migrations
{
    /// <inheritdoc />
    public partial class ResetPostIdentitySeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DBCC CHECKIDENT ('Posts', RESEED, 0);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
