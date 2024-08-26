using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatQueueManagementSystem.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModelCreationsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InactivityCounter",
                table: "ChatSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AssignmentIndexLogVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatStatus = table.Column<int>(type: "int", nullable: false),
                    QueueName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentAgentIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentIndexLogVersions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentIndexLogVersion_AgentId_ChatSessionId_CurrentAgentIndex",
                table: "AssignmentIndexLogVersions",
                columns: new[] { "AgentId", "ChatSessionId", "CurrentAgentIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentIndexLogVersions");

            migrationBuilder.DropColumn(
                name: "InactivityCounter",
                table: "ChatSessions");
        }
    }
}
