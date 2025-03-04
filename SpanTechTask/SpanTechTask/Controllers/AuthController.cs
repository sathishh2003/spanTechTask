using Microsoft.AspNetCore.Mvc;
using SpanTechTask.Services;
using System.Data.Common;
using System.Data.SqlClient;

namespace SpanTechTask.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly SqlConnection _connection;

        public  AuthController(TokenService tokenService,IConfiguration config)
        {
            _tokenService = tokenService;
            _config = config;
            _connection = new SqlConnection(_config.GetConnectionString("spanTech"));
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            _connection.Open();
            var query = "SELECT Role FROM Users WHERE Email = @Email AND Password = @Password";
            var cmd = new SqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Password", model.Password);

            var role = cmd.ExecuteScalar()?.ToString();
            _connection.Close();

            if (role == null)
                return Unauthorized("Invalid credentials.");

            var token = _tokenService.GenerateToken(model.Email, role);
            return Ok(new { Token = token, Role = role });
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
