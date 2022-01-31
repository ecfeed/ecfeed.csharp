// # define DEVELOP

using System;

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
                case Template.Raw: return "Stream";
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
    }
}