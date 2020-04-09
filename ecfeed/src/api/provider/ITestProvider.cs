using System;
using System.Collections.Generic;

namespace EcFeed
{
    public interface ITestProvider
    {
        string Model { get; set; }

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

        IEnumerable<object[]> Generate(
            string method,
            Generator generator,
            GeneratorOptions generatorOptions);
        
        IEnumerable<object[]> GenerateNWise(
            string method,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage,
            Dictionary<string, string[]> choices = null,
            object constraints = null);

        IEnumerable<object[]> GenerateCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null);
        
        IEnumerable<object[]> GenerateRandom(
            string method,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates,
            bool adaptive = Default.ParameterAdaptive,
            Dictionary<string, string[]> choices = null,
            object constraints = null);
        
        IEnumerable<object[]> GenerateStatic(
            string method,
            object testSuites = null);
    }
}