using OutOfOffice_Main.Enums;

namespace OutOfOffice_Main.Models
{
    public class AddEditEmployeeViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public Department Subdivision { get; set; }
        public Position Position { get; set; }
        public Status Status { get; set; }
        public int? PeoplePartnerId { get; set; }
        public int OutOfOfficeBalance { get; set; }
        public IFormFile? Photo { get; set; }
        public string? PhotoBase64 { get; set; }

        public EmployeeViewModel[] Partners { get; set; } = null!;
    }
}
