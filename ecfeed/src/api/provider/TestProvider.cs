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
//-------------------------------------------------------------------------------------------        
        private class TestQueue<T> : IEnumerable<T>
        {
            internal BlockingCollection<T> Fifo { get; set; }
            internal InfoMessage MethodHeader { get; set; }
            internal string[] MethodArgumentNames { get; set; }
            internal string[] MethodArgumentTypes { get; set; }

            public TestQueue()
            {
                Fifo = new BlockingCollection<T>();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public IEnumerator<T> GetEnumerator()
            {
                while (!Fifo.IsCompleted)
                {
                    T element = default(T);

                    try
                    {
                        element = Fifo.Take();
                    }
                    catch (InvalidOperationException) { }

                    if (element != null)
                    {
                        yield return element;
                    }
                }
            }
        }

//-------------------------------------------------------------------------------------------

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
                try
                {
                    if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    {
                        continue;
                    }

                    return path;
                }
                catch (TestProviderException) { }
            }

            Console.WriteLine("The default keystore could not be loaded. In order to use the test generator, please provide a correct path.");
            return "";
        }

        public void ValidateConnectionSettings() 
        {
            SendRequest<string>(GenerateURLHealthCheck(GeneratorAddress), Template.Stream);
        }

//-------------------------------------------------------------------------------------------

        public InfoMessage GetMethodHeader(string method)
        {
            return SendHeaderRequest(method).MethodHeader;
        }

        public string[] GetMethodTypes(string method)
        {
            return SendHeaderRequest(method).MethodArgumentTypes;
        }

        public string[] GetMethodNames(string method)
        {
            return SendHeaderRequest(method).MethodArgumentNames;
        }

        private TestQueue<string> SendHeaderRequest(string method)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.AddProperty(Parameter.Length, "0");

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Random.GetValue());

            TestQueue<string> queue = Process<string>(method, additionalOptions, null, null, Template.Stream);

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

        private TestQueue<T> Process<T>(
            string method, 
            GeneratorOptions options, 
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            Template template = Template.Stream)
        {
            string requestURL = GenerateURLRequest(options, method, template.GetValue());

            if (choices != null)
            {
                options.AddOption(Parameter.Choices, choices);
            }

            if (constraints != null)
            {
                options.AddOption(Parameter.Constraints, constraints);
            }

            return SendRequest<T>(requestURL, template);
        }

//-------------------------------------------------------------------------------------------

        private string GenerateURLHealthCheck(string address)
        {
            string request = $"{ address }/{ Endpoint.HealthCheck }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            PrintTrace("HEALTH CHECK REQUEST", request);

            return request;
        }

        private string GenerateURLRequest(GeneratorOptions options, string method, string template)
        {
            string requestData = $"{ SerializeTestProvider(options, method, template) }";
            string request = $"{ GeneratorAddress }/{ Endpoint.Generator }?requestType={ GetRequestType(template) }&request={ requestData }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            PrintTrace("DATA REQUEST", request);

            return request;
        }

        private string GetRequestType(string template)
        {
            return template.Equals(Template.Stream.GetValue()) ? Request.Data : Request.Export;
        }

        private string SerializeTestProvider(GeneratorOptions options, string method, string template)
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

        private TestQueue<T> SendRequest<T>(string request, Template template)
        {
            X509Certificate2 certificate = null;
            TestQueue<T> testQueue = new TestQueue<T>();

            try 
            {
                certificate = new X509Certificate2(KeyStorePath, KeyStorePassword);

                HttpWebRequest httpWebRequest = (HttpWebRequest) HttpWebRequest.Create(request);
                httpWebRequest.ServerCertificateValidationCallback = ValidateServerCertificate;
                httpWebRequest.ClientCertificates.Add(certificate);

                ProcessResponse(testQueue, (HttpWebResponse) httpWebRequest.GetResponse(), template);
            }
            catch (CryptographicException)
            {
                string message = $"The keystore password is incorrect. Keystore path: '{ Path.GetFullPath(KeyStorePath) }'";
                StatusEventArgs statusEventArgs = new StatusEventArgs() { DataRaw = message, StatusCode = HttpStatusCode.BadRequest, IsCompleted = true };
                
                throw new TestProviderException(message);
            }
            catch (WebException e)
            {
                StatusEventArgs statusEventArgs = new StatusEventArgs() { DataRaw = e.Message, StatusCode = ((HttpWebResponse) e.Response).StatusCode, IsCompleted = true };
                GenerateStatusEvent<T>(testQueue, statusEventArgs);

                throw new TestProviderException(e.Message);
            }
            finally
            {
                certificate.Dispose();
            }

            return testQueue;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) {

            foreach(X509ChainElement certificate in chain.ChainElements) {

                if (certificate.Certificate.IssuerName.Name.Contains("C=NO, L=Oslo, O=EcFeed AS, OU=EcFeed, CN=ecfeed.com")) {
                    return true;
                }

            }
 
            return false;
        }

