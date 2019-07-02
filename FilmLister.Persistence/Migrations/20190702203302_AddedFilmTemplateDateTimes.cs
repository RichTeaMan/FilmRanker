using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FilmLister.Persistence.Migrations
{
    public partial class AddedFilmTemplateDateTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDateTime",
                table: "FilmListTemplates",
                nullable: false,
                defaultValue: DateTimeOffset.UtcNow);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PublishedDateTime",
                table: "FilmListTemplates",
                nullable: true,
                defaultValue: DateTimeOffset.UtcNow);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "FilmListTemplates");

            migrationBuilder.DropColumn(
                name: "PublishedDateTime",
                table: "FilmListTemplates");
        }
    }
}
