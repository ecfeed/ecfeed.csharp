using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcFeed
{
    public sealed class GeneratorOptionsProperties
    {
        [JsonProperty("properties")] 
        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        public void AddProperty(string key, object value)
        {
            if (_properties.ContainsKey(key))
            {
                _properties[key] = value;
                return;
            }

            _properties.Add(key, value);
        }

        public void RemoveProperty(string key)
        {
            _properties.Remove(key);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(_properties, Formatting.None);
        }
    } 
}