using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermAdminAPI.Data;
using PermAdminAPI.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace PermAdminAPI.Controllers;

public class HistoryController(DataContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HistoryDTO>>> GetHistory()
    {
        var history = await context.Histories
            .Select(r => new HistoryDTO
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                EmployeeName = r.EmployeeName,
                ApplicationName = r.ApplicationName,
                Action = r.Action,
                Note = r.Note
            })
            .ToListAsync();

        return Ok(history);
    }
}
