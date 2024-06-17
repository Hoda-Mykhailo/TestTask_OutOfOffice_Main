using Microsoft.EntityFrameworkCore;
using OutOfOffice_Main.Data;
using OutOfOffice_Main.Enums;
using OutOfOffice_Main.Migrations;
using OutOfOffice_Main.Models;
using OutOfOffice_Main.Models.Entities;

namespace OutOfOffice_Main.Managers
{
    public class Manager : IManager
    {
        private readonly IOutOfOfficeContext _context;

        public Manager(IOutOfOfficeContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<List<Employee>> GetProjectManagersAsync()
        {
            return await _context.Employees
                .Where(x => x.Position == Position.ProjectManager)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task AddEmployeeAsync(AddEditEmployeeViewModel employeeViewModel)
        {
            string? photoString = null;
            if (employeeViewModel.Photo != null)
            {
                photoString = await PhototoStringAsync(employeeViewModel.Photo);
            }


            var employee = new Employee()
            {
                FullName = employeeViewModel.FullName,
                Subdivision = employeeViewModel.Subdivision,
                Position = employeeViewModel.Position,
                Status = employeeViewModel.Status,
                PeoplePartnerId = employeeViewModel.PeoplePartnerId,
                OutOfOfficeBalance = employeeViewModel.OutOfOfficeBalance,
                Photo = photoString
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task EditEmployeeAsync(int id, AddEditEmployeeViewModel employeeViewModel)
        {
            var employee = await GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return;
            }

            string? photoString = null;
            if (employeeViewModel.Photo != null)
            {
                photoString = await PhototoStringAsync(employeeViewModel.Photo);
            }

            employee.FullName = employeeViewModel.FullName;
            employee.Subdivision = employeeViewModel.Subdivision;
            employee.Position = employeeViewModel.Position;
            employee.Status = employeeViewModel.Status;
            employee.PeoplePartnerId = employeeViewModel.PeoplePartnerId;
            employee.OutOfOfficeBalance = employeeViewModel.OutOfOfficeBalance;
            employee.Photo = photoString;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsAsync()
        {
            return await _context.LeaveRequests
                .Include(x => x.Employee)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsOfUserAsync(int id)
        {
            return await _context.LeaveRequests
               .Include(x => x.Employee)
               .Where(x => x.EmployeeId == id)
               .ToListAsync();
        }

        public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(x => x.Employee)
                .FirstAsync(x => x.Id == id);
        }

        public async Task AddLeaveRequestAsync(LeaveRequest leaveRequest)
        {
            await _context.LeaveRequests.AddAsync(leaveRequest);
            await _context.SaveChangesAsync();
        }

        public async Task EditLeaveRequestAsync(int id, LeaveRequest leaveRequest)
        {
            var request = await GetLeaveRequestByIdAsync(id);
            if (request == null)
            {
                return;
            }

            request.AbsenceReason = leaveRequest.AbsenceReason;
            request.StartDate = leaveRequest.StartDate;
            request.EndDate = leaveRequest.EndDate;
            request.Comment = leaveRequest.Comment;
            request.Status = leaveRequest.Status;

            await _context.SaveChangesAsync();
        }

        public async Task<List<ApprovalRequest>> GetApprovalRequestsAsync()
        {
            return await _context.ApprovalRequests
                .Include(x => x.Approver)
                .Include(x => x.LeaveRequest)
                .ToListAsync();
        }

        public async Task<ApprovalRequest?> GetApprovalRequestByIdAsync(int id)
        {
            return await _context.ApprovalRequests
                .Include(x => x.Approver)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _context.Projects
                .Include(x => x.ProjectManager)
                .ToListAsync();
        }

        private async Task<string> PhototoStringAsync(IFormFile photo)
        {
            string photoString;
            using (var memoryStream = new MemoryStream())
            {
                await photo.CopyToAsync(memoryStream);
                byte[] photoBytes = memoryStream.ToArray();
                photoString = Convert.ToBase64String(photoBytes);
            }
            return photoString;

        }

        public async Task UpdateApprovalRequestStatusAsync(int id, ApprovalRequest approvalRequest)
        {
            var request = await GetApprovalRequestByIdAsync(id);

            if (request != null)
            {
                request.Comment = approvalRequest.Comment;
                request.Status = approvalRequest.Status;

                await _context.SaveChangesAsync();
            }
        }

        public async Task SubmitLeaveRequestsAsync(LeaveRequest leaveRequest)
        {
            var request = await GetLeaveRequestByIdAsync(leaveRequest.Id);
            request.Status = Status.Active;

            var approvalRequest = new ApprovalRequest()
            {
                ApproverId = leaveRequest.EmployeeId,
                LeaveRequestId = leaveRequest.Id,
                Comment = leaveRequest.Comment,
                Status = Status.Deactivated,
            };

            await _context.ApprovalRequests.AddAsync(approvalRequest);
            await _context.SaveChangesAsync();
        }

        public async Task CancelLeaveRequestsAsync(LeaveRequest leaveRequest)
        {
            var request = await GetLeaveRequestByIdAsync(leaveRequest.Id);
            request.Status = Status.Deactivated;

            var approvalRequests = await GetApprovalRequestsAsync();
            approvalRequests = approvalRequests.Where(x => x.LeaveRequestId == leaveRequest.Id).ToList();

            _context.ApprovalRequests.RemoveRange(approvalRequests);
            await _context.SaveChangesAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
               .Include(x => x.ProjectManager)
               .FirstAsync(x => x.Id == id);
        }

        public async Task AddProjectAsync(AddEditProjectViewModel projectViewModel)
        {
            var project = new Project()
            {
                Type = projectViewModel.Type,
                StartDate = projectViewModel.StartDate,
                EndDate = projectViewModel.EndDate,
                ProjectManagerId = projectViewModel.ProjectManagerId,
                Comment = projectViewModel.Comment,
                Status = projectViewModel.Status
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        public async Task EditProjectAsync(int id, AddEditProjectViewModel projectViewModel)
        {
            var project = await GetProjectByIdAsync(id);
            if (project == null)
            {
                return;
            }

            project.Type = projectViewModel.Type;
            project.StartDate = projectViewModel.StartDate;
            project.EndDate = projectViewModel.EndDate;
            project.ProjectManagerId = projectViewModel.ProjectManagerId;
            project.Comment = projectViewModel.Comment;
            project.Status = projectViewModel.Status;

            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }
    }
}
