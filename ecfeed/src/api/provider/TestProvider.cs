#define VERBOSE

using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Collections.Generic;
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

        public string KeyStorePath { get; private set; }
        public string KeyStorePassword { get; private set; }
        public string GeneratorAddress { get; private set; }

        public string Model { get; set; }

        private InfoMessage MethodHeader { get; set; }
        private string[] MethodArgumentNames { get; set; }
        private string[] MethodArgumentTypes { get; set; }

        private Assembly GlobalAssembly { get; set; }
        
        internal event EventHandler<DataEventArgs> TestEventHandler;
        internal event EventHandler<StatusEventArgs> StatusEventHandler;

        public TestProvider(
            string model, string keyStorePath = null, string keyStorePassword = null, string generatorAddress = null, bool verify = true)
        {
            KeyStorePath = string.IsNullOrEmpty(keyStorePath) ? SetDefaultKeyStorePath() : keyStorePath;
            KeyStorePassword = string.IsNullOrEmpty(keyStorePassword) ? SetDefaultKeyStorePassword() : keyStorePassword;;
            GeneratorAddress = string.IsNullOrEmpty(generatorAddress) ? SetDefaultServiceAddress() : generatorAddress;

            if (verify)
            {
                ValidateConnectionSettings();
            }

            Model = model;

            string main = verify ? "verified" : "non-verified | copy";
            PrintTrace($"CONFIGURATION ({ main })", this.ToString());
        }

        public TestProvider(TestProvider testProvider) 
            : this(
                testProvider.Model, testProvider.KeyStorePath, testProvider.KeyStorePassword, testProvider.GeneratorAddress, false) { }

        internal TestProvider Copy()
        {
            return new TestProvider(this);
        }

        private string SetDefaultKeyStorePath()
        {
            foreach (string path in Default.KeyStorePath)
            {
                try
                {
                    KeyStorePath = path;

                    ValidateKeyStorePathSyntax();
                    ValidateKeyStorePathCorectness();

                    return path;
                }
                catch (TestProviderException) { }
            }

            Console.WriteLine("The default keystore could not be loaded. In order to use the test generator, please provide a correct path.");
            return "";
        }

        private string SetDefaultKeyStorePassword()
        {
            return Default.KeyStorePassword;
        }

        private string SetDefaultServiceAddress()
        {
            return Default.GeneratorAddress;
        }

        public void ValidateConnectionSettings() 
        {
            ValidateKeyStorePathSyntax();
            ValidateKeyStorePathCorectness();

            ValidateKeyStorePasswordSyntax();
            ValidateKeyStorePasswordCorectness();

            ValidateServiceAddressSyntax();
            ValidateServiceAddressCorectness();

            SendRequest(GenerateHealthCheckURL(GeneratorAddress), false);
        }

        private void ValidateKeyStorePathSyntax()
        {
            if (string.IsNullOrEmpty(KeyStorePath))
            {
                throw new TestProviderException("The keystore path is not defined.");
            }
        }

        private void ValidateKeyStorePathCorectness()
        {
            if (!File.Exists(KeyStorePath)) 
            {
                throw new TestProviderException($"The keystore path is incorrect. It does not point to a file. Keystore path: '{ Path.GetFullPath(KeyStorePath) }'");
            }
        }

        private void ValidateKeyStorePasswordSyntax()
        {
            if (string.IsNullOrEmpty(KeyStorePassword))
            {
                throw new TestProviderException("The certificate password is not defined.");
            }
        }

        private void ValidateKeyStorePasswordCorectness()
        {
            X509Certificate2 certificate = null;

            try 
            {
                certificate = new X509Certificate2(KeyStorePath, KeyStorePassword);
            }
            catch (CryptographicException)
            {
                 throw new TestProviderException($"The certificate password is incorrect. Keystore path: '{ Path.GetFullPath(KeyStorePath) }'");
            }
            finally
            {
                if (certificate != null)
                {
                    certificate.Dispose();
                }
            }
        }

        private void ValidateServiceAddressSyntax()
        {
            if (string.IsNullOrEmpty(GeneratorAddress))
            {
                throw new TestProviderException("The service address is not defined.");
            }
        }

        private void ValidateServiceAddressCorectness() { }

//-------------------------------------------------------------------------------------------

        public InfoMessage GetMethodHeader(string method)
        {
            TestProvider context;
            SendHeaderRequest(out context, method);
            return context.MethodHeader;
        }

        public string[] GetMethodTypes(string method)
        {
            TestProvider context;
            SendHeaderRequest(out context, method);
            return context.MethodArgumentTypes;
        }

        public string[] GetMethodNames(string method)
        {
            TestProvider context;
            SendHeaderRequest(out context, method);
            return context.MethodArgumentNames;
        }

        private void SendHeaderRequest(out TestProvider context, string method)
        {
            context = this.Copy();

            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.AddProperty(Parameter.Length, "0");

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Random.GetValue());

            foreach(string element in ExportRequest(method, additionalOptions, Template.Stream, context)) { };
        }

