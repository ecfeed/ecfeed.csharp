using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace EcFeed
{
    internal sealed class TestQueue : IEnumerable<object[]>
    {
        private BlockingCollection<object[]> _fifo = new BlockingCollection<object[]>();

        public int Count
        {
            get => _fifo.Count;
        }

        public int Size()
        {
            return Count;
        }

        internal TestQueue(TestProvider testProvider, GeneratorOptions options, string method)
        {
            testProvider.AddTestEventHandler(TestEventHandler);
            testProvider.AddStatusEventHandler(StatusEventHandler);
            
            Execute(testProvider, options, method);
        }

        private async void Execute(TestProvider testProvider, GeneratorOptions options, string method)
        {
            await testProvider.GenerateExecute(method, options);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            while (!_fifo.IsCompleted)
            {
                object[] element = null;

                try
                {
                    element = _fifo.Take();
                }
                catch (InvalidOperationException) { }

                if (element != null)
                {
                    yield return element;
                }
            }
        }

        private void TestEventHandler(object sender, TestEventArgs args)
        {
            _fifo.Add(args.DataObject);
        }

        private void StatusEventHandler(object sender, StatusEventArgs args)
        {
            if (args.IsCompleted)
            {
                _fifo.CompleteAdding();
            }
        }

        public override string ToString()
        {
            string progress = _fifo.IsCompleted ? "Completed" : "In progress";
            return $"Remaining cases: { _fifo.Count }, Generation status: { progress }.";
        }

    }
}