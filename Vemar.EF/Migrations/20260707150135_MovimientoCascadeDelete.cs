using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vemar.EF.Migrations
{
    /// <inheritdoc />
    public partial class MovimientoCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar FK existente (sin cascade) y recrearla con CASCADE DELETE
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Remedidas_RemedidaId",
                table: "Movimientos");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Remedidas_RemedidaId",
                table: "Movimientos",
                column: "RemedidaId",
                principalTable: "Remedidas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Remedidas_RemedidaId",
                table: "Movimientos");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Remedidas_RemedidaId",
                table: "Movimientos",
                column: "RemedidaId",
                principalTable: "Remedidas",
                principalColumn: "Id");
        }
    }
}
