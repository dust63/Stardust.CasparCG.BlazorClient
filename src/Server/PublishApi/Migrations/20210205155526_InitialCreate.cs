using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stardust.Flux.PublishApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YoutubeAccounts",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YoutubeAccounts", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "YoutubeUploads",
                columns: table => new
                {
                    YoutubeUploadId = table.Column<string>(type: "text", nullable: false),
                    YoutubeAccountId = table.Column<string>(type: "text", nullable: true),
                    VideoId = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    BytesSent = table.Column<long>(type: "bigint", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YoutubeUploads", x => x.YoutubeUploadId);
                    table.ForeignKey(
                        name: "FK_YoutubeUploads_YoutubeAccounts_YoutubeAccountId",
                        column: x => x.YoutubeAccountId,
                        principalTable: "YoutubeAccounts",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeUploads_YoutubeAccountId",
                table: "YoutubeUploads",
                column: "YoutubeAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YoutubeUploads");

            migrationBuilder.DropTable(
                name: "YoutubeAccounts");
        }
    }
}
