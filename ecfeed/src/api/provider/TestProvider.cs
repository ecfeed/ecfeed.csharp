#define VERBOSE

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace EcFeed
{
    public class TestProvider
    {
        static TestProvider()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }

        public string Model { get; set; }

        public string KeyStorePath { get; private set; }
        public string KeyStorePassword { get; private set; }
        public string GeneratorAddress { get; private set; }

        public TestProvider(string model, string keyStorePath = null, string keyStorePassword = null, string generatorAddress = null)
        {
            Model = model;

            KeyStorePath = string.IsNullOrEmpty(keyStorePath) ? SetDefaultKeyStorePath() : keyStorePath;
            KeyStorePassword = string.IsNullOrEmpty(keyStorePassword) ? Default.KeyStorePassword : keyStorePassword;;
            GeneratorAddress = string.IsNullOrEmpty(generatorAddress) ? Default.GeneratorAddress : generatorAddress;
        }

        private string SetDefaultKeyStorePath()
        {
            foreach (string path in Default.KeyStorePath)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            Console.WriteLine("The default keystore could not be loaded. In order to use the test generator, please provide a correct path.");
            return "";
        }

        public void ValidateConnection() 
        {
            new HandleRequest<string>(SendRequest(GenerateHealthCheckURL(GeneratorAddress)), Template.Stream);
        }

//-------------------------------------------------------------------------------------------

        public string[] GetMethodTypes(string method)
        {
            return SendHeaderRequest(method).MethodArgumentTypes;
        }

        public string[] GetMethodNames(string method)
        {
            return SendHeaderRequest(method).MethodArgumentNames;
        }

        private HandleRequest<string> SendHeaderRequest(string method)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.AddProperty(Parameter.Length, "0");

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Random.GetValue());

            HandleRequest<string> queue = Process<string>(method, additionalOptions, null, null, Template.Stream);

            foreach(string element in queue) { };

            return queue;
        }

//-------------------------------------------------------------------------------------------

        protected IEnumerable<string> Export(
            string method,
            Generator generator,
            GeneratorOptions generatorOptions,
            Template template = Default.ParameterTemplate)
        {
            generatorOptions.AddOption(Parameter.DataSource, generator.GetValue());

            return Process<string>(method, generatorOptions, null, null, template);
        }

        public IEnumerable<string> ExportNWise(
            string method,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            Template template = Default.ParameterTemplate)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.AddProperty(Parameter.N, "" + n);
            additionalProperties.AddProperty(Parameter.Coverage, "" + coverage);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.NWise.GetValue());

            return Process<string>(method, additionalOptions, choices, constraints, template);
        }

        public IEnumerable<string> ExportCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            Template template = Default.ParameterTemplate)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Cartesian.GetValue());

            return Process<string>(method, additionalOptions, choices, constraints, template);
        }

        public IEnumerable<string> ExportRandom(
            string method,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates,
            bool adaptive = Default.ParameterAdaptive,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            Template template = Default.ParameterTemplate)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.AddProperty(Parameter.Length, "" + length);
            additionalProperties.AddProperty(Parameter.Duplicates, "" + duplicates);
            additionalProperties.AddProperty(Parameter.Adaptive, "" + adaptive);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Random.GetValue());

            return Process<string>(method, additionalOptions, choices, constraints, template);
        }

        public IEnumerable<string> ExportStatic(
            string method,
            object testSuites = null,
            Template template = Default.ParameterTemplate)
        {
            object updatedTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;

            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Static.GetValue());
            additionalOptions.AddOption(Parameter.TestSuites, updatedTestSuites);
           
            return Process<string>(method, additionalOptions, null, null, template);
        }

