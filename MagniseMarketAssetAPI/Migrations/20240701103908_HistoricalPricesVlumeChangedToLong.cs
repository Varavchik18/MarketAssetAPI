using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagniseMarketAssetAPI.Migrations
{
    /// <inheritdoc />
    public partial class HistoricalPricesVlumeChangedToLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Volume",
                table: "HistoricalPrices",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Volume",
                table: "HistoricalPrices",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
