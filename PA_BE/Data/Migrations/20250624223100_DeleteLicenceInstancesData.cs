using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PermAdminAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteLicenceInstancesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS LicenceInstances; CREATE TABLE LicenceInstances (Id INTEGER PRIMARY KEY AUTOINCREMENT,LicenceId INTEGER,ValidTo TEXT);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data deletion cannot be reverted
        }
    }
}
