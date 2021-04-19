using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcFeed
{
    public class TestData
    {
        private readonly SessionData sessionData;
        private readonly string data;
        private readonly string id;

        internal TestData(SessionData sessionData, string data, string id)
        {
            this.sessionData = sessionData;
            this.data = data;
            this.id = id;
        }

        public void register(bool status, int? duration = null, string comment = null, Dictionary<string, string> custom = null)
        {
            sessionData.AddTest(id, data, status, duration, comment, custom);
        }

    }
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class SessionTestData
    {
        [JsonProperty] private readonly string data;
        [JsonProperty] private readonly string status;
        [JsonProperty] private readonly int? duration;
        [JsonProperty] private readonly string comment;
        [JsonProperty] private readonly Dictionary<string, string> custom;

        internal SessionTestData(string data, string status, int? duration, string comment, Dictionary<string, string> custom)
        {
            this.data = data;
            this.status = status;
            this.duration = duration;
            this.comment = comment;
            this.custom = custom;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class SessionData
    {   
        [JsonIgnore] private GeneratorData _generatorData;
        [JsonIgnore] internal GeneratorData GeneratorData 
        { 
            get
            {
                return _generatorData;
            }
            set
            {
                GeneratorType = (string)value.Get(Parameter.DataSource);
                GeneratorOptions = value.Properties.List();
                _generatorData = value;
            } 
        }
        [JsonIgnore] internal bool SendFeedback { get; set; }
        [JsonIgnore] internal Template Template { get; set; }
        [JsonIgnore] internal string MethodName { get; set; }
        [JsonIgnore] internal string[] MethodArgumentTypes { get; set; }
        [JsonIgnore] internal string[] MethodArgumentNames { get; set; }
        [JsonIgnore] internal int TestCasesTotal { get; set; }
        [JsonIgnore] internal int TestCasesParsed { get; set; }
        [JsonIgnore] internal bool TransmissionFinished { get; set; }
        [JsonProperty("generatorType")] internal string GeneratorType { get; set; }
        [JsonProperty("generatorOptions")] internal string GeneratorOptions { get; set; }
        [JsonProperty("modelId")] internal string ModelId { get; set; }
        [JsonProperty("testSessionId")] internal string TestSessionId { get; set; }
        [JsonProperty("methodInfo")] internal string MethodNameQualified { get; set; }
        [JsonProperty("testResults")] internal Dictionary<string, SessionTestData> TestResults { get; set; }
        [JsonProperty("testSessionLabel")] internal string TestSessionLabel { get; set; }
        [JsonProperty("framework")] internal string Framework { get; set; }
        [JsonProperty("timestamp")] internal int Timestamp { get; set; }
        [JsonProperty("custom")] internal Dictionary<string, string> Custom { get; set; }

        internal SessionData()
        {
            Framework = "C#";
            Template = Template.Stream;
            TestResults = new Dictionary<string, SessionTestData>();
            TransmissionFinished = false;
        }

        public void AddTest(string id, string data, bool status, int? duration, string comment, Dictionary<string, string> custom)
        {
            TestCasesParsed++;
            TestResults.Add(id, new SessionTestData(data, status ? "P" : "F", duration, comment, custom));

            if (TestCasesParsed == TestCasesTotal && TransmissionFinished)
            {
                LastResort();
            }
        }

        internal string IncrementTestCasesTotal()
        {
            return "0:" + TestCasesTotal++;
        }

        internal void FinishTransmission()
        {
            TransmissionFinished = true;

            if (TestCasesParsed == TestCasesTotal)
            {
                LastResort();
            }
        }

        internal void LastResort()
        {
            Console.WriteLine(ToString());
            Console.WriteLine("End of stream");
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
    
}