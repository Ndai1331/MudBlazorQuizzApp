namespace QuizAppBlazor.API.DTOs
{
    public class UserLoginDto
    {
        public Guid? Id { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }

    public class UserAuthDto
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Email { get; set; }
        public string Nickname { get; set; }
        public bool IsAuthenticated { get; set; } = false;
        public string BearerToken { get; set; }
        public string Message { get; set; } = "";
        public string Role { get; set; }
    }
}
