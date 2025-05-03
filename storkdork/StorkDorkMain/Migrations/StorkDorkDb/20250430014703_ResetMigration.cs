using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorkDork.Migrations.StorkDorkDb
{
    /// <inheritdoc />
    public partial class ResetMigration : Migration
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
                    Order = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FamilyCommonName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FamilyScientificName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReportAs = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Range = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bird__3214EC27152FA685", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Milestone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SDUserID = table.Column<int>(type: "int", nullable: false),
                    SightingsMade = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PhotosContributed = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Milestone__3214EC27", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SDUser",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetIdentityID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_Checklist_SDUser_SDUserID",
                        column: x => x.SDUserID,
                        principalTable: "SDUser",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ModeratedContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false),
                    SubmitterId = table.Column<int>(type: "int", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModeratorId = table.Column<int>(type: "int", nullable: true),
                    ModeratedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModeratorNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirdId = table.Column<int>(type: "int", nullable: true),
                    RangeDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SubmissionNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeratedContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModeratedContent_Bird_BirdId",
                        column: x => x.BirdId,
                        principalTable: "Bird",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModeratedContent_SDUser_ModeratorId",
                        column: x => x.ModeratorId,
                        principalTable: "SDUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModeratedContent_SDUser_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "SDUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RelatedUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_SDUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SDUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                    Notes = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subdivision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PhotoContentType = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SdUserId = table.Column<int>(type: "int", nullable: false),
                    AnonymousSightings = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings_3214EC27", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_SDUser_SdUserId",
                        column: x => x.SdUserId,
                        principalTable: "SDUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_ModeratedContent_BirdId",
                table: "ModeratedContent",
                column: "BirdId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratedContent_ModeratorId",
                table: "ModeratedContent",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratedContent_SubmitterId",
                table: "ModeratedContent",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sighting_BirdID",
                table: "Sighting",
                column: "BirdID");

            migrationBuilder.CreateIndex(
                name: "IX_Sighting_SDUserID",
                table: "Sighting",
                column: "SDUserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_SdUserId",
                table: "UserSettings",
                column: "SdUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistItem");

            migrationBuilder.DropTable(
                name: "Milestone");

            migrationBuilder.DropTable(
                name: "ModeratedContent");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Sighting");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Checklist");

            migrationBuilder.DropTable(
                name: "Bird");

            migrationBuilder.DropTable(
                name: "SDUser");
        }
    }
}
