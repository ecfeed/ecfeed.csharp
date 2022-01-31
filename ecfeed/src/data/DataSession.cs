using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace EcFeed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)] internal sealed class DataSession
    {   
        [JsonIgnore] private DataGenerator _generatorData;
        [JsonIgnore] internal DataGenerator GeneratorData 
        { 
            get
            {
                return _generatorData;
            }
            set
            {
                GeneratorType = (string)value.Get(RequestTestParameter.DataSource);
                GeneratorOptions = value.Properties.List();
                _generatorData = value;
            } 
        }
        [JsonIgnore] internal string KeyStorePath { get; set; }
        [JsonIgnore] internal string KeyStorePassword { get; set; }
        [JsonIgnore] internal string GeneratorAddress { get; set; }
        [JsonIgnore] internal bool BuildFeedback { get; set; }
        [JsonIgnore] internal Template Template { get; set; }
        [JsonIgnore] internal string MethodName { get; set; }
        [JsonIgnore] internal string[] MethodArgumentTypes { get; set; }
        [JsonIgnore] internal string[] MethodArgumentNames { get; set; }
        [JsonIgnore] internal int TestCasesTotal { get; set; }
        [JsonIgnore] internal int TestCasesParsed { get; set; }
        [JsonIgnore] internal bool TransmissionFinished { get; set; }
        [JsonProperty(RequestFeedbackBody.GeneratorType)] internal string GeneratorType { get; set; }
        [JsonProperty(RequestFeedbackBody.GeneratorOptions)] internal string GeneratorOptions { get; set; }
        [JsonProperty(RequestFeedbackBody.ModelId)] internal string ModelId { get; set; }
        [JsonProperty(RequestFeedbackBody.TestSessionId)] internal string TestSessionId { get; set; }
        [JsonProperty(RequestFeedbackBody.MethodInfo)] internal string MethodNameQualified { get; set; }
        [JsonProperty(RequestFeedbackBody.TestResults)] internal Dictionary<string, TestHandle> TestResults { get; set; }
        [JsonProperty(RequestFeedbackBody.TestSessionLabel)] internal string TestSessionLabel { get; set; }
        [JsonProperty(RequestFeedbackBody.Framework)] internal string Framework { get; set; }
        [JsonProperty(RequestFeedbackBody.Timestamp)] internal int Timestamp { get; set; }
        [JsonProperty(RequestFeedbackBody.Custom)] internal Dictionary<string, string> Custom { get; set; }
        [JsonProperty(RequestFeedbackBody.TestSuites)] internal Object TestSuites { get; set; }
        [JsonProperty(RequestFeedbackBody.Constraints)] internal Object Constraints { get; set; }
        [JsonProperty(RequestFeedbackBody.Choices)] internal Object Choices { get; set; }

        internal DataSession()
        {
            Framework = "C#";
            Template = Template.Raw;
            TestResults = new Dictionary<string, TestHandle>();
            TransmissionFinished = false;
        }

        public void AddTest(string id, TestHandle testData)
        {
            TestCasesParsed++;
            TestResults.Add(id, testData);

            if (TestCasesParsed == TestCasesTotal && TransmissionFinished)
            {
                SendFeedback();
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
                SendFeedback();
            }
        }

        internal void SendFeedback()
        {
            if (BuildFeedback)
            {
                string data = RequestHelper.GenerateFeedbackURL(this, GeneratorAddress);
                RequestHelper.SendRequest(data, KeyStorePath, KeyStorePassword, ToString());
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }  
}
    