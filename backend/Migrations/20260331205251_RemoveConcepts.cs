using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveConcepts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageConcept");

            migrationBuilder.DropTable(
                name: "Concepts");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Images",
                newName: "ThumbnailPath");

            migrationBuilder.RenameColumn(
                name: "Hdurl",
                table: "Images",
                newName: "Status");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Images",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "HdPath",
                table: "Images",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HdPath",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "ThumbnailPath",
                table: "Images",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Images",
                newName: "Hdurl");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "Concepts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Concepts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageConcept",
                columns: table => new
                {
                    ConceptsId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageConcept", x => new { x.ConceptsId, x.ImageId });
                    table.ForeignKey(
                        name: "FK_ImageConcept_Concepts_ConceptsId",
                        column: x => x.ConceptsId,
                        principalTable: "Concepts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImageConcept_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Concepts_Name",
                table: "Concepts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageConcept_ImageId",
                table: "ImageConcept",
                column: "ImageId");
        }
    }
}
