namespace PermAdminAPI.DTOs
{
    public class ReportDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string LicenceName { get; set; }
        public string Action { get; set; }
        public string Note { get; set; }
    }
}
