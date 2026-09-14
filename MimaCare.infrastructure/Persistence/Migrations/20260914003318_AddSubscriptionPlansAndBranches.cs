using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MimaCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionPlansAndBranches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlanId",
                table: "Clinics",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBranchId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    MaxBranches = table.Column<int>(type: "integer", nullable: true),
                    MaxDoctors = table.Column<int>(type: "integer", nullable: true),
                    AllowsUnlimitedAppointments = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoctorBranches",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorBranches", x => new { x.DoctorId, x.BranchId });
                    table.ForeignKey(
                        name: "FK_DoctorBranches_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorBranches_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "Id", "AllowsUnlimitedAppointments", "CreatedAt", "Description", "IsActive", "MaxBranches", "MaxDoctors", "Name" },
                values: new object[,]
                {
                    { new Guid("c3a00000-0000-4000-8000-000000000001"), true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1 sucursal y hasta 3 doctores, citas ilimitadas.", true, 1, 3, "Básico" },
                    { new Guid("c3a00000-0000-4000-8000-000000000002"), true, new DateTime(2025, 1, 1, 0, 0, 1, 0, DateTimeKind.Utc), "Hasta 3 sucursales y doctores ilimitados, citas ilimitadas.", true, 3, null, "Profesional" },
                    { new Guid("c3a00000-0000-4000-8000-000000000003"), true, new DateTime(2025, 1, 1, 0, 0, 2, 0, DateTimeKind.Utc), "Sucursales y doctores ilimitados, citas ilimitadas, soporte custom.", true, null, null, "Enterprise" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_PlanId",
                table: "Clinics",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BranchId",
                table: "Appointments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CreatedBranchId",
                table: "Appointments",
                column: "CreatedBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_ClinicId",
                table: "Branches",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorBranches_BranchId",
                table: "DoctorBranches",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Branches_BranchId",
                table: "Appointments",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Branches_CreatedBranchId",
                table: "Appointments",
                column: "CreatedBranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clinics_SubscriptionPlans_PlanId",
                table: "Clinics",
                column: "PlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Branches_BranchId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Branches_CreatedBranchId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Clinics_SubscriptionPlans_PlanId",
                table: "Clinics");

            migrationBuilder.DropTable(
                name: "DoctorBranches");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Clinics_PlanId",
                table: "Clinics");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_BranchId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_CreatedBranchId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CreatedBranchId",
                table: "Appointments");
        }
    }
}
