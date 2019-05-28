using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FilmLister.Persistence.Migrations
{
    public partial class AddedReleaseDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReleaseDate",
                table: "Films",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Films");
        }
    }
}
