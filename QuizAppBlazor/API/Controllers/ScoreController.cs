using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAppBlazor.API.Data;
using QuizAppBlazor.API.DTOs;
using QuizAppBlazor.API.HttpResonpse;
using QuizAppBlazor.API.Models;

namespace QuizAppBlazor.API.Controllers
{
    [Route("api/Quizz/[controller]")]
    [ApiController]
    [Authorize]
    public class ScoreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/score
        [HttpGet]
        public ActionResult<ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>> GetScoreByUserId(
            string userNickname = "",
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

            IEnumerable<GetScoreByAuthorDTO> result;

            try
            {
                var query = (
                    from s in _context.Score
                    join u in _context.Users on s.UserId equals u.Id
                    where
                         (
                            !string.IsNullOrEmpty(userNickname)
                                ? u
                                    .Nickname.Trim()
                                    .ToLower()
                                    .Contains(userNickname.Trim().ToLower())
                                : true
                        )
                        && (isAdmin == false ? s.UserId == userId : true)
                    orderby s.Date descending
                    select new GetScoreByAuthorDTO
                    {
                        Nickname = u.Nickname,
                        Points = s.CorrectAnswers,
                        Questions = s.Questions,
                        Answers = s.Answers,
                        Corrects = s.Corrects,
                        Date = s.Date
                    }
                );

                int totalRecords = query.Count();
                int totalPages = (int)Math.Ceiling((double)totalRecords / take);

                result = query.Skip(skip).Take(take).ToList();

                return Ok(
                    new ResponseBaseHttp<IEnumerable<GetScoreByAuthorDTO>>
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

        // POST: api/score/post
        [HttpPost]
        [Route("post")]
        public ActionResult PostScore([FromBody] UserScoreDTO userScore)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            var result = new ScoreModel()
            {
                UserId = userId,
                Questions = string.Join("|", userScore.Questions),
                Answers = string.Join("|", userScore.Answers),
                Corrects = string.Join("|", userScore.Corrects),
                CorrectAnswers = userScore.CorrectAnswers,
                Date = DateTime.UtcNow
            };
            var jsonPayLoad = JsonSerializer.Serialize(result);
            Console.WriteLine(jsonPayLoad);

            // have try catch here
            try
            {
                _context.Add(result);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest("ERROR: " + e.Message);
            }
            return Ok();
        }
    }
}
