using SpanTechTask.Models;
using Microsoft.Data.SqlClient;
using SpanTechTask.Services;

namespace SpanTechTask.Repository
{
    public class LoginRepositroy
    {

        private readonly string _connectionString;
        private readonly Base64EncryptionService _base64EncryptionService;
        private readonly ILogger<TokenService> _logger;

        public LoginRepositroy(IConfiguration configuration, Base64EncryptionService base64EncryptionService,ILogger<TokenService> logger)
        {
            _connectionString = configuration.GetConnectionString("spanTech") ?? throw new Exception("Error on Connection string!");
            _base64EncryptionService = base64EncryptionService;
            _logger = logger;
        }


        public async Task<EmployeeRoleModel?> UserAuthentication(string email, string password)
        {
            password = _base64EncryptionService.Encrypt(password);
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "Select Email,IsAdmin from span.Employees where Email = @Email and Password = @password";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            _logger.LogInformation("User Verified!");
                            return new EmployeeRoleModel
                            {
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                IsAdmin = reader.GetInt32(reader.GetOrdinal("IsAdmin"))
                            };
                        }
                    }
                    _logger.LogInformation("Invalid User Credanticals!");
                    return null;
                }
            }
        }
    }
}
