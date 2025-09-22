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
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly QuizAppBlazor.API.Services.IAuditLogService _auditLog;

        public QuestionController(IQuestionService questionService, QuizAppBlazor.API.Services.IAuditLogService auditLog)
        {
            _questionService = questionService;
            _auditLog = auditLog;
        }
        // GET: api/questions
        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>>> GetQuestions(
            string question = "",
            int skip = 0,
            int take = 10
        )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User not authenticated");
            }

            try
            {
                var result = await _questionService.GetQuestionsAsync(question, skip, take);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseBaseHttp<IEnumerable<GetQuestionsDTO>>
                {
                    Result = new List<GetQuestionsDTO>()
                });
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ResponseBaseHttp<GetQuestionsDTO>>> GetQuestionById(string id)
        {
            if (!int.TryParse(id, out int questionId))
            {
                return BadRequest(new ResponseBaseHttp<GetQuestionsDTO> { Result = null });
            }

            try
            {
                var result = await _questionService.GetQuestionByIdAsync(questionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseBaseHttp<GetQuestionsDTO> { Result = null });
            }
        }

        [HttpGet]
        [Route("random")]
        public async Task<ActionResult<IEnumerable<GetQuestionsDTO>>> GetRandomQuestion()
        {
            try
            {
                var result = await _questionService.GetRandomQuestionsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/question/create
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<ResponseBaseHttp<string>>> CreateQuestion([FromBody] CreateQuestionDTO newQuestion)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new ResponseBaseHttp<string> { Result = "User not authenticated" });
            }

            try
            {
                var result = await _questionService.CreateQuestionAsync(newQuestion);
                if (string.IsNullOrEmpty(result.Result))
                {
                    await _auditLog.LogQuestionActivityAsync(userId, 0, "CREATE", $"Created question: {newQuestion.Question}");
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseBaseHttp<string> { Result = "Internal server error" });
            }
        }




        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<ResponseBaseHttp<string>>> UpdateQuestion([FromBody] CreateQuestionDTO newQuestion, string id)
        {
            if (!int.TryParse(id, out int questionId))
            {
                return BadRequest(new ResponseBaseHttp<string> { Result = "Invalid question ID" });
            }

            try
            {
                var result = await _questionService.UpdateQuestionAsync(questionId, newQuestion);
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


        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<ResponseBaseHttp<string>>> DeleteQuestion(string id)
        {
            if (!int.TryParse(id, out int questionId))
            {
                return BadRequest(new ResponseBaseHttp<string> { Result = "Invalid question ID" });
            }

            try
            {
                var result = await _questionService.DeleteQuestionAsync(questionId);
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
