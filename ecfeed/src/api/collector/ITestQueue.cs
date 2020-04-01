using System.Collections.Generic;
using NUnit.Framework;

namespace EcFeed
{
    public interface ITestQueue : IEnumerable<TestCaseData>
    {
        int Size();
    }
}