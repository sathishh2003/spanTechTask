using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpanTechTask.Models;
using SpanTechTask.Services;

namespace SpanTechTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {

        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _adminService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("GetUserById{empId}")]
        public async Task<IActionResult> GetById(int empId)
        {
            var employee = await _adminService.GetEmployeeByIdAsync(empId);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpGet("GetEmployeeByDepartment{department}")]
        public async Task<IActionResult> GetEmployeeByDepartment(string department)
        {
            var employee = await _adminService.GetEmployeeByDepartmentAsync(department);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost("InserNewtUser")]
        public async Task<IActionResult> Add([FromBody] EmployeeModel employee)
        {
            var result = await _adminService.AddEmployeeAsync(employee);
            if (result == 1)
                return Ok("Data Inserted");
            else
                return BadRequest("Error on Inserting data");
        }

        [HttpPut("{empId}")]
        public async Task<IActionResult> Update(int empId, [FromBody] EmployeeModel employee)
        {
            employee.EmpId = empId;
            var result = await _adminService.UpdateEmployeeAsync(employee);
            if (result == 1)
                return Ok("Data Updated!");
            else
                return NotFound();
        }

        [HttpDelete("{empId}")]
        public async Task<IActionResult> Delete(int empId)
        {
            var result = await _adminService.DeleteEmployeeAsync(empId);
            if (result == 1)
                return Ok("Data Deleted!");
            else
                return NotFound();
        }

    }

}