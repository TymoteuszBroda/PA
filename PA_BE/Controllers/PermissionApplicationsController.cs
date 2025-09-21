using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermAdminAPI.Data;
using PermAdminAPI.DTOs;
using PermAdminAPI.Models;
using System.Collections.Generic;
using System;
using System.Linq;

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
                UniqueId = pa.UniqueId,
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

        var year = DateTime.UtcNow.Year;

        var sequence = await context.ApplicationSequences
            .SingleOrDefaultAsync(s => s.Year == year);

        int nextNumber;

        if (sequence == null)
        {
            var existingIds = await context.PermissionApplications
                .Where(pa => pa.UniqueId.EndsWith($"/{year}"))
                .Select(pa => pa.UniqueId)
                .ToListAsync();

            var maxExisting = existingIds
                .Select(id =>
                {
                    var parts = id.Split('/', 2);
                    return int.TryParse(parts[0], out var number) ? number : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            nextNumber = maxExisting + 1;

            sequence = new ApplicationSequence
            {
                Year = year,
                LastNumber = nextNumber
            };

            context.ApplicationSequences.Add(sequence);
        }
        else
        {
            nextNumber = sequence.LastNumber + 1;
            sequence.LastNumber = nextNumber;
        }

        var uniqueId = $"{nextNumber}/{year}";

        var app = new PermissionApplication
        {
            EmployeeId = request.EmployeeId,
            LicenceId = request.LicenceId,
            IsGrant = request.IsGrant,
            UniqueId = uniqueId
        };

        context.PermissionApplications.Add(app);
        await context.SaveChangesAsync();

        request.Id = app.Id;
        request.UniqueId = app.UniqueId;
        request.EmployeeName = employee.FirstName + " " + employee.LastName;
        request.LicenceName = licence.ApplicationName;

        return Ok(request);
    }

    [HttpPost("{id}/close")]
    public async Task<ActionResult> CloseApplication(int id)
    {
        var app = await context.PermissionApplications.FindAsync(id);
        if (app == null) return NotFound();

        context.PermissionApplications.Remove(app);
        await context.SaveChangesAsync();

        return Ok();
    }
}
