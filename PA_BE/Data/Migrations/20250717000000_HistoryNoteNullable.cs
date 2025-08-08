using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PermAdminAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class HistoryNoteNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE Histories_temp (
                    Id INTEGER NOT NULL CONSTRAINT PK_Histories PRIMARY KEY AUTOINCREMENT,
                    EmployeeId INTEGER NOT NULL,
                    EmployeeName TEXT NOT NULL,
                    ApplicationName TEXT NOT NULL,
                    Action TEXT NOT NULL,
                    Note TEXT NULL
                );
                INSERT INTO Histories_temp (Id, EmployeeId, EmployeeName, ApplicationName, Action, Note)
                SELECT Id, EmployeeId, EmployeeName, ApplicationName, Action, Note FROM Histories;
                DROP TABLE Histories;
                ALTER TABLE Histories_temp RENAME TO Histories;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE Histories_temp (
                    Id INTEGER NOT NULL CONSTRAINT PK_Histories PRIMARY KEY AUTOINCREMENT,
                    EmployeeId INTEGER NOT NULL,
                    EmployeeName TEXT NOT NULL,
                    ApplicationName TEXT NOT NULL,
                    Action TEXT NOT NULL,
                    Note TEXT NOT NULL
                );
                INSERT INTO Histories_temp (Id, EmployeeId, EmployeeName, ApplicationName, Action, Note)
                SELECT Id, EmployeeId, EmployeeName, ApplicationName, Action, Note FROM Histories;
                DROP TABLE Histories;
                ALTER TABLE Histories_temp RENAME TO Histories;
            ");
        }
    }
}
