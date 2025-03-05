using SpanTechTask.Models;
using SpanTechTask.Repository;

namespace SpanTechTask.Services
{
    public class AdminService
    {
        private readonly AdminRepository _adminRepository;

        public AdminService(AdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public Task<List<EmployeeModel>> GetAllEmployeesAsync()
        {
            return _adminRepository.GetAllEmployeesAsync();
        }

        public Task<EmployeeModel?> GetEmployeeByIdAsync(int empId)
        {
            return _adminRepository.GetEmployeeByIdAsync(empId);
        }

        public Task<List<EmployeeModel>> GetEmployeeByDepartmentAsync(string department)
        {
            return _adminRepository.GetEmployeeByDepartmentAsync(department);
        }

        public Task<int> AddEmployeeAsync(EmployeeModel employee)
        {
            return _adminRepository.AddEmployeeAsync(employee);
        }


        public Task<int> UpdateEmployeeAsync(EmployeeModel employee)
        {
            return _adminRepository.UpdateEmployeeAsync(employee);
        }

        public Task<int> DeleteEmployeeAsync(int empId)
        {
            return _adminRepository.DeleteEmployeeAsync(empId);
        }

    }
}
