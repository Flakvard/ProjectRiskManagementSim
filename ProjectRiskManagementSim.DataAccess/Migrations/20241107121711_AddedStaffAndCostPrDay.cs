using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRiskManagementSim.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedStaffAndCostPrDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BackendDevs",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CostPrDay",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FrontendDevs",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Testers",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackendDevs",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "CostPrDay",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "FrontendDevs",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "Testers",
                table: "ProjectSimulationModel");
        }
    }
}
