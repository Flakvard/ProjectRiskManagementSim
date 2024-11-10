using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRiskManagementSim.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedBugCountToModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeaturesCount",
                table: "ProjectSimulationModel",
                newName: "IssueDoneCount");

            migrationBuilder.AddColumn<double>(
                name: "BugCount",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BugPercentage",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "IssueCount",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BugCount",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "BugPercentage",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "IssueCount",
                table: "ProjectSimulationModel");

            migrationBuilder.RenameColumn(
                name: "IssueDoneCount",
                table: "ProjectSimulationModel",
                newName: "FeaturesCount");
        }
    }
}
