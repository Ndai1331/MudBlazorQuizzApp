namespace QuizAppBlazor.Client.HttpResonpse
{
    public class ResponseBaseHttp<T>
    {
        public T Result { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}
