using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermAdminAPI.Data;
using PermAdminAPI.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace PermAdminAPI.Controllers;

public class ReportsController(DataContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReportDTO>>> GetReports()
    {
        var reports = await context.Reports
            .Select(r => new ReportDTO
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                EmployeeName = r.EmployeeName,
                LicenceName = r.LicenceName,
                Action = r.IsGrant ? "granted" : "revoked",
                Note = r.Note
            })
            .ToListAsync();

        return Ok(reports);
    }
}
