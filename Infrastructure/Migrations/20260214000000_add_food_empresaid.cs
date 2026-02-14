using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class add_food_empresaid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Foods",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Foods_EmpresaId",
                table: "Foods",
                column: "EmpresaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Foods_EmpresaId",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Foods");
        }
    }
}
