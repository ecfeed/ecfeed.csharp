using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EcFeed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)] public sealed class TestData
    {
        [JsonIgnore] private readonly SessionData sessionData;
        [JsonIgnore] private readonly string id;
        [JsonProperty("data")] private string data;
        [JsonProperty("status")] private string status;
        [JsonProperty("duration")] private int? duration;
        [JsonProperty("comment")] private string comment;
        [JsonProperty("custom")] private Dictionary<string, string> custom;

        internal TestData(SessionData sessionData, string data, string id)
        {
            this.sessionData = sessionData;
            this.data = data;
            this.id = id;
        }

        public void register(bool status, int? duration = null, string comment = null, Dictionary<string, string> custom = null)
        {
            this.status = status ? "P" : "F";
            this.duration = duration;
            this.comment = comment;
            this.custom = custom;

            sessionData.AddTest(id, this);
        }

    }
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)] internal sealed class SessionData
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
        [JsonProperty("testResults")] internal Dictionary<string, TestData> TestResults { get; set; }
        [JsonProperty("testSessionLabel")] internal string TestSessionLabel { get; set; }
        [JsonProperty("framework")] internal string Framework { get; set; }
        [JsonProperty("timestamp")] internal int Timestamp { get; set; }
        [JsonProperty("custom")] internal Dictionary<string, string> Custom { get; set; }

        internal SessionData()
        {
            Framework = "C#";
            Template = Template.Stream;
            TestResults = new Dictionary<string, TestData>();
            TransmissionFinished = false;
        }

        public void AddTest(string id, TestData testData)
        {
            TestCasesParsed++;
            TestResults.Add(id, testData);

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
    internal sealed class GeneratorData
    {
        internal Dictionary<string, object> UserData { get; set; }
        internal GeneratorPropertiesData Properties { get; set; }

        internal GeneratorData(GeneratorPropertiesData properties = null)
        {
            UserData = new Dictionary<string, object>();
            Properties = properties == null ? new GeneratorPropertiesData() : properties;
        }

        internal object Get(string key)
        {
            return UserData[key];
        }
        internal object Set(string key, object value)
        {
            
            if (UserData.ContainsKey(key))
            {
                object previous = UserData[key];
                UserData[key] = value;
                return previous;
            }

            UserData.Add(key, value);
            return null;
        }

        internal GeneratorData Merge(GeneratorData settings)
        {
            GeneratorData settingsTo = new GeneratorData();
            GeneratorData settingsFrom = settings == null ? new GeneratorData() : settings;

            this.UserData.ToList().ForEach(x => settingsTo.Set(x.Key, x.Value));
            settingsFrom.UserData.ToList().ForEach(x => settingsTo.Set(x.Key, x.Value));

            settingsTo.Properties = settingsTo.Properties.Merge(settings.Properties);

            return settingsTo;
        }

        public string List()
        {
            return string.Join(", ", UserData.ToList().Select(x => x.Key + " = " + x.Value));
        }
        
        public override string ToString()
        {
            GeneratorData settings = new GeneratorData();
            this.UserData.ToList().ForEach(x => settings.Set(x.Key, x.Value));
            settings.Set("properties", Properties.Properties);

            return JsonConvert.SerializeObject(settings.UserData, Formatting.None)
                .Replace("\"", "\'")
                .Replace("':'True'", "':'true'")
                .Replace("':'False'", "':'false'");
        }
    } 
    internal sealed class GeneratorPropertiesData
    {
        internal Dictionary<string, object> Properties;

        internal GeneratorPropertiesData()
        {
            Properties = new Dictionary<string, object>();
        }

        internal object Set(string key, object value)
        {
            if (Properties.ContainsKey(key))
            {
                object previous = Properties[key];
                Properties[key] = value;
                return previous;
            }

            Properties.Add(key, value);

            return null;
        }

        internal GeneratorPropertiesData Merge(GeneratorPropertiesData settings)
        {
            GeneratorPropertiesData settingsTo = new GeneratorPropertiesData();
            GeneratorPropertiesData settingsFrom = settings == null ? new GeneratorPropertiesData() : settings;

            this.Properties.ToList().ForEach(x => settingsTo.Set(x.Key, x.Value));
            settingsFrom.Properties.ToList().ForEach(x => settingsTo.Set(x.Key, x.Value));

            return settingsTo;
        }

        public string List()
        {
            return string.Join(", ", Properties.ToList().Select(x => x.Key + " = " + x.Value));
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(Properties, Formatting.None)
                .Replace("\"", "\'")
                .Replace("':'True'", "':'true'")
                .Replace("':'False'", "':'false'");
        }
    } 
}
    