using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorkDork.Migrations.StorkDork
{
    /// <inheritdoc />
    public partial class AddMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bird",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK__Bird__3214EC27B23D066D", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SDUser",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetIdentityID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SDUser__3214EC277D9B2DC9", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Checklist",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SDUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Checklis__3214EC271BFFB94F", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Checklist_SDUser",
                        column: x => x.SDUserID,
                        principalTable: "SDUser",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Sighting",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SDUserID = table.Column<int>(type: "int", nullable: true),
                    BirdID = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(8,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sighting__3214EC2734C618BE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Sighting_Bird",
                        column: x => x.BirdID,
                        principalTable: "Bird",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Sighting_SDUser",
                        column: x => x.SDUserID,
                        principalTable: "SDUser",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItem",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistID = table.Column<int>(type: "int", nullable: true),
                    BirdID = table.Column<int>(type: "int", nullable: true),
                    Sighted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Checklis__3214EC27AEE131D7", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChecklistItem_Bird",
                        column: x => x.BirdID,
                        principalTable: "Bird",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChecklistItem_Checklist",
                        column: x => x.ChecklistID,
                        principalTable: "Checklist",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_SDUserID",
                table: "Checklist",
                column: "SDUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItem_BirdID",
                table: "ChecklistItem",
                column: "BirdID");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItem_ChecklistID",
                table: "ChecklistItem",
                column: "ChecklistID");

            migrationBuilder.CreateIndex(
                name: "IX_Sighting_BirdID",
                table: "Sighting",
                column: "BirdID");

            migrationBuilder.CreateIndex(
                name: "IX_Sighting_SDUserID",
                table: "Sighting",
                column: "SDUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistItem");

            migrationBuilder.DropTable(
                name: "Sighting");

            migrationBuilder.DropTable(
                name: "Checklist");

            migrationBuilder.DropTable(
                name: "Bird");

            migrationBuilder.DropTable(
                name: "SDUser");
        }
    }
}
