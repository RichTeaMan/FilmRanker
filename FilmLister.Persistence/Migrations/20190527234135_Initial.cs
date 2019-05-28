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
                name: "Films",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TmdbId = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    ImdbId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Films", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderedLists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Completed = table.Column<bool>(nullable: false),
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
                name: "FilmListItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FilmId = table.Column<int>(nullable: true),
                    FilmListTemplateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmListItems_Films_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FilmListItems_FilmListTemplates_FilmListTemplateId",
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
                        name: "FK_OrderedFilms_Films_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderedFilms_OrderedLists_OrderedListId",
                        column: x => x.OrderedListId,
                        principalTable: "OrderedLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderedFilmRankItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LesserRankedFilmId = table.Column<int>(nullable: true),
                    GreaterRankedFilmId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedFilmRankItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderedFilmRankItems_OrderedFilms_GreaterRankedFilmId",
                        column: x => x.GreaterRankedFilmId,
                        principalTable: "OrderedFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderedFilmRankItems_OrderedFilms_LesserRankedFilmId",
                        column: x => x.LesserRankedFilmId,
                        principalTable: "OrderedFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmListItems_FilmId",
                table: "FilmListItems",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmListItems_FilmListTemplateId",
                table: "FilmListItems",
                column: "FilmListTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedFilmRankItems_GreaterRankedFilmId",
                table: "OrderedFilmRankItems",
                column: "GreaterRankedFilmId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedFilmRankItems_LesserRankedFilmId",
                table: "OrderedFilmRankItems",
                column: "LesserRankedFilmId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmListItems");

            migrationBuilder.DropTable(
                name: "OrderedFilmRankItems");

            migrationBuilder.DropTable(
                name: "OrderedFilms");

            migrationBuilder.DropTable(
                name: "Films");

            migrationBuilder.DropTable(
                name: "OrderedLists");

            migrationBuilder.DropTable(
                name: "FilmListTemplates");
        }
    }
}
