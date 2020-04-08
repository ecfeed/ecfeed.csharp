using System;
using System.Collections.Generic;

namespace EcFeed
{
    public interface ITestProvider : ITestProviderQueue
    {
        // ITestProvider Copy();

        // string KeyStorePath { get; set; }
        // string KeyStorePassword { get; set; }
        // string CertificateHash { get; set; }
        // string GeneratorAddress { get; set; }
        string Model { get; set; }

        // Dictionary<string, object> Settings { get; set; }

        void ValidateConnectionSettings();

        // void AddTestEventHandler(EventHandler<TestEventArgs> testEventHandler);
        // void RemoveTestEventHandler(EventHandler<TestEventArgs> testEventHandler);
        // void AddStatusEventHandler(EventHandler<StatusEventArgs> testEventHandler);
        // void RemoveStatusEventHandler(EventHandler<StatusEventArgs> testEventHandler);

        // IEnumerable<string> Generatex(
        //     string method,
        //     string template = Default.Template);
        
        // IEnumerable<string> GenerateCartesianx(
        //     string method,
        //     string template = Default.Template);
        
        // IEnumerable<string> GenerateNWisex(
        //     string method,
        //     string template = Default.Template,
        //     int n = Default.ParameterN, 
        //     int coverage = Default.ParameterCoverage);
        
        // IEnumerable<string> GenerateRandomx(
        //     string method,
        //     string template = Default.Template,
        //     int length = Default.ParameterLength, 
        //     bool duplicates = Default.ParameterDuplicates);
        
        // IEnumerable<string> GenerateStaticx(
        //     string method,
        //     string template = Default.Template,
        //     object testSuites = null);
    }
}