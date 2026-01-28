using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterandoCamposCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContatoCelular",
                table: "Clientes",
                type: "TEXT",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContatoEmail",
                table: "Clientes",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContatoTelefone",
                table: "Clientes",
                type: "TEXT",
                maxLength: 11,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClienteContatos",
                columns: table => new
                {
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    Celular = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteContatos", x => new { x.ClienteId, x.Id });
                    table.ForeignKey(
                        name: "FK_ClienteContatos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClienteEnderecosSecundarios",
                columns: table => new
                {
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CEP = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Logradouro = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Complemento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UF = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteEnderecosSecundarios", x => new { x.ClienteId, x.Id });
                    table.ForeignKey(
                        name: "FK_ClienteEnderecosSecundarios_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClienteContatos");

            migrationBuilder.DropTable(
                name: "ClienteEnderecosSecundarios");

            migrationBuilder.DropColumn(
                name: "ContatoCelular",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ContatoEmail",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ContatoTelefone",
                table: "Clientes");
        }
    }
}
