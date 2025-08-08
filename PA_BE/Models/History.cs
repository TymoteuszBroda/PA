namespace PermAdminAPI.Models
{
    public class History
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ApplicationName { get; set; }
        public string Action { get; set; }
        public string? Note { get; set; }
    }
}