//-------------------------------------------------------------------------------------------

        protected IEnumerable<object[]> Generate(
            string method,
            Generator generator,
            GeneratorOptions generatorOptions)
        {
            generatorOptions.AddOption(Parameter.DataSource, generator.GetValue());

            return Process<object[]>(method, generatorOptions);
        }

        public IEnumerable<object[]> GenerateNWise(
            string method,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage,
            Dictionary<string, string[]> choices = null,
            object constraints = null)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.AddProperty(Parameter.N, "" + n);
            additionalProperties.AddProperty(Parameter.Coverage, "" + coverage);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.NWise.GetValue());

            return Process<object[]>(method, additionalOptions, choices, constraints);
        }

        public IEnumerable<object[]> GenerateCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Cartesian.GetValue());

            return Process<object[]>(method, additionalOptions, choices, constraints);
        }

        public IEnumerable<object[]> GenerateRandom(
            string method,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates,
            bool adaptive = Default.ParameterAdaptive,
            Dictionary<string, string[]> choices = null,
            object constraints = null)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.AddProperty(Parameter.Length, "" + length);
            additionalProperties.AddProperty(Parameter.Duplicates, "" + duplicates);
            additionalProperties.AddProperty(Parameter.Adaptive, "" + adaptive);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Random.GetValue());

            return Process<object[]>(method, additionalOptions, choices, constraints);
        }

        public IEnumerable<object[]> GenerateStatic(
            string method,
            object testSuites = null)
        {
            object updatedTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;

            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Static.GetValue());
            additionalOptions.AddOption(Parameter.TestSuites, updatedTestSuites);
           
            return Process<object[]>(method, additionalOptions);
        }

//-------------------------------------------------------------------------------------------

        private HandleRequest<T> Process<T>(
            string method, 
            GeneratorOptions options, 
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            Template template = Template.Stream)
        {
            string requestURL = GenerateRequestURL(options, method, template.GetValue());

            if (choices != null)
            {
                options.AddOption(Parameter.Choices, choices);
            }

            if (constraints != null)
            {
                options.AddOption(Parameter.Constraints, constraints);
            }

            return new HandleRequest<T>(SendRequest(requestURL), template);
        }

//-------------------------------------------------------------------------------------------

        private string GenerateHealthCheckURL(string address)
        {
            string request = $"{ address }/{ Endpoint.HealthCheck }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            PrintTrace("HEALTH CHECK REQUEST", request);

            return request;
        }

        private string GenerateRequestURL(GeneratorOptions options, string method, string template)
        {
            string requestData = $"{ SerializeRequest(options, method, template) }";
            string request = $"{ GeneratorAddress }/{ Endpoint.Generator }?requestType={ GetRequestType(template) }&request={ requestData }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            PrintTrace("DATA REQUEST", request);

            return request;
        }

        private string GetRequestType(string template)
        {
            return template.Equals(Template.Stream.GetValue()) ? Request.Data : Request.Export;
        }

        private string SerializeRequest(GeneratorOptions options, string method, string template)
        {
            if (string.IsNullOrEmpty(Model))
            {
                throw new TestProviderException("The model ID is not defined and the default value cannot be used.");
            }

            if (string.IsNullOrEmpty(method))
            {
                throw new TestProviderException("The method is not defined and the default value cannot be used.");
            }

            var parsedRequest = new
            {
                model = Model,
                method = method,
                template = template,
                userData = options.ToString()
            };

            return JsonConvert.SerializeObject(parsedRequest);
        }

        private HttpWebResponse SendRequest(String request)
        {
            X509Certificate2 certificate = null;

            try 
                {
                    certificate = new X509Certificate2(KeyStorePath, KeyStorePassword);

                    HttpWebRequest httpWebRequest = (HttpWebRequest) HttpWebRequest.Create(request);
                    httpWebRequest.ServerCertificateValidationCallback = ValidateServerCertificate;
                    httpWebRequest.ClientCertificates.Add(certificate);

                    return (HttpWebResponse) httpWebRequest.GetResponse();
                }
                catch (CryptographicException)
                {
                    string message = $"The keystore password is incorrect. Keystore path: '{ Path.GetFullPath(KeyStorePath) }'";
                    throw new TestProviderException(message);
                }
                catch (WebException)
                {
                    string message = $"The connection could not be established.";
                    throw new TestProviderException(message);
                }
                finally
                {
                    certificate.Dispose();
                }
        }

        private bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
        {
            foreach(X509ChainElement certificate in chain.ChainElements) {
                if (certificate.Certificate.IssuerName.Name.Contains("C=NO, L=Oslo, O=EcFeed AS, OU=EcFeed, CN=ecfeed.com")) {
                    return true;
                }
            }
    
            return false;
        }

