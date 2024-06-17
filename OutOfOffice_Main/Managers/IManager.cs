using OutOfOffice_Main.Models;
using OutOfOffice_Main.Models.Entities;

namespace OutOfOffice_Main.Managers
{
    public interface IManager
    {
        Task<List<Employee>> GetEmployeesAsync();
        Task<List<Employee>> GetProjectManagersAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task AddEmployeeAsync(AddEditEmployeeViewModel employeeViewModel);
        Task EditEmployeeAsync(int id, AddEditEmployeeViewModel employeeViewModel);
        Task DeleteEmployeeAsync(int id);
        Task<List<LeaveRequest>> GetLeaveRequestsAsync();
        Task<List<LeaveRequest>> GetLeaveRequestsOfUserAsync(int id);
        Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);
        Task AddLeaveRequestAsync(LeaveRequest leaveRequest);
        Task EditLeaveRequestAsync(int id, LeaveRequest leaveRequest);
        Task SubmitLeaveRequestsAsync(LeaveRequest leaveRequest);
        Task CancelLeaveRequestsAsync(LeaveRequest leaveRequest);
        Task<List<ApprovalRequest>> GetApprovalRequestsAsync();
        Task<ApprovalRequest?> GetApprovalRequestByIdAsync(int id);
        Task UpdateApprovalRequestStatusAsync(int id, ApprovalRequest approvalRequest);
        Task<List<Project>> GetProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task AddProjectAsync(AddEditProjectViewModel projectViewModel);
        Task EditProjectAsync(int id, AddEditProjectViewModel projectViewModel);
        Task DeleteProjectAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
