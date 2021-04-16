using System.Collections.Generic;

namespace EcFeed
{
    public class SessionData
    {
        public SessionData()
        {
            Framework = "C#";
            Template = Template.Stream;
            TestResults = new Dictionary<string, FeedbackTest>();
        }
        
        public bool Feedback { get; set; }
        public Template Template { get; set; }
        public string ModelId { get; set; }
        public string TestSessionId { get; set; }
        public string MethodName { get; set; }
        public string MethodNameQualified { get; set; }
        public string[] MethodArgumentTypes { get; set; }
        public string[] MethodArgumentNames { get; set; }
        public Dictionary<string, FeedbackTest> TestResults { get; set; }
        public string TestSessionLabel { get; set; }
        public string Framework { get; set; }
        public int Timestamp { get; set; }
        internal GeneratorOptions GeneratorOptions { get; set; }
        public Dictionary<string, string> Custom { get; set; }
    }

    public class FeedbackTest
    {
        public string Data { get; set; }
        public string Status { get; set; }
        public int Duration { get; set; }
        public string Comment { get; set; }
        public Dictionary<string, string> Custom { get; set; }
    }
    
}