using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice_Main.Managers;
using OutOfOffice_Main.Models;
using OutOfOffice_Main.Models.Entities;
using System.Security.Claims;

namespace OutOfOffice_Main.Controllers
{
    public class ListsController : Controller
    {
        private readonly IManager _manager;

        public ListsController(IManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        [Authorize(Roles = "HRManager, ProjectManager")]
        public async Task<IActionResult> Employees(string sortBy)
        {
            var employees = await _manager.GetEmployeesAsync();

            switch (sortBy)
            {
                case "FullName":
                    employees = employees.OrderByDescending(x => x.FullName).ToList();
                    break;
                case "Subdivision":
                    employees = employees.OrderByDescending(x => x.Subdivision).ToList();
                    break;
                case "Position":
                    employees = employees.OrderByDescending(x => x.Position).ToList();
                    break;
                case "Status":
                    employees = employees.OrderByDescending(x => x.Status).ToList();
                    break;
                case "Partner":
                    employees = employees.OrderByDescending(x => x.PeoplePartnerId).ToList();
                    break;
                case "OutOfOfficeBalance":
                    employees = employees.OrderByDescending(x => x.OutOfOfficeBalance).ToList();
                    break;
            }

            return View(employees);
        }

        [HttpGet]
        [Authorize(Roles = "HRManager")]
        public async Task<IActionResult> AddEmployee()
        {
            var employees = await _manager.GetEmployeesAsync();

            var employeesVM = EmployeeEntitiesToViewModels(employees);

            var addEmployeeVM = new AddEditEmployeeViewModel()
            {
                Partners = employeesVM
            };

            return View(addEmployeeVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEditEmployeeViewModel employeeViewModel)
        {
            await _manager.AddEmployeeAsync(employeeViewModel);
            return RedirectToAction("Employees", "Lists");
        }

        [HttpGet]
        [Authorize(Roles = "HRManager, ProjectManager")]
        public async Task<IActionResult> EditEmployee(int id)
        {
            var employees = await _manager.GetEmployeesAsync();

            var employee = employees.Find(x => x.Id == id);
            if (employee == null)
            {
                return View();
            }

            var employeesVM = EmployeeEntitiesToViewModels(employees);

            var addEmployeeVM = new AddEditEmployeeViewModel()
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Subdivision = employee.Subdivision,
                Position = employee.Position,
                Status = employee.Status,
                PeoplePartnerId = employee.PeoplePartnerId,
                OutOfOfficeBalance = employee.OutOfOfficeBalance,
                PhotoBase64 = employee.Photo,
                Partners = employeesVM
            };

            return View(addEmployeeVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(int id, AddEditEmployeeViewModel employeeViewModel)
        {
            await _manager.EditEmployeeAsync(id, employeeViewModel);
            return RedirectToAction("Employees", "Lists");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await _manager.DeleteEmployeeAsync(id);
            return RedirectToAction("Employees", "Lists");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LeaveRequests(string sortBy)
        {
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);

            List<LeaveRequest> requests;
            if (userRoleClaim.Value != "HRManager" && userRoleClaim.Value != "ProjectManager")
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int userId = int.Parse(userIdClaim.Value);

                requests = await _manager.GetLeaveRequestsOfUserAsync(userId);
            }
            else
            {
                requests = await _manager.GetLeaveRequestsAsync();
            }

            switch (sortBy)
            {
                case "EmployeeFullName":
                    requests = requests.OrderByDescending(x => x.Employee.FullName).ToList();
                    break;
                case "AbsenceReason":
                    requests = requests.OrderByDescending(x => x.AbsenceReason).ToList();
                    break;
                case "StartDate":
                    requests = requests.OrderByDescending(x => x.StartDate).ToList();
                    break;
                case "EndDate":
                    requests = requests.OrderByDescending(x => x.EndDate).ToList();
                    break;
                case "Status":
                    requests = requests.OrderByDescending(x => x.Status).ToList();
                    break;
            }

            return View(requests);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LeaveRequests(int id)
        {
            LeaveRequest request = await _manager.GetLeaveRequestByIdAsync(id);
            if (request == null)
            {
                return RedirectToAction("LeaveRequests", "Lists");
            }

            if (request.Status == Enums.Status.Deactivated)
            {
                await _manager.SubmitLeaveRequestsAsync(request);
            }
            else
            {
                await _manager.CancelLeaveRequestsAsync(request);

            }

            return RedirectToAction("LeaveRequests", "Lists");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddEditLeaveRequest(int? id = null)
        {
            if (id != null)
            {
                var request = await _manager.GetLeaveRequestByIdAsync(id.Value);
                ViewData["edit"] = "true";
                return View(request);
            }
            ViewData["edit"] = "false";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEditLeaveRequest(LeaveRequest leaveRequest, int? id = null)
        {
            if (id != null)
            {
                await _manager.EditLeaveRequestAsync(id.Value, leaveRequest);
            }
            else
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                int userId = int.Parse(userIdClaim.Value);
                leaveRequest.EmployeeId = userId;
                await _manager.AddLeaveRequestAsync(leaveRequest);
            }

            return RedirectToAction("LeaveRequests", "Lists");
        }


        [HttpGet]
        public async Task<IActionResult> LeaveRequestDetails(int id)
        {
            var request = await _manager.GetLeaveRequestByIdAsync(id);
            return View(request);
        }

        [HttpGet]
        [Authorize(Roles = "HRManager, ProjectManager")]
        public async Task<IActionResult> ApprovalRequests(string sortBy)
        {
            var requests = await _manager.GetApprovalRequestsAsync();

            switch (sortBy)
            {
                case "ApproverFullName":
                    requests = requests.OrderByDescending(x => x.Approver.FullName).ToList();
                    break;
                case "LeaveRequestId":
                    requests = requests.OrderByDescending(x => x.LeaveRequestId).ToList();
                    break;
                case "Status":
                    requests = requests.OrderByDescending(x => x.Status).ToList();
                    break;
            }

            return View(requests);
        }

        [HttpGet]
        [Authorize(Roles = "HRManager, ProjectManager")]
        public async Task<IActionResult> ApprovalRequestDetails(int id)
        {
            var request = await _manager.GetApprovalRequestByIdAsync(id);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> ApprovalRequestDetails(int id, ApprovalRequest approvalRequest)
        {
            await _manager.UpdateApprovalRequestStatusAsync(id, approvalRequest);
            return RedirectToAction("ApprovalRequests", "Lists");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Projects(string sortBy)
        {
            var projects = await _manager.GetProjectsAsync();

            switch (sortBy)
            {
                case "Type":
                    projects = projects.OrderByDescending(x => x.Type).ToList();
                    break;
                case "StartDate":
                    projects = projects.OrderByDescending(x => x.StartDate).ToList();
                    break;
                case "EndDate":
                    projects = projects.OrderByDescending(x => x.EndDate).ToList();
                    break;
                case "ProjectManager":
                    projects = projects.OrderByDescending(x => x.ProjectManager.FullName).ToList();
                    break;
                case "Status":
                    projects = projects.OrderByDescending(x => x.Status).ToList();
                    break;
            }

            return View(projects);
        }

        [HttpGet]
        [Authorize(Roles = "HRManager, ProjectManager")]
        public async Task<IActionResult> ProjectDetails(int id)
        {
            var request = await _manager.GetProjectByIdAsync(id);
            return View(request);
        }

        [HttpGet]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> AddProject()
        {
            var projectManagers = await _manager.GetProjectManagersAsync();

            var projectManagersVM = EmployeeEntitiesToViewModels(projectManagers);

            var addEmployeeVM = new AddEditProjectViewModel()
            {
                ProjectManagers = projectManagersVM
            };

            return View(addEmployeeVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(AddEditProjectViewModel projectViewModel)
        {
            await _manager.AddProjectAsync(projectViewModel);
            return RedirectToAction("Projects", "Lists");
        }

        [HttpGet]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> EditProject(int id)
        {
            var project = await _manager.GetProjectByIdAsync(id);

            var projectManagers = await _manager.GetProjectManagersAsync();

            if (project == null)
            {
                return View();
            }

            var projectManagersVM = EmployeeEntitiesToViewModels(projectManagers);


            var addProjectVM = new AddEditProjectViewModel()
            {
                Id = project.Id,
                Type = project.Type,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ProjectManagerId = project.ProjectManagerId,
                Comment = project.Comment,
                Status = project.Status,
                ProjectManagers = projectManagersVM
            };

            return View(addProjectVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditProject(int id, AddEditProjectViewModel projectViewModel)
        {
            await _manager.EditProjectAsync(id, projectViewModel);
            return RedirectToAction("Projects", "Lists");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _manager.DeleteProjectAsync(id);
            return RedirectToAction("Projects", "Lists");
        }

        private EmployeeViewModel[] EmployeeEntitiesToViewModels(List<Employee> employees)
        {
            EmployeeViewModel[] employeesVM = new EmployeeViewModel[employees.Count];
            for (int i = 0; i < employees.Count; i++)
            {
                employeesVM[i] = new EmployeeViewModel()
                {
                    Id = employees[i].Id,
                    FullName = employees[i].FullName,
                    Subdivision = employees[i].Subdivision,
                    Position = employees[i].Position,
                    Status = employees[i].Status,
                    PeoplePartnerId = employees[i].PeoplePartnerId,
                    OutOfOfficeBalance = employees[i].OutOfOfficeBalance,
                    Photo = null
                };
            }

            return employeesVM;
        }
    }
}
