namespace PermAdminAPI.DTOs
{
    public class HistoryDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ApplicationName { get; set; }
        public string Action { get; set; }
        public string Note { get; set; }
    }
}
