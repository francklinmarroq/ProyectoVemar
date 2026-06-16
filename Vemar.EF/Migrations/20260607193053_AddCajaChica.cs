using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vemar.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddCajaChica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CajasChicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipoOperacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemedidaId = table.Column<int>(type: "int", nullable: true),
                    ProyectoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajasChicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CajasChicas_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CajasChicas_Remedidas_RemedidaId",
                        column: x => x.RemedidaId,
                        principalTable: "Remedidas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CajasChicas_ProyectoId",
                table: "CajasChicas",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_CajasChicas_RemedidaId",
                table: "CajasChicas",
                column: "RemedidaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CajasChicas");
        }
    }
}
