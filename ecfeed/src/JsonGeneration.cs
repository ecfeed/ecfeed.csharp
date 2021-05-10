using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;

namespace EcFeed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)] public sealed class TestHandle
    {
        [JsonIgnore] private readonly SessionData sessionData;
        [JsonIgnore] private readonly string id;
        [JsonIgnore] private bool pending;
        [JsonProperty(Feedback.Data)] private string data;
        [JsonProperty(Feedback.Status)] private string status;
        [JsonProperty(Feedback.Duration)] private int? duration;
        [JsonProperty(Feedback.Comment)] private string comment;
        [JsonProperty(Feedback.Custom)] private Dictionary<string, string> custom;

        internal TestHandle(SessionData sessionData, string data, string id)
        {
            this.sessionData = sessionData;
            this.data = data;
            this.id = id;
            this.pending = true;
        }

        ~TestHandle()
        {
            Console.WriteLine("OKO");
        }

        public string addFeedback(bool status, int? duration = null, string comment = null, Dictionary<string, string> custom = null)
        {
            if (this.pending)
            {
                this.status = status ? Feedback.StatusPassed : Feedback.StatusFailed;
                this.duration = duration;
                this.comment = comment;
                this.custom = custom;

                sessionData.AddTest(id, this);

                this.pending = false;
            }

            return comment != null ? comment : "feedback";
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
        [JsonIgnore] internal string KeyStorePath { get; set; }
        [JsonIgnore] internal string KeyStorePassword { get; set; }
        [JsonIgnore] internal bool BuildFeedback { get; set; }
        [JsonIgnore] internal Template Template { get; set; }
        [JsonIgnore] internal string MethodName { get; set; }
        [JsonIgnore] internal string[] MethodArgumentTypes { get; set; }
        [JsonIgnore] internal string[] MethodArgumentNames { get; set; }
        [JsonIgnore] internal int TestCasesTotal { get; set; }
        [JsonIgnore] internal int TestCasesParsed { get; set; }
        [JsonIgnore] internal bool TransmissionFinished { get; set; }
        [JsonProperty(Feedback.GeneratorType)] internal string GeneratorType { get; set; }
        [JsonProperty(Feedback.GeneratorOptions)] internal string GeneratorOptions { get; set; }
        [JsonProperty(Feedback.ModelId)] internal string ModelId { get; set; }
        [JsonProperty(Feedback.TestSessionId)] internal string TestSessionId { get; set; }
        [JsonProperty(Feedback.MethodInfo)] internal string MethodNameQualified { get; set; }
        [JsonProperty(Feedback.TestResults)] internal Dictionary<string, TestHandle> TestResults { get; set; }
        [JsonProperty(Feedback.TestSessionLabel)] internal string TestSessionLabel { get; set; }
        [JsonProperty(Feedback.Framework)] internal string Framework { get; set; }
        [JsonProperty(Feedback.Timestamp)] internal int Timestamp { get; set; }
        [JsonProperty(Feedback.Custom)] internal Dictionary<string, string> Custom { get; set; }

        internal SessionData()
        {
            Framework = "C#";
            Template = Template.Stream;
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
                string data = RequestHelper.GenerateFeedbackURL(this, Default.GeneratorAddress);
                RequestHelper.SendRequest(data, KeyStorePath, KeyStorePassword, ToString());
            }
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
    