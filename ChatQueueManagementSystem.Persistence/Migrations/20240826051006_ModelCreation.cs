using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatQueueManagementSystem.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class ModelCreation : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
					name: "Overflows",
					columns: table => new
					{
						Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
						CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						IsDeleted = table.Column<bool>(type: "bit", nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_Overflows", x => x.Id);
					});

			migrationBuilder.CreateTable(
					name: "Queues",
					columns: table => new
					{
						Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
						QueueName = table.Column<string>(type: "nvarchar(450)", nullable: false),
						QueueLength = table.Column<int>(type: "int", nullable: false),
						IsOverflow = table.Column<bool>(type: "bit", nullable: false),
						CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						IsDeleted = table.Column<bool>(type: "bit", nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_Queues", x => x.Id);
					});

			migrationBuilder.CreateTable(
					name: "Teams",
					columns: table => new
					{
						Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
						Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
						CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						IsDeleted = table.Column<bool>(type: "bit", nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_Teams", x => x.Id);
					});

			migrationBuilder.CreateTable(
					name: "Users",
					columns: table => new
					{
						Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
						Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
						Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
						PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
						CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						IsDeleted = table.Column<bool>(type: "bit", nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_Users", x => x.Id);
					});

			migrationBuilder.CreateTable(
					name: "ChatSessions",
					columns: table => new
					{
						Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
						QueueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
						UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
						AgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
						StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
						EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
						Status = table.Column<int>(type: "int", nullable: false),
						IsActive = table.Column<bool>(type: "bit", nullable: false),
						Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
						CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						IsDeleted = table.Column<bool>(type: "bit", nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_ChatSessions", x => x.Id);
						table.ForeignKey(
											name: "FK_ChatSessions_Queues_QueueId",
											column: x => x.QueueId,
											principalTable: "Queues",
											principalColumn: "Id");
					});

			migrationBuilder.CreateTable(
					name: "Agents",
					columns: table => new
					{
						Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
						TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
						Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
						SeniorityLevel = table.Column<int>(type: "int", nullable: false),
						MaxConcurrentChats = table.Column<int>(type: "int", nullable: false),
						CurrentConcurrentChats = table.Column<int>(type: "int", nullable: false),
						SeniorityMultiplier = table.Column<double>(type: "float", nullable: false),
						ShiftStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
						ShiftDuration = table.Column<TimeSpan>(type: "time", nullable: false),
						OverflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
						CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
						IsDeleted = table.Column<bool>(type: "bit", nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_Agents", x => x.Id);
						table.ForeignKey(
											name: "FK_Agents_Overflows_OverflowId",
											column: x => x.OverflowId,
											principalTable: "Overflows",
											principalColumn: "Id");
						table.ForeignKey(
											name: "FK_Agents_Teams_TeamId",
											column: x => x.TeamId,
											principalTable: "Teams",
											principalColumn: "Id",
											onDelete: ReferentialAction.Cascade);
					});

			migrationBuilder.CreateIndex(
					name: "IX_Agent_Name",
					table: "Agents",
					column: "Name");

			migrationBuilder.CreateIndex(
					name: "IX_Agent_TeamId",
					table: "Agents",
					column: "TeamId");

			migrationBuilder.CreateIndex(
					name: "IX_Agents_OverflowId",
					table: "Agents",
					column: "OverflowId");

			migrationBuilder.CreateIndex(
					name: "IX_ChatSession_AgentId",
					table: "ChatSessions",
					column: "AgentId");

			migrationBuilder.CreateIndex(
					name: "IX_ChatSession_Status",
					table: "ChatSessions",
					column: "Status");

			migrationBuilder.CreateIndex(
					name: "IX_ChatSession_UserId",
					table: "ChatSessions",
					column: "UserId");

			migrationBuilder.CreateIndex(
					name: "IX_ChatSessions_QueueId",
					table: "ChatSessions",
					column: "QueueId");

			migrationBuilder.CreateIndex(
					name: "IX_Queue_QueueName",
					table: "Queues",
					column: "QueueName");

			migrationBuilder.CreateIndex(
					name: "IX_Team_Name",
					table: "Teams",
					column: "Name");

			migrationBuilder.CreateIndex(
					name: "IX_User_Email",
					table: "Users",
					column: "Email");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
					name: "Agents");

			migrationBuilder.DropTable(
					name: "ChatSessions");

			migrationBuilder.DropTable(
					name: "Users");

			migrationBuilder.DropTable(
					name: "Overflows");

			migrationBuilder.DropTable(
					name: "Teams");

			migrationBuilder.DropTable(
					name: "Queues");
		}
	}
}
