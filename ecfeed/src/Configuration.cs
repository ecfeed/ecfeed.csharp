using System;

namespace EcFeed
{
    public static class Template
    {
        public const string JSON = "JSON";
        public const string CSV = "CSV";
        public const string Gherkin = "Gherkin";
        public const string XML = "XML";
        public const string Stream = "Stream";
        public const string StreamRaw = "StreamRaw";
    }   

    public static class Generator
    {
        public const string Cartesian = "genCartesian";
        public const string NWise = "genNWise";
        public const string Random = "genRandom";
        public const string Static = "static";
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
        internal const int ParameterN = 2;
        internal const int ParameterCoverage = 100;
        internal const int ParameterLength = 10;

        internal const string Template = EcFeed.Template.JSON;
    }    
}