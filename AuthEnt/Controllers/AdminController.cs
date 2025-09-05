using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthEnt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromQuery] string roleName)
        {
            var exists = await _roleManager.RoleExistsAsync(roleName);
            if (exists) return Conflict("Role already exists");
            var result = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName, Description = "" });
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromQuery] string email, [FromQuery] string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();
            var res = await _userManager.AddToRoleAsync(user, role);
            if (!res.Succeeded) return BadRequest(res.Errors);
            return Ok();
        }
    }
}
