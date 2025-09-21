using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PermAdminAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLicenceInstanceReferenceToEmployeeLicence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LicenceInstanceId",
                table: "EmployeeLicences",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLicences_LicenceInstanceId",
                table: "EmployeeLicences",
                column: "LicenceInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLicences_LicenceInstances_LicenceInstanceId",
                table: "EmployeeLicences",
                column: "LicenceInstanceId",
                principalTable: "LicenceInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLicences_LicenceInstances_LicenceInstanceId",
                table: "EmployeeLicences");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLicences_LicenceInstanceId",
                table: "EmployeeLicences");

            migrationBuilder.DropColumn(
                name: "LicenceInstanceId",
                table: "EmployeeLicences");
        }
    }
}
