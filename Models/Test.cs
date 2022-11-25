namespace QuestionApi.Models
{
    public class Test
    {
        public string lmsData { get; set; }
        public MsgData msgData { get; set; }
    }

    public class LmsData
    {
        public string messageid { get; set; }
    }

    public class MsgData
    {
        public string config { get; set; }
        public string assignment { get; set; }
    }
}