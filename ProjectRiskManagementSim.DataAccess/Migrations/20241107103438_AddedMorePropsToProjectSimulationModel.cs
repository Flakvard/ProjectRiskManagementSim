using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectRiskManagementSim.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedMorePropsToProjectSimulationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Revenue",
                table: "ProjectSimulationModel",
                newName: "SimulationDays");

            migrationBuilder.RenameColumn(
                name: "Hours",
                table: "ProjectSimulationModel",
                newName: "SimulationCosts");

            migrationBuilder.RenameColumn(
                name: "Costs",
                table: "ProjectSimulationModel",
                newName: "BudgetCosts");

            migrationBuilder.AddColumn<double>(
                name: "ActualCosts",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ActualDays",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ActualHours",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ActualRevenue",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FeaturesCount",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SimEndDate",
                table: "ProjectSimulationModel",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetDays",
                table: "ProjectSimulationModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualCosts",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "ActualDays",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "ActualHours",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "ActualRevenue",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "FeaturesCount",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "SimEndDate",
                table: "ProjectSimulationModel");

            migrationBuilder.DropColumn(
                name: "TargetDays",
                table: "ProjectSimulationModel");

            migrationBuilder.RenameColumn(
                name: "SimulationDays",
                table: "ProjectSimulationModel",
                newName: "Revenue");

            migrationBuilder.RenameColumn(
                name: "SimulationCosts",
                table: "ProjectSimulationModel",
                newName: "Hours");

            migrationBuilder.RenameColumn(
                name: "BudgetCosts",
                table: "ProjectSimulationModel",
                newName: "Costs");
        }
    }
}
