using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcFeed
{
    public sealed class GeneratorOptions
    {
        [JsonProperty("userData")] 
        private Dictionary<string, object> _userData = new Dictionary<string, object>();

        private GeneratorOptionsProperties Properties { get; set; }

        public GeneratorOptions(GeneratorOptionsProperties properties = null)
        {
            Properties = properties == null ? new GeneratorOptionsProperties() : properties;
        }

        public void AddProperty(string key, object value)
        {
            if (_userData.ContainsKey(key))
            {
                _userData[key] = value;
                return;
            }

            _userData.Add(key, value);
        }

        public void RemoveProperty(string key)
        {
            _userData.Remove(key);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(_userData, Formatting.None);
        }
    } 

}