using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorkDork.Migrations.StorkDorkDb
{
    /// <inheritdoc />
    public partial class AddBirdPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "ModeratedContent",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoContentType",
                table: "ModeratedContent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "ModeratedContent",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BirdPhoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BirdId = table.Column<int>(type: "int", nullable: false),
                    PhotoData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PhotoContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirdPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirdPhoto_Bird_BirdId",
                        column: x => x.BirdId,
                        principalTable: "Bird",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BirdPhoto_BirdId",
                table: "BirdPhoto",
                column: "BirdId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BirdPhoto");

            migrationBuilder.DropColumn(
                name: "Caption",
                table: "ModeratedContent");

            migrationBuilder.DropColumn(
                name: "PhotoContentType",
                table: "ModeratedContent");

            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "ModeratedContent");
        }
    }
}
