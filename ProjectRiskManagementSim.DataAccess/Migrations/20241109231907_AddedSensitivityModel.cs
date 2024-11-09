using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRiskManagementSim.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedSensitivityModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SensitivityModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    SensitivityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Days = table.Column<double>(type: "float", nullable: false),
                    EndDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectSimulationModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensitivityModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensitivityModel_ProjectSimulationModel_ProjectSimulationModelId",
                        column: x => x.ProjectSimulationModelId,
                        principalTable: "ProjectSimulationModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensitivityModel_ProjectSimulationModelId",
                table: "SensitivityModel",
                column: "ProjectSimulationModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensitivityModel");
        }
    }
}
