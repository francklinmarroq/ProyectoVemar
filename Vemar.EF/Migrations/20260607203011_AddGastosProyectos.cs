using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vemar.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddGastosProyectos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // La migración GastoProyecto (20260607052738) fue generada vacía por error.
            // Esta migración crea la tabla si no existe todavía.
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GastosProyectos')
                BEGIN
                    CREATE TABLE [GastosProyectos] (
                        [Id]             INT             NOT NULL IDENTITY(1,1),
                        [ProyectoId]     INT             NULL,
                        [Descripcion]    NVARCHAR(MAX)   NULL,
                        [Cantidad]       DECIMAL(18,2)   NOT NULL DEFAULT 0,
                        [CostoUnitario]  DECIMAL(18,2)   NOT NULL DEFAULT 0,
                        [PendienteDePago] BIT            NOT NULL DEFAULT 0,
                        CONSTRAINT [PK_GastosProyectos] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_GastosProyectos_Proyectos_ProyectoId]
                            FOREIGN KEY ([ProyectoId]) REFERENCES [Proyectos]([Id])
                    );
                    CREATE INDEX [IX_GastosProyectos_ProyectoId]
                        ON [GastosProyectos] ([ProyectoId]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "GastosProyectos");
        }
    }
}
