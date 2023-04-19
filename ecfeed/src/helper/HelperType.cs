using System.Reflection;

namespace EcFeed
{
    static class HelperType
    {
        public static string GetValue(this Template template)
        {
            switch (template)
            {
                case Template.JSON: return "JSON";
                case Template.CSV: return "CSV";
                case Template.Gherkin: return "Gherkin";
                case Template.XML: return "XML";
                case Template.Stream: return "Stream";
            }
            
            throw new TestProviderException("Invalid export type");
        }

        public static string GetValue(this Generator generator)
        {
            switch (generator)
            {
                case Generator.Cartesian: return "genCartesian";
                case Generator.NWise: return "genNWise";
                case Generator.Random: return "genRandom";
                case Generator.Static: return "static";
            }

            throw new TestProviderException("Invalid generator type");
        }

        internal static string ParseConstructorParameter(ParameterInfo parameter) 
        {
            var name = parameter.ParameterType.Name;

            switch(name)
            {
                case "Byte": return "byte";
                case "Int16": return "short";
                case "Int32": return "int";
                case "Int64": return "long";
                case "Single": return "float";
                case "Double": return "double";
                case "Char": return "char";
                case "String": return "String";
            }

            return name;
        }

        internal static bool IsPrimitive(string typeName) 
        {
            switch (typeName) 
            {
                case "byte":
                case "short":
                case "int":
                case "long":
                case "float":
                case "double":
                case "boolean":
                case "char":
                case "String":
                    return true;
                default:
                    return false; 
            }
        }
    }
}