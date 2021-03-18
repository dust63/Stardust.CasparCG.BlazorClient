using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stardust.Flux.ScheduleEngine.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "text", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: true),
                    ParamType = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ScheduleAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CronExpression = table.Column<string>(type: "text", nullable: true),
                    LastExecution = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NextExecution = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastError = table.Column<string>(type: "text", nullable: true),
                    StartRecordJobId = table.Column<string>(type: "text", nullable: true),
                    StopRecordJobId = table.Column<string>(type: "text", nullable: true),
                    IsStarted = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraParams = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
