using OutOfOffice_Main.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutOfOffice_Main.Models.Entities
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = null!;
        public Department Subdivision { get; set; }
        public Position Position { get; set; }
        public Status Status { get; set; }
        [ForeignKey("Employee")]
        public int? PeoplePartnerId { get; set; }
        public Employee? PeoplePartner { get; set; } = null!;
        public int OutOfOfficeBalance { get; set; }
        public string? Photo { get; set; }

        public ICollection<ApprovalRequest> ApprovalReq { get; set; }
        public ICollection<LeaveRequest> LeaveReq { get; set; }
        public User User { get; set; }
    }
}
