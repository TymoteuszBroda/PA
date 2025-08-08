namespace PermAdminAPI.DTOs;

public class PermissionApplicationDTO
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public int LicenceId { get; set; }
    public string LicenceName { get; set; }
    public bool IsGrant { get; set; }
}
