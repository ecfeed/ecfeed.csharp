using System;

namespace EcFeed
{
    public enum Template
    {
        JSON, CSV, Gherkin, XML, Stream, StreamRaw
    }

    public enum Generator
    {
        Cartesian, NWise, Random, Static
    }

    static class EnumExtensions
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
                case Template.StreamRaw: return "StreamRaw";

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

    public static class Parameter
    {
        public const string DataSource = "dataSource";
        public const string N = "n";
        public const string Coverage = "coverage";
        public const string Properties = "properties";
        public const string Length = "length";
        public const string Duplicates = "duplicates";
        public const string TestSuites = "testSuites";
        public const string Constraints = "constraints";
        public const string Arguments = "arguments";
        public const string Choices = "choices";
        public const string Adaptive = "adaptive";
    }

    public static class Request
    {
        public const string Data = "requestData";
        public const string Export = "requestExport";
    }

    public static class Endpoint
    {
        internal const string HealthCheck = "genServiceVersion";
        internal const string Generator = "testCaseService";
    }

    static class Default
    {
    // Production

    // internal const string GeneratorAddress = "https://gen.ecfeed.com";
    // internal const string CertificateHash = "AAE72557A7DB47EA4CF4C649108E422528EFDA1B";

    // Development

        internal const string KeyStorePassword = "changeit";
        internal const string GeneratorAddress = "https://develop-gen.ecfeed.com";
        internal const string CertificateHash = "FD3D44720A70F2A22454AAA0B3F1E8AE6FC0D84E";
        internal static readonly string[] KeyStorePath =
        {
            Environment.GetEnvironmentVariable("HOME") + "/.ecfeed/security.p12",
            Environment.GetEnvironmentVariable("HOME") + "/ecfeed/security.p12",
            "./security.p12"
        };
        
        internal const string ParameterTestSuite = "ALL";
        internal const bool ParameterDuplicates = true;
        internal const bool ParameterAdaptive = false;
        internal const int ParameterN = 2;
        internal const int ParameterCoverage = 100;
        internal const int ParameterLength = 10;

        internal const string Template = "JSON";
    }    
}