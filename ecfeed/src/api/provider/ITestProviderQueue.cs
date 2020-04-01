namespace EcFeed
{
    public interface ITestProviderQueue
    {
        TestQueue Queue();
        
        TestQueue QueueCartesian();
        
        TestQueue QueueNWise(
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage);
        
        TestQueue QueueRandom(
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates);
        
        TestQueue QueueStatic(
            object testSuites = null);
    }
}