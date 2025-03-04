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

        public Task<List<EmployeeModel>> GetAllEmployeesAsync()
        {
            return _employeeRepository.GetAllEmployeesAsync();
        }

        public Task<EmployeeModel> GetEmployeeByIdAsync(int id)
        {
            return _employeeRepository.GetEmployeeByIdAsync(id);
         }

        public Task<int> AddEmployeeAsync(EmployeeModel employee)
        {
            return _employeeRepository.AddEmployeeAsync(employee);
        }


        public Task<int> UpdateEmployeeAsync(EmployeeModel employee)
        {
            return _employeeRepository.UpdateEmployeeAsync(employee);
        }

        public Task<int> DeleteEmployeeAsync(int id)
        {
            return _employeeRepository.DeleteEmployeeAsync(id);
        }

    }
}
