using System;

namespace EcFeed
{
    internal sealed class DataEventArgs
    {
        public TestCase Schema { get; set; }
        public string DataRaw { get; set; }
        public object[] DataObject { get; set; }

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