using Newtonsoft.Json;
using System.Linq;

namespace EcFeed
{
    public struct InfoMessage
    {
        [JsonProperty("info", Required = Required.Always)] public string Info { get; set; }

        public override string ToString() => $"Info: { Info }";
    }

    static class InfoMessageHelper 
    { 
        internal static string[] ExtractTypes(InfoMessage schema)
        {
            int leftBracket = schema.Info.IndexOf("(");
            int rightBracket = schema.Info.IndexOf(")");
            string argumentsString = schema.Info.Substring(leftBracket + 1, (rightBracket - leftBracket));
            
            return argumentsString.Split(",").Select(argument => argument.Trim().Split(" ")[0]).ToArray();
        }
    }
}