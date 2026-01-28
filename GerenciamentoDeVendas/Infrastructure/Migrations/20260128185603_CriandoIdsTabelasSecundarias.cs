using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriandoIdsTabelasSecundarias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClienteEnderecosSecundarios",
                table: "ClienteEnderecosSecundarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClienteContatos",
                table: "ClienteContatos");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClienteEnderecosSecundarios",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClienteContatos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClienteEnderecosSecundarios",
                table: "ClienteEnderecosSecundarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClienteContatos",
                table: "ClienteContatos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteEnderecosSecundarios_ClienteId",
                table: "ClienteEnderecosSecundarios",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteContatos_ClienteId",
                table: "ClienteContatos",
                column: "ClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClienteEnderecosSecundarios",
                table: "ClienteEnderecosSecundarios");

            migrationBuilder.DropIndex(
                name: "IX_ClienteEnderecosSecundarios_ClienteId",
                table: "ClienteEnderecosSecundarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClienteContatos",
                table: "ClienteContatos");

            migrationBuilder.DropIndex(
                name: "IX_ClienteContatos_ClienteId",
                table: "ClienteContatos");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClienteEnderecosSecundarios",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClienteContatos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClienteEnderecosSecundarios",
                table: "ClienteEnderecosSecundarios",
                columns: new[] { "ClienteId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClienteContatos",
                table: "ClienteContatos",
                columns: new[] { "ClienteId", "Id" });
        }
    }
}
