namespace QuestionApi.Models
{
    public class CreateTest
    {
        public int count { get; set; }
        public Question[] questions { get; set; }
    }

    public class Question
    {
        public string test { get; set; }
        public string questionNo { get; set; }
    }
}