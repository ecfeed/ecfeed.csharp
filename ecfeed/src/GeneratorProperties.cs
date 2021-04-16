using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcFeed
{
    internal sealed class GeneratorProperties
    {
        internal Dictionary<string, object> Properties;

        internal GeneratorProperties()
        {
            Properties = new Dictionary<string, object>();
        }

        internal void Add(string key, object value)
        {
            if (Properties.ContainsKey(key))
            {
                Properties[key] = value;
                return;
            }

            Properties.Add(key, value);
        }

        internal GeneratorProperties Merge(GeneratorProperties settings)
        {
            GeneratorProperties settingsTo = new GeneratorProperties();
            GeneratorProperties settingsFrom = settings == null ? new GeneratorProperties() : settings;

            this.Properties.ToList().ForEach(x => settingsTo.Add(x.Key, x.Value));
            settingsFrom.Properties.ToList().ForEach(x => settingsTo.Add(x.Key, x.Value));

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