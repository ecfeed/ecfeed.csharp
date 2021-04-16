using System.Collections.Generic;

namespace EcFeed
{
    public class Feedback
    {
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
        public string GeneratorType { get; set; }
        public string GeneratorOptions { get; set; }
        public object Constraints { get; set; }
        public object Choices { get; set; }
        public object TestSuites { get; set; }
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