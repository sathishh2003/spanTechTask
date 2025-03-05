using SpanTechTask.Models;
using SpanTechTask.Repository;

namespace SpanTechTask.Services
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _employeeRepository;

        public EmployeeService(EmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public Task<List<EmployeeViewModel>> GetAllEmployeesAsync()
        {
            return _employeeRepository.GetAllEmployeesAsync();
        }

        public Task<EmployeeViewModel?> GetEmployeeByIdAsync(int empId)
        {
            return _employeeRepository.GetEmployeeByIdAsync(empId);
         } 
        
        public Task<List<EmployeeViewModel>> GetEmployeeByDepartmentAsync(string department)
        {
            return _employeeRepository.GetEmployeeByDepartmentAsync(department);
         }
    }
}
