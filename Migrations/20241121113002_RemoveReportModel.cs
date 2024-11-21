using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace correos_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReportModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Budgets",
                newName: "FileUrl");

            migrationBuilder.RenameColumn(
                name: "BudgetDocumentLink",
                table: "Budgets",
                newName: "FileName");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Budgets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Budgets",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Budgets");

            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "Budgets",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Budgets",
                newName: "BudgetDocumentLink");

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewStatus = table.Column<int>(type: "int", nullable: false),
                    ReviewerComments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportId);
                });
        }
    }
}
