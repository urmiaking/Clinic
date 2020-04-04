using Microsoft.EntityFrameworkCore.Migrations;

namespace Clinic.DataContext.Migrations
{
    public partial class RemoveDrugPharmacyRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_Users_PharmacyId",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_PharmacyId",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "PharmacyId",
                table: "Drugs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PharmacyId",
                table: "Drugs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_PharmacyId",
                table: "Drugs",
                column: "PharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_Users_PharmacyId",
                table: "Drugs",
                column: "PharmacyId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
