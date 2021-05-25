using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EcFeed
{
    internal sealed class DataGenerator
    {
        internal Dictionary<string, object> UserData { get; set; }
        internal DataGeneratorProperties Properties { get; set; }

        internal DataGenerator(DataGeneratorProperties properties = null)
        {
            UserData = new Dictionary<string, object>();
            Properties = properties == null ? new DataGeneratorProperties() : properties;
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

        internal DataGenerator Merge(DataGenerator settings)
        {
            DataGenerator settingsTo = new DataGenerator();
            DataGenerator settingsFrom = settings == null ? new DataGenerator() : settings;

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
            DataGenerator settings = new DataGenerator();
            this.UserData.ToList().ForEach(x => settings.Set(x.Key, x.Value));
            settings.Set("properties", Properties.Properties);

            return JsonConvert.SerializeObject(settings.UserData, Formatting.None)
                .Replace("\"", "\'")
                .Replace("':'True'", "':'true'")
                .Replace("':'False'", "':'false'");
        }
    } 
}
    