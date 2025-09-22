using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAppBlazor.API.DTOs;
using QuizAppBlazor.API.HttpResponse;
using QuizAppBlazor.API.Services;

namespace QuizAppBlazor.API.Controllers
{
    [Route("api/Quizz/[controller]")]
    [ApiController]
    [Authorize]
    public class ScoreController : ControllerBase
    {
        private readonly IScoreService _scoreService;

        public ScoreController(IScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        // GET: api/score
        [HttpGet]
        public async Task<ActionResult<ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>>> GetScoreByUserId(
            string userNickname = "",
            int skip = 0,
            int take = 10,
            bool isAdmin = false
        )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User not authenticated");
            }

            try
            {
                var result = await _scoreService.GetScoresByUserAsync(userId, userNickname, skip, take, isAdmin);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>
                {
                    Result = new List<GetScoreByAuthorDTO>()
                });
            }
        }

        // POST: api/score/post
        [HttpPost]
        [Route("post")]
        public async Task<ActionResult<ResponseBaseHttp<string>>> PostScore([FromBody] UserScoreDTO userScore)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new ResponseBaseHttp<string> { Result = "User not authenticated" });
            }

            try
            {
                var result = await _scoreService.CreateScoreAsync(userId, userScore);
                if (string.IsNullOrEmpty(result.Result))
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseBaseHttp<string> { Result = "Internal server error" });
            }
        }
    }
}
