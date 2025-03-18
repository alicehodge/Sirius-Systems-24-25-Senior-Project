using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorkDork.Migrations.StorkDorkDb
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Birds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScientificName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CommonName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SpeciesCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Order = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    FamilyCommonName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FamilyScientificName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReportAs = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Range = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bird__3214EC27152FA685", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SDUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetIdentityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SDUser__3214EC27D4DAB424", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Checklist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SdUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Checklis__3214EC2791C5CD06", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checklist_SDUser",
                        column: x => x.SdUserId,
                        principalTable: "SDUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sightings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SdUserId = table.Column<int>(type: "int", nullable: true),
                    BirdId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sighting__3214EC27BEAC21ED", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sighting_Bird",
                        column: x => x.BirdId,
                        principalTable: "Birds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sighting_SDUser",
                        column: x => x.SdUserId,
                        principalTable: "SDUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistId = table.Column<int>(type: "int", nullable: true),
                    BirdId = table.Column<int>(type: "int", nullable: true),
                    Sighted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Checklis__3214EC27391E9E0F", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItem_Bird",
                        column: x => x.BirdId,
                        principalTable: "Birds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChecklistItem_Checklist",
                        column: x => x.ChecklistId,
                        principalTable: "Checklist",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_SdUserId",
                table: "Checklist",
                column: "SdUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_BirdId",
                table: "ChecklistItems",
                column: "BirdId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_ChecklistId",
                table: "ChecklistItems",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_Sightings_BirdId",
                table: "Sightings",
                column: "BirdId");

            migrationBuilder.CreateIndex(
                name: "IX_Sightings_SdUserId",
                table: "Sightings",
                column: "SdUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistItems");

            migrationBuilder.DropTable(
                name: "Sightings");

            migrationBuilder.DropTable(
                name: "Checklist");

            migrationBuilder.DropTable(
                name: "Birds");

            migrationBuilder.DropTable(
                name: "SDUser");
        }
    }
}
