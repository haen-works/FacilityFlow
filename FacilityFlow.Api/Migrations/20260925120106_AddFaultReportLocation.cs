using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFaultReportLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "FaultReports",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "FaultReports");
        }
    }
}
