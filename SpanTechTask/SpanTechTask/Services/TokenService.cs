using Microsoft.IdentityModel.Tokens;
using SpanTechTask.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SpanTechTask.Models;
using System.Text;


namespace SpanTechTask.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly LoginRepositroy _loginRepositroy;

        public TokenService(IConfiguration configuration,LoginRepositroy loginRepositroy)
        {
            _configuration = configuration;
            _loginRepositroy = loginRepositroy;
        }

        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            var employee = await _loginRepositroy.UserAuthentication(email, password);
            if (employee != null)
            {
                var role = employee.IsAdmin == 1 ? "Admin" : "User";
                return GenerateToken(employee.Email, role);
            }
            return null;
        }


        public string GenerateToken(string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim("Role", role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
