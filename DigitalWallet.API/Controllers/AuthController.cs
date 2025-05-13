using DigitalWallet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.API.Controllers
{
    
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("generate_token")]
        public IActionResult GenerateToken()
        {
            string email = "user@example.com";

            var token = _jwtTokenService.GenerateToken(userId: "admin1", email: email);
            return Ok(new { Token = token });
        }
    }
}
