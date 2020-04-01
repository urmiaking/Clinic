using Microsoft.EntityFrameworkCore.Migrations;

namespace Clinic.DataContext.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Administrations",
                columns: new[] { "Id", "Email", "Host", "Password", "Port" },
                values: new object[] { 1, "masoud.xpress@gmail.com", "smtp.gmail.com", "MASOUD7559", 587 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "Password", "ResetPasswordCode", "UserType", "Username" },
                values: new object[,]
                {
                    { 2, "manager@manager.com", "مدیر کلینیک", "6ee4a469cd4e91053847f5d3fcb61dbcc91e8f0ef10be7748da4c4a1ba382d17", null, "ClinicManager", "manager" },
                    { 3, "pharmacy@pharmacy.com", "مسئول داروخانه", "20b04a4f018b89f369ce009213afc6369f59fe1c903c0770732ec7402163c358", null, "Pharmacy", "pharmacy" },
                    { 1, "admin@admin.com", "مدیر سایت", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", null, "SiteAdmin", "admin" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Administrations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
