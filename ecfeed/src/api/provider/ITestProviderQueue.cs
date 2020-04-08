using System.Collections.Generic;

namespace EcFeed
{
    public interface ITestProviderQueue
    {
        IEnumerable<object[]> Generate(
            string method,
            Generator generator);
        
        IEnumerable<object[]> GenerateCartesian(
            string method);
        
        IEnumerable<object[]> GenerateNWise(
            string method,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage);
        
        IEnumerable<object[]> GenerateRandom(
            string method,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates);
        
        IEnumerable<object[]> GenerateStatic(
            string method,
            object testSuites = null);
    }
}