using System.Threading.Tasks;

namespace EcFeed
{
    public interface ITestProviderGenerator
    {
        Task<string> Generate(
            string template = Default.Template);
        
        Task<string> GenerateCartesian(
            string template = Default.Template);
        
        Task<string> GenerateNWise(
            string template = Default.Template,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage);
        
        Task<string> GenerateRandom(
            string template = Default.Template,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates);
        
        Task<string> GenerateStatic(
            string template = Default.Template,
            object testSuites = null);
    }
}