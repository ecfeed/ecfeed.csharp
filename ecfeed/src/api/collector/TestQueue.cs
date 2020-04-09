using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace EcFeed
{
    internal sealed class TestQueue<T> : IEnumerable<T>
    {
        private BlockingCollection<T> _fifo = new BlockingCollection<T>();

        internal int Count
        {
            get => _fifo.Count;
        }

        internal TestQueue(TestProvider testProvider, GeneratorOptions options, Template template, string method)
        {
            testProvider.AddTestEventHandler(TestEventHandler);
            testProvider.AddStatusEventHandler(StatusEventHandler);
            
            testProvider.StartQueue(method, options, template);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (!_fifo.IsCompleted)
            {
                T element = default(T);

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

        private void TestEventHandler(object sender, DataEventArgs args)
        {
            if ( args.DataRaw != null && args.DataRaw.GetType() == typeof(T))
            {
                _fifo.Add((T)(object)args.DataRaw);
                return;
            }
            
            if ( args.DataObject != null && args.DataObject.GetType() == typeof(T))
            {
                _fifo.Add((T)(object)args.DataObject);
                return;
            }
            
            throw new TestProviderException("Unknown type.");
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