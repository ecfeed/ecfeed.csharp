using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EcFeed
{
    internal sealed class DataGeneratorProperties
    {
        internal Dictionary<string, object> Properties;

        internal DataGeneratorProperties()
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

        internal DataGeneratorProperties Merge(DataGeneratorProperties settings)
        {
            DataGeneratorProperties settingsTo = new DataGeneratorProperties();
            DataGeneratorProperties settingsFrom = settings == null ? new DataGeneratorProperties() : settings;

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
    