//-------------------------------------------------------------------------------------------

        public IEnumerable<string> Export(
            string method,
            Generator generator,
            GeneratorOptions generatorOptions,
            Template template = Default.ParameterTemplate)
        {
            generatorOptions.AddOption(Parameter.DataSource, generator.GetValue());

            return ExportRequest(method, generatorOptions, template);
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

            if (choices != null)
            {
                additionalOptions.AddOption(Parameter.Choices, choices);
            }

            if (constraints != null)
            {
                additionalOptions.AddOption(Parameter.Constraints, constraints);
            }

            return ExportRequest(method, additionalOptions, template);
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

            if (choices != null)
            {
                additionalOptions.AddOption(Parameter.Choices, choices);
            }

            if (constraints != null)
            {
                additionalOptions.AddOption(Parameter.Constraints, constraints);
            }

            return ExportRequest(method, additionalOptions, template);
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

            if (choices != null)
            {
                additionalOptions.AddOption(Parameter.Choices, choices);
            }

            if (constraints != null)
            {
                additionalOptions.AddOption(Parameter.Constraints, constraints);
            }

            return ExportRequest(method, additionalOptions, template);
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
           
            return ExportRequest(method, additionalOptions, template);
        }

        private TestProviderQueue<string> ExportRequest(
            string method, 
            GeneratorOptions options, 
            Template template, 
            TestProvider context = null)
        {
            return new TestProviderQueue<string>(context == null ? this.Copy() : context, options, template, method);
        }

//-------------------------------------------------------------------------------------------

        public IEnumerable<object[]> Generate(
            string method,
            Generator generator,
            GeneratorOptions generatorOptions)
        {
            generatorOptions.AddOption(Parameter.DataSource, generator.GetValue());

            return GenerateRequest(method, generatorOptions);
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

            if (choices != null)
            {
                additionalOptions.AddOption(Parameter.Choices, choices);
            }

            if (constraints != null)
            {
                additionalOptions.AddOption(Parameter.Constraints, constraints);
            }

            return GenerateRequest(method, additionalOptions);
        }

        public IEnumerable<object[]> GenerateCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Cartesian.GetValue());

            if (choices != null)
            {
                additionalOptions.AddOption(Parameter.Choices, choices);
            }

            if (constraints != null)
            {
                additionalOptions.AddOption(Parameter.Constraints, constraints);
            }

            return GenerateRequest(method, additionalOptions);
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

            if (choices != null)
            {
                additionalOptions.AddOption(Parameter.Choices, choices);
            }

            if (constraints != null)
            {
                additionalOptions.AddOption(Parameter.Constraints, constraints);
            }

            return GenerateRequest(method, additionalOptions);
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
           
            return GenerateRequest(method, additionalOptions);
        }

        private TestProviderQueue<object[]> GenerateRequest(string method, GeneratorOptions options)
        {
            return new TestProviderQueue<object[]>(this.Copy(), options, Template.Stream, method);
        }

//-------------------------------------------------------------------------------------------

        internal void StartQueue(string method, GeneratorOptions options, Template template)
        {
            string requestURL = GenerateRequestURL(this, options, method, template.GetValue());

            SendRequest(requestURL, template == Template.Stream);
        }

//-------------------------------------------------------------------------------------------        

        internal void AddTestEventHandler(EventHandler<DataEventArgs> testEventHandler)
        {
            if (TestEventHandler == null)
            {
                TestEventHandler += testEventHandler;
            }
            else
            {
                if (Array.IndexOf(TestEventHandler.GetInvocationList(), testEventHandler) == -1)
                {
                    TestEventHandler += testEventHandler;
                } 
            }   
        }

        internal void RemoveTestEventHandler(EventHandler<DataEventArgs> testEventHandler)
        {
            if (TestEventHandler != null)
            {
                if (Array.IndexOf(TestEventHandler.GetInvocationList(), testEventHandler) != -1)
                {
                    TestEventHandler -= testEventHandler;
                }
            }
        }

        internal void AddStatusEventHandler(EventHandler<StatusEventArgs> statusEventHandler)
        {
            if (StatusEventHandler == null)
            {
                StatusEventHandler += statusEventHandler;
            }
            else
            {
                if (Array.IndexOf(StatusEventHandler.GetInvocationList(), statusEventHandler) == -1)
                {
                    StatusEventHandler += statusEventHandler;
                } 
            }   
        }

        internal void RemoveStatusEventHandler(EventHandler<StatusEventArgs> statusEventHandler)
        {
            if (StatusEventHandler != null)
            {
                if (Array.IndexOf(StatusEventHandler.GetInvocationList(), statusEventHandler) != -1)
                {
                    StatusEventHandler -= statusEventHandler;
                }
            }
        }

