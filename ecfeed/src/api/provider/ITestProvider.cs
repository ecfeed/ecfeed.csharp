using System;
using System.Collections.Generic;

namespace EcFeed
{
    public interface ITestProvider : ITestProviderGenerator, ITestProviderQueue, ITestProviderList
    {
        ITestProvider Copy();

        string KeyStorePath { get; set; }
        string KeyStorePassword { get; set; }
        string CertificateHash { get; set; }
        string GeneratorAddress { get; set; }
        string Model { get; set; }
        string Method { get; set; }
        Dictionary<string, object> Settings { get; set; }

        void ValidateConnectionSettings();

        void AddTestEventHandler(EventHandler<TestEventArgs> testEventHandler);
        void RemoveTestEventHandler(EventHandler<TestEventArgs> testEventHandler);
        void AddStatusEventHandler(EventHandler<StatusEventArgs> testEventHandler);
        void RemoveStatusEventHandler(EventHandler<StatusEventArgs> testEventHandler);
    }
}