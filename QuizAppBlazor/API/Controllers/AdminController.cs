using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAppBlazor.API.Data;
using QuizAppBlazor.API.Models;
using System.Security.Claims;

namespace QuizAppBlazor.API.Controllers
{
    [Route("api/Admin/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // POST: api/Admin/ResetPassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                // Check if current user is admin
                // var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                // var currentUser = await _userManager.FindByIdAsync(currentUserId);
                
                // if (currentUser == null)
                // {
                //     return Unauthorized("Only admin users can reset passwords");
                // }

                // Find target user
                var targetUser = await _userManager.FindByEmailAsync(request.Email);
                if (targetUser == null)
                {
                    return NotFound($"User with email {request.Email} not found");
                }

                // Remove current password
                await _userManager.RemovePasswordAsync(targetUser);
                
                // Add new password
                var result = await _userManager.AddPasswordAsync(targetUser, request.NewPassword);
                
                if (result.Succeeded)
                {
                    return Ok(new { message = "Password reset successfully" });
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to reset password for user: {Email}. Errors: {Errors}", 
                        request.Email, errors);
                    
                    return BadRequest(new { message = "Failed to reset password", errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user: {Email}", request.Email);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
