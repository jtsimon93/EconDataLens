using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EconDataLens.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cpi_area",
                columns: table => new
                {
                    area_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    area_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cpi_area", x => x.area_code);
                });

            migrationBuilder.CreateTable(
                name: "cpi_footnote",
                columns: table => new
                {
                    footnote_code = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    footnote_text = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cpi_footnote", x => x.footnote_code);
                });

            migrationBuilder.CreateTable(
                name: "cpi_item",
                columns: table => new
                {
                    item_code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    item_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cpi_item", x => x.item_code);
                });

            migrationBuilder.CreateTable(
                name: "cpi_period",
                columns: table => new
                {
                    period = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    period_abbreviation = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    period_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cpi_period", x => x.period);
                });

            migrationBuilder.CreateTable(
                name: "cpi_series",
                columns: table => new
                {
                    series_id = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    area_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    item_code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    seasonal = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    periodicity_code = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    base_code = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    base_period = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    series_title = table.Column<string>(type: "text", nullable: false),
                    footnote_codes = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    begin_year = table.Column<int>(type: "integer", maxLength: 4, nullable: false),
                    begin_period = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    end_year = table.Column<int>(type: "integer", maxLength: 4, nullable: false),
                    end_period = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cpi_series", x => x.series_id);
                    table.ForeignKey(
                        name: "fk_cpi_series_cpi_area_area_code",
                        column: x => x.area_code,
                        principalTable: "cpi_area",
                        principalColumn: "area_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cpi_series_cpi_item_item_code",
                        column: x => x.item_code,
                        principalTable: "cpi_item",
                        principalColumn: "item_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cpi_series_cpi_period_begin_period",
                        column: x => x.begin_period,
                        principalTable: "cpi_period",
                        principalColumn: "period",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cpi_series_cpi_period_end_period",
                        column: x => x.end_period,
                        principalTable: "cpi_period",
                        principalColumn: "period",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cpi_data",
                columns: table => new
                {
                    series_id = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    year = table.Column<int>(type: "integer", maxLength: 4, nullable: false),
                    period = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    value = table.Column<decimal>(type: "numeric", maxLength: 12, nullable: false),
                    footnote_codes = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cpi_data", x => new { x.series_id, x.year, x.period });
                    table.ForeignKey(
                        name: "fk_cpi_data_cpi_period_period",
                        column: x => x.period,
                        principalTable: "cpi_period",
                        principalColumn: "period",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cpi_data_cpi_series_series_id",
                        column: x => x.series_id,
                        principalTable: "cpi_series",
                        principalColumn: "series_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cpi_data_period",
                table: "cpi_data",
                column: "period");

            migrationBuilder.CreateIndex(
                name: "ix_cpi_series_area_code",
                table: "cpi_series",
                column: "area_code");

            migrationBuilder.CreateIndex(
                name: "ix_cpi_series_begin_period",
                table: "cpi_series",
                column: "begin_period");

            migrationBuilder.CreateIndex(
                name: "ix_cpi_series_end_period",
                table: "cpi_series",
                column: "end_period");

            migrationBuilder.CreateIndex(
                name: "ix_cpi_series_item_code",
                table: "cpi_series",
                column: "item_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cpi_data");

            migrationBuilder.DropTable(
                name: "cpi_footnote");

            migrationBuilder.DropTable(
                name: "cpi_series");

            migrationBuilder.DropTable(
                name: "cpi_area");

            migrationBuilder.DropTable(
                name: "cpi_item");

            migrationBuilder.DropTable(
                name: "cpi_period");
        }
    }
}
