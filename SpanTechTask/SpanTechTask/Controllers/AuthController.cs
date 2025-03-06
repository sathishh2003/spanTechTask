using Microsoft.AspNetCore.Mvc;
using SpanTechTask.Services;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using SpanTechTask.Models;

namespace SpanTechTask.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var token = await _tokenService.AuthenticateAsync(model.Email, model.Password);
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Invalid credentials.");
            return Ok(new { token });
        }

    } 

}
