using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
            try
            {
                var context = services.GetRequiredService<DataContext>();
                context.Database.Migrate();

                context.Database.ExecuteSqlRaw(@"CREATE TABLE IF NOT EXISTS Histories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    EmployeeId INTEGER NOT NULL,
                    EmployeeName TEXT,
                    ApplicationName TEXT,
                    Action TEXT,
                    Note TEXT
                );");

                context.Database.ExecuteSqlRaw(@"CREATE TABLE IF NOT EXISTS PermissionApplications (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UniqueId TEXT NOT NULL,
                    EmployeeId INTEGER NOT NULL,
                    LicenceId INTEGER NOT NULL,
                    IsGrant INTEGER NOT NULL,
                    FOREIGN KEY (EmployeeId) REFERENCES Employees(id),
                    FOREIGN KEY (LicenceId) REFERENCES Licences(id)
                );");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}