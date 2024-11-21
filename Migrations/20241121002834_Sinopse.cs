using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRepository.Migrations
{
    /// <inheritdoc />
    public partial class Sinopse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sinopse",
                table: "Livros",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sinopse",
                table: "Livros");
        }
    }
}