//-------------------------------------------------------------------------------------------  

        public class HandleRequest<T> : IEnumerable<T>
        {
            internal BlockingCollection<T> TestQueue { get; set; }
            internal string[] MethodArgumentNames { get; set; }
            internal string[] MethodArgumentTypes { get; set; }

            public HandleRequest(HttpWebResponse response, Template template)
            {
                TestQueue = new BlockingCollection<T>();
                ProcessResponse(response, template);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public IEnumerator<T> GetEnumerator()
            {
                while (!TestQueue.IsCompleted)
                {
                    T element = default(T);

                    try
                    {
                        element = TestQueue.Take();
                    }
                    catch (InvalidOperationException) { }

                    if (element != null)
                    {
                        yield return element;
                    }
                }
            }

            private async void ProcessResponse(HttpWebResponse response, Template template) 
            {
                if (!response.StatusCode.ToString().Equals("OK"))
                {
                    GenerateStatusEvent(response.StatusDescription, true);
                    return;
                }

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
                    string line;
                    
                    while ((line = await reader.ReadLineAsync()) != null) {
                        ProcessResponseLine(line, template);
                    }

                    EndTransmission();
                }

            }

            private void ProcessResponseLine(string line, Template template)
            {Console.WriteLine(line);
                if (template == Template.Stream)
                {
                    ProcessResponseInfoLine(line);
                    ProcessResponseStatusLine(line);
                    ProcessResponseDataLine(line);
                }
                else
                {
                    DataEventArgs dataEventArgs = new DataEventArgs() { DataRaw = line};
                    GenerateTestEvent(dataEventArgs);
                }
            }

            private void ProcessResponseInfoLine(string line)
            {
                if (line.Contains("\"info\""))
                {
                    try
                    {
                        InfoMessage infoMessage = JsonConvert.DeserializeObject<InfoMessage>(line);
                        MethodArgumentNames = InfoMessageHelper.ExtractArgumentNames(infoMessage);
                        MethodArgumentTypes = InfoMessageHelper.ExtractArgumentTypes(infoMessage);
                    }
                    catch (JsonReaderException) { }
                    catch (JsonSerializationException) { }
                }
            }
            private void ProcessResponseStatusLine(string line)
            {
                if (line.Contains("\"status\""))
                {
                    try
                    {
                        StatusMessage statusMessage = JsonConvert.DeserializeObject<StatusMessage>(line);
                        GenerateStatusEvent(line, false);
                    }
                    catch (JsonReaderException) { }
                    catch (JsonSerializationException) { }
                }
            }

            private void ProcessResponseDataLine(string line)
            {
                DataEventArgs testEventArgs = new DataEventArgs() { DataRaw = line };
                
                try
                {
                    testEventArgs.Schema = StreamParser.ParseTestCase(line);

                    if (testEventArgs.Schema.TestCaseArguments != null)
                    {
                        testEventArgs.DataObject = StreamParser.ParseTestCaseToDataType(testEventArgs.Schema, MethodArgumentTypes);
                        GenerateTestEvent(testEventArgs);
                        return;
                    }    
                }
                catch (JsonReaderException) { }
                catch (JsonSerializationException) { }
            }

            private void EndTransmission()
            {
                GenerateStatusEvent("END_DATA", true);
            }

            private void GenerateStatusEvent(string message, bool completed)
            {
                if (completed)
                {
                    TestQueue.CompleteAdding();
                }
            }

            private void GenerateTestEvent(DataEventArgs args)
            {
                if (args.DataRaw != null && args.DataRaw.GetType() == typeof(T))
                {
                    TestQueue.Add((T)(object)args.DataRaw);
                    return;
                }
                
                if (args.DataObject != null && args.DataObject.GetType() == typeof(T))
                {
                    TestQueue.Add((T)(object)args.DataObject);
                    return;
                }
                
                throw new TestProviderException("Unknown type.");
            }
    }

//------------------------------------------------------------------------------------------- 

        [Conditional("VERBOSE")]
        private void PrintTrace(string header, string trace)
        {
            Console.WriteLine($"{ DateTime.Now.ToString()} - { header }\n{ trace }\n");
        }

        public override string ToString()
        { 
            return
                $"TestProvider:\n" +
                $"\t[KeyStorePath: '{ Path.GetFullPath(KeyStorePath) }']\n" +
                $"\t[KeyStorePassword: '{ KeyStorePassword }']\n" +
                $"\t[GeneratorAddress: '{ GeneratorAddress }']\n" +
                $"\t[Model: '{ Model }']";
        }
    }
}