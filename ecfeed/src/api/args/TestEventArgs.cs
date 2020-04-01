using System;
using NUnit.Framework;

namespace EcFeed
{
    public sealed class TestEventArgs : EventArgs
    {
        public TestCase Schema { get; set; }
        public string DataRaw { get; set; }
        public object[] DataObject { get; set; }
        public TestCaseData DataTest { get; set; }

        public override string ToString()
        {
            return 
                $"TestEventArgs:\n" + 
                $"\t[string: { !string.IsNullOrEmpty(DataRaw) }]\n" +
                $"\t[struct: { Schema.TestCaseArguments != null }]\n" +
                $"\t[object: { DataObject != null }]";
        }
    } 
}