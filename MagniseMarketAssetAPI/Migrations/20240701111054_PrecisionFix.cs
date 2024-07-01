using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagniseMarketAssetAPI.Migrations
{
    /// <inheritdoc />
    public partial class PrecisionFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Open",
                table: "HistoricalPrices",
                type: "decimal(23,15)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Low",
                table: "HistoricalPrices",
                type: "decimal(23,15)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "High",
                table: "HistoricalPrices",
                type: "decimal(23,15)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Close",
                table: "HistoricalPrices",
                type: "decimal(23,15)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,30)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Open",
                table: "HistoricalPrices",
                type: "decimal(38,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(23,15)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Low",
                table: "HistoricalPrices",
                type: "decimal(38,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(23,15)");

            migrationBuilder.AlterColumn<decimal>(
                name: "High",
                table: "HistoricalPrices",
                type: "decimal(38,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(23,15)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Close",
                table: "HistoricalPrices",
                type: "decimal(38,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(23,15)");
        }
    }
}
