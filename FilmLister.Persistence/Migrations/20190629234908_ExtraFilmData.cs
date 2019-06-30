using Microsoft.EntityFrameworkCore.Migrations;

namespace FilmLister.Persistence.Migrations
{
    public partial class ExtraFilmData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Budget",
                table: "Films",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Films",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Revenue",
                table: "Films",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FilmListTemplates",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "Director",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "Revenue",
                table: "Films");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FilmListTemplates",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
