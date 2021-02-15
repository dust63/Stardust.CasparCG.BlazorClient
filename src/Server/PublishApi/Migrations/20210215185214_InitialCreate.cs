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
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IssuedUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    ExpiresInSeconds = table.Column<long>(type: "bigint", nullable: true),
                    IdToken = table.Column<string>(type: "text", nullable: true),
                    Scope = table.Column<string>(type: "text", nullable: true),
                    TokenType = table.Column<string>(type: "text", nullable: true),
                    AccessToken = table.Column<string>(type: "text", nullable: true)
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
                    YoutubeAccountId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<string>(type: "text", nullable: true),
                    PrivacyStatus = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    YoutubeVideoId = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    BytesSent = table.Column<long>(type: "bigint", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true),
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
                        onDelete: ReferentialAction.Cascade);
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
