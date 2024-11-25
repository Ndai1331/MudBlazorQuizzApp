using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class QuestionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/questions
        [HttpGet]
        [Route("list")]
        public ActionResult<ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>> GetQuestions(
            string question = "",
            int skip = 0,
            int take = 10
        )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            IEnumerable<GetQuestionsDTO> result;

            try
            {
                var query = (
                    from q in _context.Questions
                    where
                         (
                            !string.IsNullOrEmpty(question)
                                ? q.Question.Trim()
                                    .ToLower()
                                    .Contains(question.Trim().ToLower())
                                : true
                        )
                    orderby q.Id descending
                    select new GetQuestionsDTO
                    {
                        Id = q.Id,
                        Question = q.Question,
                        CorrectAnswer = q.CorrectAnswer,
                        Alternativ2 = q.Alternativ2,
                        Alternativ3 = q.Alternativ3,
                        Alternativ4 = q.Alternativ4,
                        IsTextInput = q.IsTextInput,
                        ImageVideo = q.ImageVideo,
                        IsImage = q.IsImage,
                        IsVideo = q.IsVideo,
                        IsYoutubeVideo = q.IsYoutubeVideo,
                        HasTimeLimit = q.HasTimeLimit,
                        TimeLimit = q.TimeLimit
                    }
                );

                int totalRecords = query.Count();
                int totalPages = (int)Math.Ceiling((double)totalRecords / take);

                result = query.Skip(skip).Take(take).ToList();

                return Ok(
                    new ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>
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

        [HttpGet]
        [Route("{id}")]
        public ActionResult<ResponseBaseHttp<GetQuestionsDTO>> GetQuestionById(
          string id
      )
        {
            GetQuestionsDTO result;
            try
            {
                var query = (
                    from q in _context.Questions
                    where q.Id == int.Parse(id)
                    select new GetQuestionsDTO
                    {
                        Id = q.Id,
                        Question = q.Question,
                        CorrectAnswer = q.CorrectAnswer,
                        Alternativ2 = q.Alternativ2,
                        Alternativ3 = q.Alternativ3,
                        Alternativ4 = q.Alternativ4,
                        IsTextInput = q.IsTextInput,
                        ImageVideo = q.ImageVideo == null ? string.Empty : q.ImageVideo,
                        IsImage = q.IsImage,
                        IsVideo = q.IsVideo,
                        IsYoutubeVideo = q.IsYoutubeVideo,
                        HasTimeLimit = q.HasTimeLimit,
                        TimeLimit = q.TimeLimit
                    }
                );

                result = query.FirstOrDefault();

                return Ok(
                    new ResponseBaseHttp<GetQuestionsDTO>
                    {
                        Result = result
                    }
                );
            }
            catch (Exception e)
            {
                return BadRequest($"ERROR: {e}");
            }
        }

        [HttpGet]
        [Route("random")]
        public ActionResult<IEnumerable<GetQuestionsDTO>> GetRandomQuestion()
        {
            try
            {
                var result = _context
                    .Questions
                    .OrderBy(x => Guid.NewGuid())
                    .Take(15)
                    .Select(x => new GetQuestionsDTO
                    {
                        Question = x.Question,
                        CorrectAnswer = x.CorrectAnswer,
                        Alternativ2 = x.Alternativ2,
                        Alternativ3 = x.Alternativ3,
                        Alternativ4 = x.Alternativ4,
                        IsTextInput = x.IsTextInput,
                        ImageVideo = x.ImageVideo,
                        IsImage = x.IsImage,
                        IsVideo = x.IsVideo,
                        IsYoutubeVideo = x.IsYoutubeVideo,
                        HasTimeLimit = x.HasTimeLimit,
                        TimeLimit = x.TimeLimit
                    });

                return Ok(result.ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                return BadRequest("ERROR:" + e);
            }
        }

        // POST: api/question/create
        [HttpPost]
        [Route("create")]
        public IActionResult CreateQuestion([FromBody] CreateQuestionDTO newQuestion)
        {
            //Console.WriteLine("HEEELLLOOO");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            var result = new QuestionModel()
            {
                Question = newQuestion.Question,
                CorrectAnswer = newQuestion.CorrectAnswer,
                Alternativ2 = newQuestion.Alternativ2,
                Alternativ3 = newQuestion.Alternativ3,
                Alternativ4 = newQuestion.Alternativ4,
                IsTextInput = newQuestion.IsTextInput,
                ImageVideo = newQuestion.ImageVideo,
                IsImage = newQuestion.IsImage,
                IsVideo = newQuestion.IsVideo,
                IsYoutubeVideo = newQuestion.IsYoutubeVideo,
                HasTimeLimit = newQuestion.HasTimeLimit,
                TimeLimit = newQuestion.TimeLimit
            };
            _context.Add(result);
            _context.SaveChanges();
            return Ok();
        }




        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<ResponseBaseHttp<string>>> UpdateQuestion([FromBody] CreateQuestionDTO newQuestion, string id)
        {
            try
            {
                var questionModel = await _context.Questions.FindAsync(int.Parse(id));
                if (questionModel == null)
                {
                    return BadRequest(new ResponseBaseHttp<string> { Result = "Không tìm thấy câu hỏi" });
                }
                questionModel.Question = newQuestion.Question;
                questionModel.CorrectAnswer = newQuestion.CorrectAnswer;
                questionModel.Alternativ2 = newQuestion.Alternativ2;
                questionModel.Alternativ3 = newQuestion.Alternativ3;
                questionModel.Alternativ4 = newQuestion.Alternativ4;
                questionModel.IsTextInput = newQuestion.IsTextInput;
                questionModel.ImageVideo = newQuestion.ImageVideo;
                questionModel.IsImage = newQuestion.IsImage;
                questionModel.IsVideo = newQuestion.IsVideo;
                questionModel.IsYoutubeVideo = newQuestion.IsYoutubeVideo;
                questionModel.HasTimeLimit = newQuestion.HasTimeLimit;
                questionModel.TimeLimit = newQuestion.TimeLimit;
                _context.Update(questionModel);
                await _context.SaveChangesAsync();
                return Ok(new ResponseBaseHttp<string> { Result = string.Empty });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseBaseHttp<string> { Result = e.Message });
            }
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<ResponseBaseHttp<string>>> DeleteQuestion(string id)
        {
            try
            {
                var questionModel = await _context.Questions.FindAsync(int.Parse(id));
                if (questionModel == null)
                {
                    return BadRequest(new ResponseBaseHttp<string> { Result = "Không tìm thấy câu hỏi" });
                }
                _context.Remove(questionModel);
                int result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return Ok(new ResponseBaseHttp<string> { Result = string.Empty });
                }
                return BadRequest(new ResponseBaseHttp<string> { Result = "Không thể xóa câu hỏi." });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseBaseHttp<string> { Result = e.Message });
            }
        }


    }
}
