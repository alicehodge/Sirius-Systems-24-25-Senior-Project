using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorkDork.Migrations.StorkDork
{
    /// <inheritdoc />
    public partial class AddMilestonesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checklist_SDUser",
                table: "Checklist");

            migrationBuilder.DropForeignKey(
                name: "FK_Sighting_SDUser",
                table: "Sighting");

           //migrationBuilder.AddColumn<string>(
            //    name: "FirstName",
             //   table: "SDUser",
             //   type: "nvarchar(max)",
             //   nullable: true);

           // migrationBuilder.AddColumn<string>(
            //    name: "LastName",
             //   table: "SDUser",
             //   type: "nvarchar(max)",
             //   nullable: true);

            migrationBuilder.CreateTable(
                name: "Milestones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SDUserId = table.Column<int>(type: "int", nullable: false),
                    SightingsMade = table.Column<int>(type: "int", nullable: false),
                    PhotosContributed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestones", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Checklist_SDUser_SDUserID",
                table: "Checklist",
                column: "SDUserID",
                principalTable: "SDUser",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sighting_SDUser_SDUserID",
                table: "Sighting",
                column: "SDUserID",
                principalTable: "SDUser",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checklist_SDUser_SDUserID",
                table: "Checklist");

            migrationBuilder.DropForeignKey(
                name: "FK_Sighting_SDUser_SDUserID",
                table: "Sighting");

            migrationBuilder.DropTable(
                name: "Milestones");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "SDUser");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "SDUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Checklist_SDUser",
                table: "Checklist",
                column: "SDUserID",
                principalTable: "SDUser",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sighting_SDUser",
                table: "Sighting",
                column: "SDUserID",
                principalTable: "SDUser",
                principalColumn: "ID");
        }
    }
}
