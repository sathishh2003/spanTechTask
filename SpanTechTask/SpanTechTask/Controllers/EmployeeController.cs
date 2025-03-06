using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

using SpanTechTask.Models;
using SpanTechTask.Services;

namespace SpanTechTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "User")]
    public class EmployeeController : ControllerBase
    {

        private readonly EmployeeService _employeeService;
        private static readonly NLog.ILogger logger = LogManager.GetCurrentClassLogger();

        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("GetUserById{empId}")]
        public async Task<IActionResult> GetById(int empId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(empId);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpGet("GetEmployeeByDepartment{department}")]
        public async Task<IActionResult> GetEmployeeByDepartment(string department)
        {
            var employee = await _employeeService.GetEmployeeByDepartmentAsync(department);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

    }

}