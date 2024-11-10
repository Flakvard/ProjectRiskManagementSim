using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRiskManagementSim.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedDaysOfDelayToModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SimulationDaysOfDelay",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DaysDelay",
                table: "ForecastModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SimulationDaysOfDelay",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "DaysDelay",
                table: "ForecastModel");
        }
    }
}
