using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcFeed
{
    public sealed class GeneratorProperties
    {
        internal Dictionary<string, object> Properties = new Dictionary<string, object>();

        public object GetProperty(string key)
        {
            return Properties[key];
        }

        public void AddProperty(string key, object value)
        {
            if (Properties.ContainsKey(key))
            {
                Properties[key] = value;
                return;
            }

            Properties.Add(key, value);
        }

        public void RemoveProperty(string key)
        {
            Properties.Remove(key);
        }

        internal GeneratorProperties MergeInto(GeneratorProperties settings)
        {
            GeneratorProperties settingsTo = new GeneratorProperties();
            GeneratorProperties settingsFrom = settings == null ? new GeneratorProperties() : settings;

            this.Properties.ToList().ForEach(x => settingsTo.AddProperty(x.Key, x.Value));
            settingsFrom.Properties.ToList().ForEach(x => settingsTo.AddProperty(x.Key, x.Value));

            return settingsTo;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(Properties, Formatting.None).Replace("\"", "\'");;
        }
    } 
}