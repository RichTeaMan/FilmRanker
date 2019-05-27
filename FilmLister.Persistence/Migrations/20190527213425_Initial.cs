using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FilmLister.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilmListTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmListTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderedLists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FilmListTemplateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderedLists_FilmListTemplates_FilmListTemplateId",
                        column: x => x.FilmListTemplateId,
                        principalTable: "FilmListTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FilmListItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FilmId = table.Column<int>(nullable: true),
                    FilmListTemplateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmListItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmListItem_FilmListTemplates_FilmListTemplateId",
                        column: x => x.FilmListTemplateId,
                        principalTable: "FilmListTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderedFilms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FilmId = table.Column<int>(nullable: true),
                    OrderedListId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedFilms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderedFilms_OrderedLists_OrderedListId",
                        column: x => x.OrderedListId,
                        principalTable: "OrderedLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Films",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TmdbId = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    ImdbId = table.Column<string>(nullable: true),
                    OrderedFilmId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Films", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Films_OrderedFilms_OrderedFilmId",
                        column: x => x.OrderedFilmId,
                        principalTable: "OrderedFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmListItem_FilmId",
                table: "FilmListItem",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmListItem_FilmListTemplateId",
                table: "FilmListItem",
                column: "FilmListTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Films_OrderedFilmId",
                table: "Films",
                column: "OrderedFilmId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedFilms_FilmId",
                table: "OrderedFilms",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedFilms_OrderedListId",
                table: "OrderedFilms",
                column: "OrderedListId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedLists_FilmListTemplateId",
                table: "OrderedLists",
                column: "FilmListTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_FilmListItem_Films_FilmId",
                table: "FilmListItem",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedFilms_Films_FilmId",
                table: "OrderedFilms",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedFilms_Films_FilmId",
                table: "OrderedFilms");

            migrationBuilder.DropTable(
                name: "FilmListItem");

            migrationBuilder.DropTable(
                name: "Films");

            migrationBuilder.DropTable(
                name: "OrderedFilms");

            migrationBuilder.DropTable(
                name: "OrderedLists");

            migrationBuilder.DropTable(
                name: "FilmListTemplates");
        }
    }
}
