using Microsoft.EntityFrameworkCore.Migrations;

namespace spendingAPI.Migrations
{
    public partial class EntryAmountChangedFromIntToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Entries",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Entries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
