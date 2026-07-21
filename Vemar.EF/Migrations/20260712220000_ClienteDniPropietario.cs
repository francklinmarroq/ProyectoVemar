using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vemar.EF.Migrations
{
    /// <inheritdoc />
    public partial class ClienteDniPropietario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DniPropietario",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DniPropietario",
                table: "Clientes");
        }
    }
}
