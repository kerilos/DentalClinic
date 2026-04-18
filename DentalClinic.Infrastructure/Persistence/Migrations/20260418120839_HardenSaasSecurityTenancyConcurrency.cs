using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinic.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HardenSaasSecurityTenancyConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Treatments_ToothNumber",
                table: "Treatments");

            migrationBuilder.DropIndex(
                name: "IX_Patients_FullName",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_PhoneNumber",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentDate",
                table: "Appointments");

            migrationBuilder.AddColumn<Guid>(
                name: "ClinicId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Users",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<Guid>(
                name: "ClinicId",
                table: "Treatments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Treatments",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<Guid>(
                name: "ClinicId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Payments",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClinicId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Patients",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<Guid>(
                name: "ClinicId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Invoices",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<Guid>(
                name: "ClinicId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Appointments",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClinicId_Email",
                table: "Users",
                columns: new[] { "ClinicId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_ClinicId_PatientId",
                table: "Treatments",
                columns: new[] { "ClinicId", "PatientId" });

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_ClinicId_ToothNumber",
                table: "Treatments",
                columns: new[] { "ClinicId", "ToothNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceId_RequestId",
                table: "Payments",
                columns: new[] { "InvoiceId", "RequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ClinicId_FullName",
                table: "Patients",
                columns: new[] { "ClinicId", "FullName" });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ClinicId_PhoneNumber",
                table: "Patients",
                columns: new[] { "ClinicId", "PhoneNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ClinicId_PatientId",
                table: "Invoices",
                columns: new[] { "ClinicId", "PatientId" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClinicId_DoctorId_AppointmentDate",
                table: "Appointments",
                columns: new[] { "ClinicId", "DoctorId", "AppointmentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClinicId_PatientId",
                table: "Appointments",
                columns: new[] { "ClinicId", "PatientId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ClinicId_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Treatments_ClinicId_PatientId",
                table: "Treatments");

            migrationBuilder.DropIndex(
                name: "IX_Treatments_ClinicId_ToothNumber",
                table: "Treatments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_InvoiceId_RequestId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ClinicId_FullName",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ClinicId_PhoneNumber",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ClinicId_PatientId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ClinicId_DoctorId_AppointmentDate",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ClinicId_PatientId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Treatments");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Treatments");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_ToothNumber",
                table: "Treatments",
                column: "ToothNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_FullName",
                table: "Patients",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PhoneNumber",
                table: "Patients",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentDate",
                table: "Appointments",
                column: "AppointmentDate");
        }
    }
}
