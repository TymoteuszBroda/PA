namespace PermAdminAPI.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string LicenceName { get; set; }
        public bool IsGrant { get; set; }
        public string Note { get; set; }
    }
}
