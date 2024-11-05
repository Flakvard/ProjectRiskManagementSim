using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRiskManagementSim.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JiraId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JiraProjectId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSimulationModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Revenue = table.Column<double>(type: "float", nullable: true),
                    Costs = table.Column<double>(type: "float", nullable: true),
                    DeliverablesCount = table.Column<double>(type: "float", nullable: false),
                    PercentageLowBound = table.Column<double>(type: "float", nullable: false),
                    PercentageHighBound = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSimulationModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSimulationModel_ProjectModel_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ProjectModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColumnModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedLowBound = table.Column<double>(type: "float", nullable: false),
                    EstimatedHighBound = table.Column<double>(type: "float", nullable: false),
                    WIP = table.Column<int>(type: "int", nullable: false),
                    WIPMax = table.Column<int>(type: "int", nullable: false),
                    IsBuffer = table.Column<bool>(type: "bit", nullable: false),
                    ProjectSimulationModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColumnModel_ProjectSimulationModel_ProjectSimulationModelId",
                        column: x => x.ProjectSimulationModelId,
                        principalTable: "ProjectSimulationModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColumnModel_ProjectSimulationModelId",
                table: "ColumnModel",
                column: "ProjectSimulationModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSimulationModel_ProjectId",
                table: "ProjectSimulationModel",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColumnModel");

            migrationBuilder.DropTable(
                name: "ProjectSimulationModel");

            migrationBuilder.DropTable(
                name: "ProjectModel");
        }
    }
}
