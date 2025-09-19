using Microsoft.AspNetCore.Mvc;
using Contact_Manager_Application.Interfaces;
using Contact_Manager_Application.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Contact_Manager_Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /Users/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var result = users.Select(u => new
            {
                userId = u.UserId,
                name = u.Name,
                dateOfBirth = u.DateOfBirth,
                married = u.Married,
                phone = u.Phone,
                salary = u.Salary
            });

            return Ok(result);
        }

        // PUT: /Users/Update/{id}
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var success = await _userService.UpdateUserAsync(id, updatedUser);
            if (!success)
                return NotFound("User not found");

            return Ok("User updated");
        }

        // DELETE: /Users/Delete/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound("User not found");

            return Ok("User deleted");
        }
    }
}
