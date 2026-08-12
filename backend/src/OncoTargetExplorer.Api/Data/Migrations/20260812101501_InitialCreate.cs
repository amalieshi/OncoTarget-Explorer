using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OncoTargetExplorer.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortlistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Accession = table.Column<string>(type: "TEXT", nullable: false),
                    GeneName = table.Column<string>(type: "TEXT", nullable: false),
                    ProteinName = table.Column<string>(type: "TEXT", nullable: false),
                    AddedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortlistItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShortlistItems_Accession",
                table: "ShortlistItems",
                column: "Accession",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortlistItems");
        }
    }
}
