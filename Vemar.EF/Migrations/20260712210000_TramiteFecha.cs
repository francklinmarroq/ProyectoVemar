using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vemar.EF.Migrations
{
    /// <inheritdoc />
    public partial class TramiteFecha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<System.DateTime>(
                name: "Fecha",
                table: "Tramites",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Tramites");
        }
    }
}
