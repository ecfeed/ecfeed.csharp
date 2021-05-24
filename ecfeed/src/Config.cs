// # define DEVELOP

using System;

namespace EcFeed
{
    public enum Template
    {
        JSON, CSV, Gherkin, XML, Stream
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

    public static class Feedback
    {
        public const string GeneratorType = "generatorType";
        public const string GeneratorOptions = "generatorOptions";
        public const string ModelId = "modelId";
        public const string TestSessionId = "testSessionId";
        public const string MethodInfo = "methodInfo";
        public const string TestResults = "testResults";
        public const string TestSessionLabel = "testSessionLabel";
        public const string Framework = "framework";
        public const string Timestamp = "timestamp";
        public const string Custom = "custom";
        public const string Data = "data";
        public const string Status = "status";
        public const string StatusPassed = "P";
        public const string StatusFailed = "F";
        public const string Duration = "duration";
        public const string Comment = "comment";
        public const string TestSuites = "testSuites";
        public const string Constraints = "constraints";
        public const string Choices = "choices";
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
        internal const string Feedback = "streamFeedback";
    }

    static class Default
    {
        #if DEVELOP
            internal const string GeneratorAddress = "https://develop-gen.ecfeed.com";
        #else
            internal const string GeneratorAddress = "https://develop-gen.ecfeed.com";
        #endif

        internal const string KeyStorePassword = "changeit";

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

        internal const Template ParameterTemplate = Template.CSV;
    }    
}