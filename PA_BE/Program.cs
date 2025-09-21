using System;
using System.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PermAdminAPI.Data;

namespace PermAdminAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<DataContext>();

                try
                {
                    context.Database.Migrate();
                }
                catch (Exception migrateEx)
                {
                    logger.LogError(migrateEx, "An error occurred while applying EF Core migrations");
                }

                TryEnsureHistoriesTable(context, logger);
                TryEnsurePermissionApplicationsTable(context, logger);
                TryEnsureEmployeeLicenceInstanceColumn(context, logger);
                TryEnsureApplicationSequencesTable(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initialising the database");
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void TryEnsureHistoriesTable(DataContext context, ILogger logger)
        {
            try
            {
                context.Database.ExecuteSqlRaw(@"CREATE TABLE IF NOT EXISTS Histories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    EmployeeId INTEGER NOT NULL,
                    EmployeeName TEXT NOT NULL,
                    ApplicationName TEXT NOT NULL,
                    Action TEXT NOT NULL,
                    Note TEXT NULL
                );");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to ensure Histories table");
            }
        }

        private static void TryEnsurePermissionApplicationsTable(DataContext context, ILogger logger)
        {
            try
            {
                context.Database.ExecuteSqlRaw(@"CREATE TABLE IF NOT EXISTS PermissionApplications (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UniqueId TEXT NOT NULL,
                    EmployeeId INTEGER NOT NULL,
                    LicenceId INTEGER NOT NULL,
                    IsGrant INTEGER NOT NULL,
                    FOREIGN KEY (EmployeeId) REFERENCES Employees(id) ON DELETE CASCADE,
                    FOREIGN KEY (LicenceId) REFERENCES Licences(id) ON DELETE CASCADE
                );");

                context.Database.ExecuteSqlRaw(@"CREATE INDEX IF NOT EXISTS IX_PermissionApplications_EmployeeId ON PermissionApplications(EmployeeId);");
                context.Database.ExecuteSqlRaw(@"CREATE INDEX IF NOT EXISTS IX_PermissionApplications_LicenceId ON PermissionApplications(LicenceId);");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to ensure PermissionApplications table");
            }
        }

        private static void TryEnsureApplicationSequencesTable(DataContext context, ILogger logger)
        {
            try
            {
                context.Database.ExecuteSqlRaw(@"CREATE TABLE IF NOT EXISTS ApplicationSequences (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Year INTEGER NOT NULL,
                    LastNumber INTEGER NOT NULL
                );");

                context.Database.ExecuteSqlRaw(@"CREATE UNIQUE INDEX IF NOT EXISTS IX_ApplicationSequences_Year ON ApplicationSequences(Year);");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to ensure ApplicationSequences table");
            }
        }

        private static void TryEnsureEmployeeLicenceInstanceColumn(DataContext context, ILogger logger)
        {
            try
            {
                var connection = context.Database.GetDbConnection();
                var previousState = connection.State;

                if (previousState != ConnectionState.Open)
                {
                    connection.Open();
                }

                try
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = "SELECT 1 FROM pragma_table_info('EmployeeLicences') WHERE name = 'LicenceInstanceId' LIMIT 1;";
                    var exists = command.ExecuteScalar() != null;

                    if (!exists)
                    {
                        context.Database.ExecuteSqlRaw("ALTER TABLE EmployeeLicences ADD COLUMN LicenceInstanceId INTEGER NULL;");
                    }

                    context.Database.ExecuteSqlRaw(@"CREATE INDEX IF NOT EXISTS IX_EmployeeLicences_LicenceInstanceId ON EmployeeLicences(LicenceInstanceId);");
                }
                finally
                {
                    if (previousState != ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to ensure EmployeeLicences schema");
            }
        }
    }
}