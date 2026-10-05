using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class SyncFaultReportAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedTechnicianId",
                table: "FaultReports",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaultReports_AssignedTechnicianId",
                table: "FaultReports",
                column: "AssignedTechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_FaultReports_Users_AssignedTechnicianId",
                table: "FaultReports",
                column: "AssignedTechnicianId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaultReports_Users_AssignedTechnicianId",
                table: "FaultReports");

            migrationBuilder.DropIndex(
                name: "IX_FaultReports_AssignedTechnicianId",
                table: "FaultReports");

            migrationBuilder.DropColumn(
                name: "AssignedTechnicianId",
                table: "FaultReports");
        }
    }
}
