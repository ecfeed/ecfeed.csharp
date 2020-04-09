using System;
using System.Collections.Generic;

namespace EcFeed
{
    public interface ITestProvider
    {
        string Model { get; set; }

        void ValidateConnectionSettings();

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

        IEnumerable<string> Export(
            string method,
            Generator generator,
            GeneratorOptions generatorOptions,
            Template template = Default.ExportTemplate);
        
        IEnumerable<string> ExportNWise(
            string method,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            Template template = Default.ExportTemplate);

        IEnumerable<string> ExportCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            Template template = Default.ExportTemplate);
        
        IEnumerable<string> ExportRandom(
            string method,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates,
            bool adaptive = Default.ParameterAdaptive,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            Template template = Default.ExportTemplate);
        
        IEnumerable<string> ExportStatic(
            string method,
            object testSuites = null,
            Template template = Default.ExportTemplate);
    }
}