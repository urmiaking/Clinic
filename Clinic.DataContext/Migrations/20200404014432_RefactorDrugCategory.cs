using Microsoft.EntityFrameworkCore.Migrations;

namespace Clinic.DataContext.Migrations
{
    public partial class RefactorDrugCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Drugs");

            migrationBuilder.AddColumn<int>(
                name: "DrugCategoryId",
                table: "Drugs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DrugCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_DrugCategoryId",
                table: "Drugs",
                column: "DrugCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_DrugCategory_DrugCategoryId",
                table: "Drugs",
                column: "DrugCategoryId",
                principalTable: "DrugCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_DrugCategory_DrugCategoryId",
                table: "Drugs");

            migrationBuilder.DropTable(
                name: "DrugCategory");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_DrugCategoryId",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "DrugCategoryId",
                table: "Drugs");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Drugs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
