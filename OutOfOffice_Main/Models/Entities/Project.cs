using OutOfOffice_Main.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutOfOffice_Main.Models.Entities
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public ProjectType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [ForeignKey("Employee")]
        public int ProjectManagerId { get; set; }
        public Employee ProjectManager { get; set; } = null!;
        public string? Comment { get; set; }
        public Status Status { get; set; }
    }
}
