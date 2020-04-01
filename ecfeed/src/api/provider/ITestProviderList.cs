namespace EcFeed
{
    public interface ITestProviderList
    {
        TestList List();
        
        TestList ListCartesian();
        
        TestList ListNWise(
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage);
        
        TestList ListRandom(
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates);
        
        TestList ListStatic(
            object testSuites = null);
    }
}