using System.Collections.Generic;

namespace EcFeed
{
    public interface ITestList : IEnumerable<TestEventArgs>
    {
        int Size();
        void WaitUntilFinished();
        TestEventArgs this[int index] {get; set;}
    }
}