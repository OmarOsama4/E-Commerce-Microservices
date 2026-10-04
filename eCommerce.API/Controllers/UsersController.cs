using eCommerce.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserByUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            return BadRequest("User ID can't be empty.");

        var user = await userService.GetUserByUserId(userId);
        if (user is null)
            return NotFound($"User with ID {userId} not found.");
        return Ok(user);
    }
}