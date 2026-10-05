using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFaultReportReporter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportedById",
                table: "FaultReports",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaultReports_ReportedById",
                table: "FaultReports",
                column: "ReportedById");

            migrationBuilder.AddForeignKey(
                name: "FK_FaultReports_Users_ReportedById",
                table: "FaultReports",
                column: "ReportedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaultReports_Users_ReportedById",
                table: "FaultReports");

            migrationBuilder.DropIndex(
                name: "IX_FaultReports_ReportedById",
                table: "FaultReports");

            migrationBuilder.DropColumn(
                name: "ReportedById",
                table: "FaultReports");
        }
    }
}
