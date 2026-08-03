using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job_Portal_Website.Migrations
{
    /// <summary>
    /// US-05 "Inactive account" acceptance test. Existing accounts are
    /// backfilled as active so that nobody is locked out by this migration.
    /// </summary>
    public partial class AddAccountActiveStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "JobSeeker",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Employer",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "JobSeeker");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Employer");
        }
    }
}
