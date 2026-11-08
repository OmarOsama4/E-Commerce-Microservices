using eCommerce.Core.Domain.DTO;
using eCommerce.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserService userService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO registerRequest)
        {
            if (registerRequest is null)
                return BadRequest("Register request is null");

            var result = await userService.Register(registerRequest);

            if (result is null || result.Success == false)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            if (loginRequest is null)
                return BadRequest("Login request is null");

            var result = await userService.Login(loginRequest);

            if (result is null || result.Success == false)
                return Unauthorized(result);
            
            return Ok(result);

        }
    }
}
