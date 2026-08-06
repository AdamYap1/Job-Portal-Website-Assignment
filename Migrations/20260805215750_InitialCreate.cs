using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job_Portal_Website.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employer",
                columns: table => new
                {
                    employerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    companyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employerEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    employerPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employerDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employerIndustry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    logoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employer", x => x.employerId);
                });

            migrationBuilder.CreateTable(
                name: "JobSeeker",
                columns: table => new
                {
                    jobSeekerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    seekerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    seekerEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    seekerPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    seekerSkills = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    seekerExp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resumePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeeker", x => x.jobSeekerId);
                });

            migrationBuilder.CreateTable(
                name: "JobListing",
                columns: table => new
                {
                    jobListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jobDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jobRequirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jobSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    jobLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isClosed = table.Column<bool>(type: "bit", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    postedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    employerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobListing", x => x.jobListId);
                    table.ForeignKey(
                        name: "FK_JobListing_Employer_employerId",
                        column: x => x.employerId,
                        principalTable: "Employer",
                        principalColumn: "employerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    applyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobSeekerId = table.Column<int>(type: "int", nullable: false),
                    jobListId = table.Column<int>(type: "int", nullable: false),
                    applyStatus = table.Column<int>(type: "int", nullable: false),
                    appliedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.applyId);
                    table.ForeignKey(
                        name: "FK_Application_JobListing_jobListId",
                        column: x => x.jobListId,
                        principalTable: "JobListing",
                        principalColumn: "jobListId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Application_JobSeeker_jobSeekerId",
                        column: x => x.jobSeekerId,
                        principalTable: "JobSeeker",
                        principalColumn: "jobSeekerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_jobListId",
                table: "Application",
                column: "jobListId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_jobSeekerId_jobListId",
                table: "Application",
                columns: new[] { "jobSeekerId", "jobListId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employer_employerEmail",
                table: "Employer",
                column: "employerEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobListing_employerId",
                table: "JobListing",
                column: "employerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeeker_seekerEmail",
                table: "JobSeeker",
                column: "seekerEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "JobListing");

            migrationBuilder.DropTable(
                name: "JobSeeker");

            migrationBuilder.DropTable(
                name: "Employer");
        }
    }
}
