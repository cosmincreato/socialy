using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAW.Data.Migrations
{
    /// <inheritdoc />
    public partial class friendrequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserIdReceiver = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserIdSender = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => new { x.Id, x.UserIdSender, x.UserIdReceiver });
                    table.ForeignKey(
                        name: "FK_FriendRequests_AspNetUsers_UserIdReceiver",
                        column: x => x.UserIdReceiver,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FriendRequests_AspNetUsers_UserIdSender",
                        column: x => x.UserIdSender,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_UserIdReceiver",
                table: "FriendRequests",
                column: "UserIdReceiver");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_UserIdSender",
                table: "FriendRequests",
                column: "UserIdSender");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendRequests");
        }
    }
}
