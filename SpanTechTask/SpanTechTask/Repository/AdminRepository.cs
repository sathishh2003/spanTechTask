using Microsoft.Data.SqlClient;
using System.Data;
using SpanTechTask.Models;
using SpanTechTask.Services;

namespace SpanTechTask.Repository
{
    public class AdminRepository
    {
        private readonly string _connectionString;
        private readonly Base64EncryptionService _base64EncryptionService;
        private readonly ILogger<AdminRepository> _logger;

        public AdminRepository(IConfiguration configuration, Base64EncryptionService base64EncryptionService, ILogger<AdminRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("spanTech") ?? throw new Exception("Error on Connection string!");
            _base64EncryptionService = base64EncryptionService;
            _logger = logger;
        }

        public async Task<List<EmployeeModel>> GetAllEmployeesAsync()
        {
            var employees = new List<EmployeeModel>();
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
                            var employee = new EmployeeModel
                            {
                                EmpId = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                                IsAdmin = reader.IsDBNull(reader.GetOrdinal("IsAdmin")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsAdmin")),
                                Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? string.Empty : reader.GetString(reader.GetOrdinal("Department")),
                                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            };
                            employees.Add(employee);
                        }
                    }
                }
                _logger.LogInformation("Fetchcing all the by Admin");
            }
            return employees;
        }

        public async Task<EmployeeModel?> GetEmployeeByIdAsync(int empId)
        {
            EmployeeModel employee = null;
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
                            employee = new EmployeeModel
                            {
                                EmpId = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                                IsAdmin = reader.IsDBNull(reader.GetOrdinal("IsAdmin")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsAdmin")),
                                Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? string.Empty : reader.GetString(reader.GetOrdinal("Department")),
                                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            };
                        }
                    }
                }
                _logger.LogInformation($"Fetchcing {empId} the by Admin");
            }
            return employee;
        }

        public async Task<List<EmployeeModel>> GetEmployeeByDepartmentAsync(string department)
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
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
                            var employee = new EmployeeModel
                            {
                                EmpId = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                                IsAdmin = reader.IsDBNull(reader.GetOrdinal("IsAdmin")) ? 0 : reader.GetInt32(reader.GetOrdinal("IsAdmin")),
                                Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? string.Empty : reader.GetString(reader.GetOrdinal("Department")),
                                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            };
                            employees.Add(employee);
                        }
                    }
                }
                _logger.LogInformation($"Fetchcing {department} department details the by Admin");
            }
            return employees;
        }

        public async Task<int> AddEmployeeAsync(EmployeeModel employee)
        {
            employee.Password = _base64EncryptionService.Encrypt(employee.Password);
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                _logger.LogInformation($"Adding {employee.Name}  by Admin");
                var query = @"INSERT INTO span.Employees (Name, Email, Password, IsAdmin, Department, CreatedAt)
                              VALUES (@Name, @Email, @Password,@IsAdmin, @Department, @CreatedAt)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Password", employee.Password);
                    cmd.Parameters.AddWithValue("@IsAdmin", employee.IsAdmin);
                    cmd.Parameters.AddWithValue("@Department", employee.Department.ToUpper());
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateEmployeeAsync(EmployeeModel employee)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                _logger.LogInformation($"Updating {employee.Name}  by Admin");
                var query = @"UPDATE span.Employees SET Name = @Name, Email = @Email, Password = @Password,
                              IsAdmin = @IsAdmin, Department = @Department, UpdatedAt = GETDATE()
                              WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", employee.EmpId);
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Password", employee.Password);
                    cmd.Parameters.AddWithValue("@IsAdmin", employee.IsAdmin);
                    cmd.Parameters.AddWithValue("@Department", employee.Department);
                    cmd.Parameters.AddWithValue("@UpdatedAt", employee.UpdatedAt);
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteEmployeeAsync(int empId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                _logger.LogInformation($"Removing {empId}  by Admin");
                var query = "DELETE FROM span.Employees WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", empId);
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
