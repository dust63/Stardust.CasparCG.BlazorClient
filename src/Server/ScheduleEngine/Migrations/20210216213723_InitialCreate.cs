using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stardust.Flux.ScheduleEngine.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecordJobs",
                columns: table => new
                {
                    RecordJobId = table.Column<string>(type: "text", nullable: false),
                    RecordType = table.Column<string>(type: "text", nullable: true),
                    Filename = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ScheduleAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CronExpression = table.Column<string>(type: "text", nullable: true),
                    LastExecution = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NextExecution = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastError = table.Column<string>(type: "text", nullable: true),
                    RecordSlotId = table.Column<int>(type: "integer", nullable: false),
                    StartRecordJobId = table.Column<string>(type: "text", nullable: true),
                    StopRecordJobId = table.Column<string>(type: "text", nullable: true),
                    IsRecording = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordJobs", x => x.RecordJobId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordJobs");
        }
    }
}
