using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paschoalotto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTitulosAndParcelas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Titulos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroTitulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomeDevedor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CpfDevedor = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    PercentualJuros = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    PercentualMulta = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titulos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parcelas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TituloId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    DataVencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcelas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parcelas_Titulos_TituloId",
                        column: x => x.TituloId,
                        principalTable: "Titulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parcelas_TituloId",
                table: "Parcelas",
                column: "TituloId");

            migrationBuilder.CreateIndex(
                name: "IX_Titulos_NumeroTitulo",
                table: "Titulos",
                column: "NumeroTitulo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parcelas");

            migrationBuilder.DropTable(
                name: "Titulos");
        }
    }
}
