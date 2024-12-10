using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace correos_backend.Migrations
{
    /// <inheritdoc />
    public partial class BudgetCreatorInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Budgets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Budgets");
        }
    }
}