//-------------------------------------------------------------------------------------------

        private string GenerateHealthCheckURL(string address)
        {
            string request = $"{ address }/{ Endpoint.HealthCheck }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            PrintTrace("HEALTH CHECK REQUEST", request);

            return request;
        }

        private string GenerateRequestURL(TestProvider provider, GeneratorOptions options, string method, string template)
        {
            string requestData = $"{ SerializeTestProvider(options, provider.Model, method, template) }";
            string request = $"{ GeneratorAddress }/{ Endpoint.Generator }?requestType={ GetRequestType(template) }&request={ requestData }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            PrintTrace("DATA REQUEST", request);

            return request;
        }

        private string GetRequestType(string template)
        {
            return template.Equals(Template.Stream.GetValue()) || template.Equals(Template.StreamRaw.GetValue()) ? Request.Data : Request.Export;
        }

        private string SerializeTestProvider(GeneratorOptions options, string model, string method, string template)
        {
            if (string.IsNullOrEmpty(model))
            {
                throw new TestProviderException("The model ID is not defined and the default value cannot be used.");
            }

            if (string.IsNullOrEmpty(method))
            {
                throw new TestProviderException("The method is not defined and the default value cannot be used.");
            }

            var parsedRequest = new
            {
                model = model,
                method = method,
                template = template,
                userData = options.ToString()
            };

            return JsonConvert.SerializeObject(parsedRequest);
        }

        private void SendRequest(string request, bool streamFilter)
        {
            X509Certificate2 certificate = null;

            try 
            {
                certificate = new X509Certificate2(KeyStorePath, KeyStorePassword);

                HttpWebRequest httpWebRequest = (HttpWebRequest) HttpWebRequest.Create(request);
                httpWebRequest.ServerCertificateValidationCallback = ValidateServerCertificate;
                httpWebRequest.ClientCertificates.Add(certificate);

                ProcessResponse((HttpWebResponse) httpWebRequest.GetResponse(), streamFilter);
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
                GenerateStatusEvent(statusEventArgs);

                throw new TestProviderException(e.Message);
            }
            finally
            {
                certificate.Dispose();
            }
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

        private async void ProcessResponse(HttpWebResponse response, bool streamFilter) 
        {
            if (!response.StatusCode.ToString().Equals("OK"))
            {
                StatusEventArgs statusEventArgs = new StatusEventArgs() { DataRaw = response.StatusDescription, StatusCode = response.StatusCode, IsCompleted = true };
                GenerateStatusEvent(statusEventArgs);

                return;
            }

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
                string line;
                
                while ((line = await reader.ReadLineAsync()) != null) {
                    ProcessSingleResponse(line, streamFilter);
                }

                EndTransmission();
            }

        }

        private void ProcessSingleResponse(string line, bool streamFilter)
        {
            ProcessSingleInfoResponse(line);
            ProcessSingleStatusResponse(line);
            ProcessSingleDataResponse(line, streamFilter);
        }

        private void ProcessSingleInfoResponse(string line)
        {
            if (line.Contains("'method'"))
            {
                try
                {
                    MethodHeader = StreamParser.ParseInfoMessage(line);
                    MethodArgumentNames = InfoMessageHelper.ExtractArgumentNames(MethodHeader);
                    MethodArgumentTypes = InfoMessageHelper.ExtractArgumentTypes(MethodHeader);
                }
                catch (JsonReaderException) { }
                catch (JsonSerializationException) { }
            }
        }
        private void ProcessSingleStatusResponse(string line)
        {
            try
            {
                StatusEventArgs statusEventArgs = new StatusEventArgs() { DataRaw = line, StatusCode = HttpStatusCode.OK };
                statusEventArgs.Schema = JsonConvert.DeserializeObject<StatusMessage>(line);

                if (statusEventArgs.Schema.Status != null)
                {
                    statusEventArgs.IsCompleted = false;       
                    GenerateStatusEvent(statusEventArgs);
                }
            }
            catch (JsonReaderException) { }
            catch (JsonSerializationException) { }
        }

        private void ProcessSingleDataResponse(string line, bool streamFilter)
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

            if (!streamFilter)
            {
                 GenerateTestEvent(testEventArgs);
            }
        }

        private void EndTransmission()
        {
            StatusEventArgs statusEventArgs = new StatusEventArgs() { DataRaw = "END_DATA", StatusCode = HttpStatusCode.OK, IsCompleted = true };
            GenerateStatusEvent(statusEventArgs);
        }

        private void GenerateStatusEvent(StatusEventArgs args)
        {
            if (StatusEventHandler != null && StatusEventHandler.GetInvocationList().Length > 0)
            {
                StatusEventHandler(this, args);
            }
        }

        private void GenerateTestEvent(DataEventArgs args)
        {
            if (TestEventHandler != null && TestEventHandler.GetInvocationList().Length > 0)
            {
                TestEventHandler(this, args);
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