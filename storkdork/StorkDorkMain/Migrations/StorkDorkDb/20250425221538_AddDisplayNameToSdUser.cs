using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorkDork.Migrations.StorkDorkDb
{
    /// <inheritdoc />
    public partial class AddDisplayNameToSdUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checklist_SDUser",
                table: "Checklist");

            migrationBuilder.DropForeignKey(
                name: "FK_Sighting_SDUser",
                table: "Sightings");

            migrationBuilder.RenameTable(
                name: "Checklist",
                newName: "Checklists");

            migrationBuilder.RenameIndex(
                name: "IX_Checklist_SdUserId",
                table: "Checklists",
                newName: "IX_Checklists_SdUserId");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Sightings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoContentType",
                table: "Sightings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "Sightings",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subdivision",
                table: "Sightings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "SDUser",
                type: "nvarchar(max)",
                nullable: true);

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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_SdUserId",
                table: "UserSettings",
                column: "SdUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Checklists_SDUser_SdUserId",
                table: "Checklists",
                column: "SdUserId",
                principalTable: "SDUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sightings_SDUser_SdUserId",
                table: "Sightings",
                column: "SdUserId",
                principalTable: "SDUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checklists_SDUser_SdUserId",
                table: "Checklists");

            migrationBuilder.DropForeignKey(
                name: "FK_Sightings_SDUser_SdUserId",
                table: "Sightings");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Sightings");

            migrationBuilder.DropColumn(
                name: "PhotoContentType",
                table: "Sightings");

            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "Sightings");

            migrationBuilder.DropColumn(
                name: "Subdivision",
                table: "Sightings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "SDUser");

            migrationBuilder.RenameTable(
                name: "Checklists",
                newName: "Checklist");

            migrationBuilder.RenameIndex(
                name: "IX_Checklists_SdUserId",
                table: "Checklist",
                newName: "IX_Checklist_SdUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checklist_SDUser",
                table: "Checklist",
                column: "SdUserId",
                principalTable: "SDUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sighting_SDUser",
                table: "Sightings",
                column: "SdUserId",
                principalTable: "SDUser",
                principalColumn: "Id");
        }
    }
}
