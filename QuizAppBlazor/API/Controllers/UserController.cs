using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAppBlazor.API.Data;
using QuizAppBlazor.API.DTOs;
using QuizAppBlazor.API.Helpers;
using QuizAppBlazor.API.HttpResponse;
using QuizAppBlazor.API.Models;

namespace QuizAppBlazor.API.Controllers
{
    [Route("api/Quizz/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private JwtSettings _settings = null;

        private readonly ApplicationDbContext _context;

        public UserController(
            ApplicationDbContext context,
            JwtSettings settings,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _settings = settings;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpPost("login")]
        [AllowAnonymous]
        [Produces("application/json", "application/xml", Type = typeof(UserAuthDto))]
        public async Task<IActionResult> UserLogin(UserLoginDto userLoginCommand)
        {
            UserAuthDto userRes = new UserAuthDto();
            var result = await _signInManager.PasswordSignInAsync(
                userLoginCommand.Email,
                userLoginCommand.Password,
                userLoginCommand.RememberMe,
                lockoutOnFailure: false
            );
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(userLoginCommand.Email);
                userRes = JwtHelper.BuildUserAuthObject(user, _settings);
            }
            else
            {
                userRes.Message = "Sai tài khoản hoặc mật khẩu";
            }

            return Ok(
                new
                {
                    userRes.Role,
                    userRes.Id,
                    userRes.Email,
                    userRes.BearerToken,
                    userRes.Message
                }
            );
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [Produces("application/json", "application/xml", Type = typeof(string))]
        public async Task<IActionResult> Register(UserLoginDto createUserCommand)
        {
            try
            {
                var appUserByCode = await _context.Users.FirstOrDefaultAsync(c =>
                    c.Email.Trim().ToLower() == createUserCommand.Email.Trim().ToLower() ||
                    c.Nickname.Trim().ToLower() == createUserCommand.Nickname.Trim().ToLower()
                );

                if (appUserByCode != null)
                {
                    return BadRequest($"Email hoặc nickname đã tồn tại.");
                }


                ApplicationUser user = new ApplicationUser
                {
                    Email = createUserCommand.Email,
                    UserName = createUserCommand.Email,
                    Nickname = createUserCommand.Nickname,
                    Role = "User",
                };
                IdentityResult result = await _userManager.CreateAsync(
                    user,
                    createUserCommand.Password
                );
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.FirstOrDefault()?.Description);
                }

                return Ok(string.Empty);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("update")]
        [Produces("application/json", "application/xml", Type = typeof(string))]
        public async Task<IActionResult> Update(UserLoginDto createUserCommand)
        {
            try
            {
                if (createUserCommand.Id == null)
                {
                    return BadRequest("Id is required");
                }
                var user = await _userManager.FindByIdAsync(createUserCommand.Id.ToString());
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var appUserByCode = await _context.Users.FirstOrDefaultAsync(c =>
                    (c.Email.Trim().ToLower() == createUserCommand.Email.Trim().ToLower() ||
                    c.Nickname.Trim().ToLower() == createUserCommand.Nickname.Trim().ToLower()) &&
                    c.Id != createUserCommand.Id.ToString()
                );

                if (appUserByCode != null)
                {
                    return BadRequest($"Email hoặc nickname đã tồn tại.");
                }

                user.Nickname = createUserCommand.Nickname;
                
                IdentityResult result = await _userManager.UpdateAsync(user);
                if ((!string.IsNullOrEmpty(createUserCommand.Email) && user.Email != createUserCommand.Email)
                    || (!string.IsNullOrEmpty(createUserCommand.Email) && user.UserName != createUserCommand.Email))
                {
                    user.UserName = createUserCommand.Email;
                    var token = await _userManager.GenerateChangeEmailTokenAsync(user, createUserCommand.Email);
                    result = await _userManager.ChangeEmailAsync(user, createUserCommand.Email, token);
                }

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.FirstOrDefault()?.Description);
                }
                if (!string.IsNullOrEmpty(createUserCommand.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    IdentityResult resultPassword = await _userManager.ResetPasswordAsync(user, token, createUserCommand.Password);
                    if (!resultPassword.Succeeded)
                    {
                        return BadRequest(resultPassword.Errors.FirstOrDefault()?.Description);
                    }
                }

                return Ok(string.Empty);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}")]
        [Produces("application/json", "application/xml", Type = typeof(UserAuthDto))]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not found");
                }
                return Ok(
                    new UserAuthDto
                    {
                        Role = user.Role,
                        Id = Guid.Parse(user.Id),
                        Email = user.Email,
                        Nickname = user.Nickname,
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET: api/score
        [HttpGet]
        public ActionResult<ResponseBaseHttp<IEnumerable<UserAuthDto>>> GetUserList(
            string search = "",
            int skip = 0,
            int take = 10,
            bool isAdmin = false
        )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            IEnumerable<UserAuthDto> result;

            try
            {
                var query = (
                    from u in _context.Users
                    where
                         (
                            !string.IsNullOrEmpty(search)
                                ? u
                                    .Nickname.Trim()
                                    .ToLower()
                                    .Contains(search.Trim().ToLower())
                                : true
                        )
                        && (
                            !string.IsNullOrEmpty(search)
                                ? u
                                    .Email.Trim()
                                    .ToLower()
                                    .Contains(search.Trim().ToLower())
                                : true
                        )
                    orderby u.Email descending
                    select new UserAuthDto
                    {
                        Id = Guid.Parse(u.Id),
                        Nickname = u.Nickname,
                        Email = u.Email,
                    }
                );

                int totalRecords = query.Count();
                int totalPages = (int)Math.Ceiling((double)totalRecords / take);

                result = query.Skip(skip).Take(take).ToList();

                return Ok(
                    new ResponseBaseHttp<IEnumerable<UserAuthDto>>
                    {
                        Result = result,
                        TotalPages = totalPages,
                        TotalRecords = totalRecords
                    }
                );
            }
            catch (Exception e)
            {
                return BadRequest($"ERROR: {e}");
            }
        }

    }
}
