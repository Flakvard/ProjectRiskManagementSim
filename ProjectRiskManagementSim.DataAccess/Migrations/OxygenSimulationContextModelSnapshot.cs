﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectRiskManagementSim.DataAccess;

#nullable disable

namespace ProjectRiskManagementSim.DataAccess.Migrations
{
    [DbContext(typeof(OxygenSimulationContext))]
    partial class OxygenSimulationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProjectRiskManagementSim.DataAccess.Models.ColumnModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("EstimatedHighBound")
                        .HasColumnType("float");

                    b.Property<double>("EstimatedLowBound")
                        .HasColumnType("float");

                    b.Property<bool>("IsBuffer")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProjectSimulationModelId")
                        .HasColumnType("int");

                    b.Property<int>("WIP")
                        .HasColumnType("int");

                    b.Property<int>("WIPMax")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectSimulationModelId");

                    b.ToTable("ColumnModel");
                });

            modelBuilder.Entity("ProjectRiskManagementSim.DataAccess.Models.ProjectModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("JiraId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JiraProjectId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProjectModel");
                });

            modelBuilder.Entity("ProjectRiskManagementSim.DataAccess.Models.ProjectSimulationModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double?>("Costs")
                        .HasColumnType("float");

                    b.Property<double>("DeliverablesCount")
                        .HasColumnType("float");

                    b.Property<double?>("Hours")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("PercentageHighBound")
                        .HasColumnType("float");

                    b.Property<double>("PercentageLowBound")
                        .HasColumnType("float");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<double?>("Revenue")
                        .HasColumnType("float");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TargetDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectSimulationModel");
                });

            modelBuilder.Entity("ProjectRiskManagementSim.DataAccess.Models.ColumnModel", b =>
                {
                    b.HasOne("ProjectRiskManagementSim.DataAccess.Models.ProjectSimulationModel", "ProjectSimulationModel")
                        .WithMany("Columns")
                        .HasForeignKey("ProjectSimulationModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectSimulationModel");
                });

            modelBuilder.Entity("ProjectRiskManagementSim.DataAccess.Models.ProjectSimulationModel", b =>
                {
                    b.HasOne("ProjectRiskManagementSim.DataAccess.Models.ProjectModel", "Project")
                        .WithMany("ProjectSimulationModels")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ProjectRiskManagementSim.DataAccess.Models.ProjectModel", b =>
                {
                    b.Navigation("ProjectSimulationModels");
                });

            modelBuilder.Entity("ProjectRiskManagementSim.DataAccess.Models.ProjectSimulationModel", b =>
                {
                    b.Navigation("Columns");
                });
#pragma warning restore 612, 618
        }
    }
}
