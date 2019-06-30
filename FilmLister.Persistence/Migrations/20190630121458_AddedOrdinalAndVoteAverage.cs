using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FilmLister.Persistence.Migrations
{
    public partial class AddedOrdinalAndVoteAverage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedDateTime",
                table: "OrderedLists",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDateTime",
                table: "OrderedLists",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "Ordinal",
                table: "OrderedFilms",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VoteAverage",
                table: "Films",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDateTime",
                table: "OrderedLists");

            migrationBuilder.DropColumn(
                name: "StartDateTime",
                table: "OrderedLists");

            migrationBuilder.DropColumn(
                name: "Ordinal",
                table: "OrderedFilms");

            migrationBuilder.DropColumn(
                name: "VoteAverage",
                table: "Films");
        }
    }
}
