using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vemar.EF.Migrations
{
    /// <inheritdoc />
    public partial class ProyectoDescripcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Proyectos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Proyectos");
        }
    }
}
