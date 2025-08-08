using PermAdminAPI.Models;
using System;

namespace PermAdminAPI.Models;

public class PermissionApplication
{
    public int Id { get; set; }
    public required string UniqueId { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public int LicenceId { get; set; }
    public Licence Licence { get; set; }
    public bool IsGrant { get; set; }
}

