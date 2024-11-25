using System;
namespace QuizAppBlazor.API.Helpers
{
	public class JwtSettings
	{
		public JwtSettings()
		{
		}
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int MinutesToExpiration { get; set; }
    }

    public class UserInfoToken
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string ConnectionId { get; set; }
    }

}

