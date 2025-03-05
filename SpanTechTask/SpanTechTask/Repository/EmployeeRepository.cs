using Microsoft.Data.SqlClient;
using System.Data;
using SpanTechTask.Models;
using SpanTechTask.Services;

namespace SpanTechTask.Repository
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;
        private readonly Base64EncryptionService _base64EncryptionService;

        public EmployeeRepository(IConfiguration configuration, Base64EncryptionService base64EncryptionService)
        {
            _connectionString = configuration.GetConnectionString("spanTech") ?? throw new Exception("Error on Connection string!");
            _base64EncryptionService = base64EncryptionService;

        }

       

        public async Task<List<EmployeeViewModel>> GetAllEmployeesAsync()
        {
            var employees = new List<EmployeeViewModel>();
            using (var conn = new SqlConnection(_connectionString))
            {
               await conn.OpenAsync(); 
                var query = "SELECT * FROM span.Employees";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var employee = new EmployeeViewModel
                            {
                                EmpId = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                                IsAdmin = reader.IsDBNull(reader.GetOrdinal("IsAdmin")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsAdmin")),
                                Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? string.Empty : reader.GetString(reader.GetOrdinal("Department"))
                            };
                            employees.Add(employee);
                        }
                    }
                }
            }
            return employees;
        }

        public async Task<EmployeeViewModel?> GetEmployeeByIdAsync(int empId)
        {
            EmployeeViewModel employee = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = "SELECT * FROM span.Employees WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", empId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            employee = new EmployeeViewModel
                            {
                                EmpId = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                                IsAdmin = reader.IsDBNull(reader.GetOrdinal("IsAdmin")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsAdmin")),
                                Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? string.Empty : reader.GetString(reader.GetOrdinal("Department"))
                            };
                        }
                    }
                }
            }
            return employee;
        }

        public async Task<List<EmployeeViewModel>> GetEmployeeByDepartmentAsync(string department)
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = "SELECT * FROM span.Employees WHERE Department = @department";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@department", department);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var employee = new EmployeeViewModel
                            {
                                EmpId = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                                IsAdmin = reader.IsDBNull(reader.GetOrdinal("IsAdmin")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsAdmin")),
                                Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? string.Empty : reader.GetString(reader.GetOrdinal("Department"))
                            };
                            employees.Add(employee);
                        }
                    }
                }
            }
            return employees;
        }

    }
}
