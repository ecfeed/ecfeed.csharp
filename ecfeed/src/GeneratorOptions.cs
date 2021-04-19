using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcFeed
{
    internal sealed class GeneratorData
    {
        internal Dictionary<string, object> UserData { get; set; }
        internal GeneratorProperties Properties { get; set; }

        internal GeneratorData(GeneratorProperties properties = null)
        {
            UserData = new Dictionary<string, object>();
            Properties = properties == null ? new GeneratorProperties() : properties;
        }

        internal object Get(string key)
        {
            return UserData[key];
        }

        internal void Add(string key, object value)
        {
            if (value == null)
            {
                return;
            }
            
            if (UserData.ContainsKey(key))
            {
                UserData[key] = value;
                return;
            }

            UserData.Add(key, value);
        }

        internal GeneratorData Merge(GeneratorData settings)
        {
            GeneratorData settingsTo = new GeneratorData();
            GeneratorData settingsFrom = settings == null ? new GeneratorData() : settings;

            this.UserData.ToList().ForEach(x => settingsTo.Add(x.Key, x.Value));
            settingsFrom.UserData.ToList().ForEach(x => settingsTo.Add(x.Key, x.Value));

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
            this.UserData.ToList().ForEach(x => settings.Add(x.Key, x.Value));
            settings.Add("properties", Properties.Properties);

            return JsonConvert.SerializeObject(settings.UserData, Formatting.None)
                .Replace("\"", "\'")
                .Replace("':'True'", "':'true'")
                .Replace("':'False'", "':'false'");
        }
    } 

}