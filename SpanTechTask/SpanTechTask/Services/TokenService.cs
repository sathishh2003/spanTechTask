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
        private readonly ILogger<LoginRepositroy> _logger;
        public TokenService(IConfiguration configuration,LoginRepositroy loginRepositroy, ILogger<LoginRepositroy> logger)
        {
            _configuration = configuration;
            _loginRepositroy = loginRepositroy;
            _logger = logger;
        }

        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            if(email == "admin@gmail.com" && password == "12345")
            {
                return GenerateToken(email, "Admin");
            }
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

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("Role", role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

          
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = _configuration["Jwt:Key"];
            var tokenValidatyMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes");
            var tokenExpiryTime = DateTime.Now.AddMinutes(tokenValidatyMinutes);
           
            var toeknDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Email,email),
                new Claim("Role",role)
                 }),
                Expires = tokenExpiryTime,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256Signature),

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(toeknDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            _logger.LogInformation("Token Generated Sucessfully!");

            return accessToken;
        }
    }
}
