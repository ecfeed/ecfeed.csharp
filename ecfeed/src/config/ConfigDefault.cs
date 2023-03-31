// # define DEVELOP

using System;

namespace EcFeed
{
    public static class RequestTestType
    {
        public const string Data = "requestData";
        public const string Export = "requestExport";
    }

    public static class RequestTestParameter
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

    public static class RequestFeedbackBody
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

    public static class RequestEndpoint
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
            internal const string GeneratorAddress = "https://gen.ecfeed.com";
        #endif

        internal const string KeyStorePassword = "changeit";

        internal static readonly string[] KeyStorePath =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.ecfeed/security.p12",
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/ecfeed/security.p12",
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