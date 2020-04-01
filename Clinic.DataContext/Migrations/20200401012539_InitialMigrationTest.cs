using Microsoft.EntityFrameworkCore.Migrations;

namespace Clinic.DataContext.Migrations
{
    public partial class InitialMigrationTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Reservations_ReservationPatientId_ReservationDoctorId_ReservationReserveDate",
                table: "Visits");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Reservations_ReservationPatientId_ReservationDoctorId_ReservationReserveDate",
                table: "Visits",
                columns: new[] { "ReservationPatientId", "ReservationDoctorId", "ReservationReserveDate" },
                principalTable: "Reservations",
                principalColumns: new[] { "PatientId", "DoctorId", "ReserveDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Reservations_ReservationPatientId_ReservationDoctorId_ReservationReserveDate",
                table: "Visits");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Reservations_ReservationPatientId_ReservationDoctorId_ReservationReserveDate",
                table: "Visits",
                columns: new[] { "ReservationPatientId", "ReservationDoctorId", "ReservationReserveDate" },
                principalTable: "Reservations",
                principalColumns: new[] { "PatientId", "DoctorId", "ReserveDate" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