//-------------------------------------------------------------------------------------------   

        private async void ProcessResponse<T>(TestQueue<T> testQueue, HttpWebResponse response, Template template) 
        {
            if (!response.StatusCode.ToString().Equals("OK"))
            {
                StatusEventArgs statusEventArgs = new StatusEventArgs() { DataRaw = response.StatusDescription, StatusCode = response.StatusCode, IsCompleted = true };
                GenerateStatusEvent<T>(testQueue, statusEventArgs);

                return;
            }

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
                string line;
                
                while ((line = await reader.ReadLineAsync()) != null) {
                    ProcessResponseLine<T>(testQueue, line, template);
                }

                EndTransmission<T>(testQueue);
            }

        }

        private void ProcessResponseLine<T>(TestQueue<T> testQueue, string line, Template template)
        {
            if (template == Template.Stream)
            {
                ProcessResponseInfoLine<T>(testQueue, line);
                ProcessResponseStatusLine<T>(testQueue, line);
                ProcessResponseDataLine<T>(testQueue, line);
            }
            else
            {
                DataEventArgs dataEventArgs = new DataEventArgs() { DataRaw = line};
                GenerateTestEvent<T>(testQueue, dataEventArgs);
            }
        }

        private void ProcessResponseInfoLine<T>(TestQueue<T> testQueue, string line)
        {
            if (line.Contains("'method'"))
            {
                try
                {
                    testQueue.MethodHeader = StreamParser.ParseInfoMessage(line);
                    testQueue.MethodArgumentNames = InfoMessageHelper.ExtractArgumentNames(testQueue.MethodHeader);
                    testQueue.MethodArgumentTypes = InfoMessageHelper.ExtractArgumentTypes(testQueue.MethodHeader);
                }
                catch (JsonReaderException) { }
                catch (JsonSerializationException) { }
            }
        }
        private void ProcessResponseStatusLine<T>(TestQueue<T> testQueue, string line)
        {
            try
            {
                StatusEventArgs statusEventArgs = new StatusEventArgs() { DataRaw = line, StatusCode = HttpStatusCode.OK };
                statusEventArgs.Schema = JsonConvert.DeserializeObject<StatusMessage>(line);

                if (statusEventArgs.Schema.Status != null)
                {
                    statusEventArgs.IsCompleted = false;       
                    GenerateStatusEvent<T>(testQueue, statusEventArgs);
                }
            }
            catch (JsonReaderException) { }
            catch (JsonSerializationException) { }
        }

        private void ProcessResponseDataLine<T>(TestQueue<T> testQueue, string line)
        {
            DataEventArgs testEventArgs = new DataEventArgs() { DataRaw = line };
            
            try
            {
                testEventArgs.Schema = StreamParser.ParseTestCase(line);

                if (testEventArgs.Schema.TestCaseArguments != null)
                {
                    testEventArgs.DataObject = StreamParser.ParseTestCaseToDataType(testEventArgs.Schema, testQueue.MethodArgumentTypes);
                    GenerateTestEvent<T>(testQueue, testEventArgs);
                    return;
                }    
            }
            catch (JsonReaderException) { }
            catch (JsonSerializationException) { }
        }

        private void EndTransmission<T>(TestQueue<T> testQueue)
        {
            StatusEventArgs statusEventArgs = new StatusEventArgs() { DataRaw = "END_DATA", StatusCode = HttpStatusCode.OK, IsCompleted = true };
            GenerateStatusEvent<T>(testQueue, statusEventArgs);
        }

        private void GenerateStatusEvent<T>(TestQueue<T> testQueue, StatusEventArgs args)
        {
            if (args.IsCompleted)
            {
                testQueue.Fifo.CompleteAdding();
            }
        }

        private void GenerateTestEvent<T>(TestQueue<T> testQueue, DataEventArgs args)
        {
            if (args.DataRaw != null && args.DataRaw.GetType() == typeof(T))
            {
                testQueue.Fifo.Add((T)(object)args.DataRaw);
                return;
            }
            
            if (args.DataObject != null && args.DataObject.GetType() == typeof(T))
            {
                testQueue.Fifo.Add((T)(object)args.DataObject);
                return;
            }
            
            throw new TestProviderException("Unknown type.");
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