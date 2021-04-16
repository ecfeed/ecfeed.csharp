using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcFeed
{
    internal sealed class GeneratorOptions
    {
        internal Dictionary<string, object> UserData { get; set; }
        internal GeneratorProperties Properties { get; set; }

        internal GeneratorOptions(GeneratorProperties properties = null)
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

        internal GeneratorOptions Merge(GeneratorOptions settings)
        {
            GeneratorOptions settingsTo = new GeneratorOptions();
            GeneratorOptions settingsFrom = settings == null ? new GeneratorOptions() : settings;

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
            GeneratorOptions settings = new GeneratorOptions();
            this.UserData.ToList().ForEach(x => settings.Add(x.Key, x.Value));
            settings.Add("properties", Properties.Properties);

            return JsonConvert.SerializeObject(settings.UserData, Formatting.None)
                .Replace("\"", "\'")
                .Replace("':'True'", "':'true'")
                .Replace("':'False'", "':'false'");
        }
    } 

}