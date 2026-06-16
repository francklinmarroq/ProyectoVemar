using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vemar.EF.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Remedidas_Clientes_ClienteId",
                table: "Remedidas");

            migrationBuilder.DropIndex(
                name: "IX_Remedidas_ClienteId",
                table: "Remedidas");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Remedidas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Remedidas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Remedidas_ClienteId",
                table: "Remedidas",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Remedidas_Clientes_ClienteId",
                table: "Remedidas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
