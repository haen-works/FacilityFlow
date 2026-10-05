using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFaultReportPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "FaultReports",
                type: "TEXT",
                nullable: false,
                defaultValue: "Medium");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "FaultReports");
        }
    }
}
