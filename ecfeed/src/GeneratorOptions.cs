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

        internal object GetOption(string key)
        {
            return UserData[key];
        }

        internal void AddOption(string key, object value)
        {
            if (UserData.ContainsKey(key))
            {
                UserData[key] = value;
                return;
            }

            UserData.Add(key, value);
        }

        internal void RemoveOption(string key)
        {
            UserData.Remove(key);
        }

        internal GeneratorOptions MergeInto(GeneratorOptions settings)
        {
            GeneratorOptions settingsTo = new GeneratorOptions();
            GeneratorOptions settingsFrom = settings == null ? new GeneratorOptions() : settings;

            this.UserData.ToList().ForEach(x => settingsTo.AddOption(x.Key, x.Value));
            settingsFrom.UserData.ToList().ForEach(x => settingsTo.AddOption(x.Key, x.Value));

            settingsTo.Properties = settingsTo.Properties.MergeInto(settings.Properties);

            return settingsTo;
        }

        public override string ToString()
        {
            GeneratorOptions settings = new GeneratorOptions();
            this.UserData.ToList().ForEach(x => settings.AddOption(x.Key, x.Value));
            settings.AddOption("properties", Properties.Properties);

            return JsonConvert.SerializeObject(settings.UserData, Formatting.None)
                .Replace("\"", "\'")
                .Replace("':'True'", "':'true'")
                .Replace("':'False'", "':'false'");
        }
    } 

}