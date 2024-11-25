using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppBlazor.API.DTOs
{
    public class GetScoreByAuthorDTO
    {
        public string Title { get; set; }
        public string Nickname { get; set; }
        public int Points { get; set; }
        public string Questions { get; set; }
        public string Answers { get; set; }
        public string Corrects { get; set; }
        public DateTime? Date { get; set; }
    }
}
