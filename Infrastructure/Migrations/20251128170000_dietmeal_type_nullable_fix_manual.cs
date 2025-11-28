using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class dietmeal_type_nullable_fix_manual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Garante que a coluna Type da tabela DietMeals aceita NULL
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "DietMeals",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Volta a coluna Type para NOT NULL com default 0
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "DietMeals",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
