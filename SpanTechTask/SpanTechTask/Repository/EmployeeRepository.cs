using System.Data.SqlClient;
using System.Data;
using SpanTechTask.Models;

namespace SpanTechTask.Repository
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
                            employees.Add(new EmployeeModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Role = reader.GetString(3),
                                Department = reader.GetString(4),
                                CreatedAt = reader.GetDateTime(5),
                                UpdatedAt = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }
            return employees;
        }

        public async Task<EmployeeModel> GetEmployeeByIdAsync(int id)
        {
            EmployeeModel employee = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = "SELECT * FROM HR.Employees WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            employee = new EmployeeModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Role = reader.GetString(3),
                                Department = reader.GetString(4),
                                CreatedAt = reader.GetDateTime(5),
                                UpdatedAt = reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }
            return employee;
        }

        public async Task<int> AddEmployeeAsync(EmployeeModel employee)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = @"INSERT INTO HR.Employees (Name, Email, Role, Department)
                              VALUES (@Name, @Email, @Role, @Department)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Role", employee.Role);
                    cmd.Parameters.AddWithValue("@Department", employee.Department);
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateEmployeeAsync(EmployeeModel employee)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = @"UPDATE HR.Employees SET Name = @Name, Email = @Email,
                              Role = @Role, Department = @Department, UpdatedAt = GETDATE()
                              WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", employee.Id);
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Role", employee.Role);
                    cmd.Parameters.AddWithValue("@Department", employee.Department);
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteEmployeeAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = "DELETE FROM HR.Employees WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
