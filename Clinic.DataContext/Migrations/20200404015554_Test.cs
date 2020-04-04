using Microsoft.EntityFrameworkCore.Migrations;

namespace Clinic.DataContext.Migrations
{
    public partial class Test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_DrugCategory_DrugCategoryId",
                table: "Drugs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DrugCategory",
                table: "DrugCategory");

            migrationBuilder.RenameTable(
                name: "DrugCategory",
                newName: "DrugCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DrugCategories",
                table: "DrugCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_DrugCategories_DrugCategoryId",
                table: "Drugs",
                column: "DrugCategoryId",
                principalTable: "DrugCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_DrugCategories_DrugCategoryId",
                table: "Drugs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DrugCategories",
                table: "DrugCategories");

            migrationBuilder.RenameTable(
                name: "DrugCategories",
                newName: "DrugCategory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DrugCategory",
                table: "DrugCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_DrugCategory_DrugCategoryId",
                table: "Drugs",
                column: "DrugCategoryId",
                principalTable: "DrugCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
