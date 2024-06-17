using Microsoft.EntityFrameworkCore;
using OutOfOffice_Main.Models.Entities;

namespace OutOfOffice_Main.Data
{
    public class OutOfOfficeContext : DbContext, IOutOfOfficeContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }

        public OutOfOfficeContext(DbContextOptions<OutOfOfficeContext> options)
            : base(options)
        {
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
