using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job_Portal_Website.Migrations
{
    /// <inheritdoc />
    public partial class AddJobStatusAndEmploymentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "employmentType",
                table: "JobListing",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Full-Time");

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "JobListing",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "employmentType",
                table: "JobListing");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "JobListing");
        }
    }
}
