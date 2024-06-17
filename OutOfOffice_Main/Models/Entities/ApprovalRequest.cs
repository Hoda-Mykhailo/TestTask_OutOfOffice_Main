using OutOfOffice_Main.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OutOfOffice_Main.Models.Entities
{
    public class ApprovalRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Employee")]
        public int ApproverId { get; set; }
        public Employee Approver { get; set; } = null!;
        [ForeignKey("LeaveRequest")]
        public int LeaveRequestId { get; set; }
        public LeaveRequest LeaveRequest { get; set; } = null!;
        public string? Comment { get; set; }
        public Status Status { get; set; }
    }
}
