using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace GadoEmDia.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Aplicadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Funcao = table.Column<int>(type: "int", nullable: false),
                    Crmv = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    Telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aplicadores", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Bovinos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Identificacao = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Sexo = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<int>(type: "int", nullable: true),
                    Raca = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "date", nullable: true),
                    PesoKg = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    Situacao = table.Column<int>(type: "int", nullable: false),
                    Observacoes = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bovinos", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Finalidade = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    Fabricante = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true),
                    Unidade = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    IntervaloReforcoDias = table.Column<int>(type: "int", nullable: false),
                    CarenciaDias = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Aplicacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<DateTime>(type: "date", nullable: false),
                    BovinoId = table.Column<int>(type: "int", nullable: false),
                    AplicadorId = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aplicacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aplicacoes_Aplicadores_AplicadorId",
                        column: x => x.AplicadorId,
                        principalTable: "Aplicadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Aplicacoes_Bovinos_BovinoId",
                        column: x => x.BovinoId,
                        principalTable: "Bovinos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AplicacaoProduto",
                columns: table => new
                {
                    AplicacaoId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Dose = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AplicacaoProduto", x => new { x.AplicacaoId, x.ProdutoId });
                    table.ForeignKey(
                        name: "FK_AplicacaoProduto_Aplicacoes_AplicacaoId",
                        column: x => x.AplicacaoId,
                        principalTable: "Aplicacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AplicacaoProduto_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AplicacaoProduto_ProdutoId",
                table: "AplicacaoProduto",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Aplicacoes_AplicadorId",
                table: "Aplicacoes",
                column: "AplicadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Aplicacoes_BovinoId",
                table: "Aplicacoes",
                column: "BovinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Bovinos_Identificacao",
                table: "Bovinos",
                column: "Identificacao",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AplicacaoProduto");

            migrationBuilder.DropTable(
                name: "Aplicacoes");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Aplicadores");

            migrationBuilder.DropTable(
                name: "Bovinos");
        }
    }
}
