using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRiskManagementSim.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class BugAndDefectData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockingEventModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlockingEventPercentage = table.Column<double>(type: "float", nullable: false),
                    BlockingEventsPercentageLowBound = table.Column<double>(type: "float", nullable: false),
                    BlockingEventsPercentageHighBound = table.Column<double>(type: "float", nullable: false),
                    ProjectSimulationModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockingEventModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockingEventModel_ProjectSimulationModel_ProjectSimulationModelId",
                        column: x => x.ProjectSimulationModelId,
                        principalTable: "ProjectSimulationModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefectModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefectPercentage = table.Column<double>(type: "float", nullable: false),
                    DefectsPercentageLowBound = table.Column<double>(type: "float", nullable: false),
                    DefectsPercentageHighBound = table.Column<double>(type: "float", nullable: false),
                    ProjectSimulationModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefectModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefectModel_ProjectSimulationModel_ProjectSimulationModelId",
                        column: x => x.ProjectSimulationModelId,
                        principalTable: "ProjectSimulationModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockingEventModel_ProjectSimulationModelId",
                table: "BlockingEventModel",
                column: "ProjectSimulationModelId");

            migrationBuilder.CreateIndex(
                name: "IX_DefectModel_ProjectSimulationModelId",
                table: "DefectModel",
                column: "ProjectSimulationModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockingEventModel");

            migrationBuilder.DropTable(
                name: "DefectModel");
        }
    }
}
