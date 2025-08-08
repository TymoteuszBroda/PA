using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermAdminAPI.Data;
using PermAdminAPI.DTOs;
using PermAdminAPI.Models;
using System.Collections.Generic;

namespace PermAdminAPI.Controllers;

public class PermissionApplicationsController(DataContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionApplicationDTO>>> GetApplications()
    {
        var apps = await context.PermissionApplications
            .Include(pa => pa.Employee)
            .Include(pa => pa.Licence)
            .Select(pa => new PermissionApplicationDTO
            {
                Id = pa.Id,
                EmployeeId = pa.EmployeeId,
                EmployeeName = pa.Employee.FirstName + " " + pa.Employee.LastName,
                LicenceId = pa.LicenceId,
                LicenceName = pa.Licence.ApplicationName,
                IsGrant = pa.IsGrant
            })
            .ToListAsync();

        return Ok(apps);
    }

    [HttpPost]
    public async Task<ActionResult<PermissionApplicationDTO>> CreateApplication(PermissionApplicationDTO request)
    {
        var employee = await context.Employees.FindAsync(request.EmployeeId);
        if (employee == null) return NotFound("Employee not found");

        var licence = await context.Licences.FindAsync(request.LicenceId);
        if (licence == null) return NotFound("Licence not found");

        var alreadyAssigned = await context.EmployeeLicences
            .AnyAsync(el => el.employeeId == request.EmployeeId && el.licenceId == request.LicenceId);

        if (request.IsGrant && alreadyAssigned)
        {
            return BadRequest("Employee already has this licence");
        }

        if (!request.IsGrant && !alreadyAssigned)
        {
            return BadRequest("Employee does not have this licence");
        }

        var app = new PermissionApplication
        {
            EmployeeId = request.EmployeeId,
            LicenceId = request.LicenceId,
            IsGrant = request.IsGrant
        };

        context.PermissionApplications.Add(app);
        await context.SaveChangesAsync();

        request.Id = app.Id;
        request.EmployeeName = employee.FirstName + " " + employee.LastName;
        request.LicenceName = licence.ApplicationName;

        return Ok(request);
    }

    [HttpPost("{id}/close")]
    public async Task<ActionResult> CloseApplication(int id, CloseApplicationDTO request)
    {
        var app = await context.PermissionApplications
            .Include(pa => pa.Employee)
            .Include(pa => pa.Licence)
            .FirstOrDefaultAsync(pa => pa.Id == id);

        if (app == null) return NotFound();

        var report = new Report
        {
            EmployeeId = app.EmployeeId,
            EmployeeName = app.Employee.FirstName + " " + app.Employee.LastName,
            LicenceName = app.Licence.ApplicationName,
            IsGrant = app.IsGrant,
            Note = request.Note
        };

        context.Reports.Add(report);
        context.PermissionApplications.Remove(app);
        await context.SaveChangesAsync();

        return Ok();
    }
}
