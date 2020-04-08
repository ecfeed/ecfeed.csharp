using System;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace EcFeed
{
    public class TestProvider : ITestProvider
    {
        static TestProvider()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }

        public string KeyStorePath { get; private set; }
        public string KeyStorePassword { get; private set; }
        public string CertificateHash { get; private set; }
        public string GeneratorAddress { get; private set; }

        public string Model { get; set; }
        private string Method { get; set; }

        public GeneratorOptions Settings { get; set; }       

        internal Assembly TestAssembly { get; set; }
        internal string[] ArgumentTypes { get; set; }

        internal event EventHandler<TestEventArgs> TestEventHandler;
        internal event EventHandler<StatusEventArgs> StatusEventHandler;

        public TestProvider(
            string keyStorePath = null, string keyStorePassword = null, string certificateHash = null, string generatorAddress = null, 
            string model = null, GeneratorOptions settings = null,
            bool verify = true)
        {
            KeyStorePath = string.IsNullOrEmpty(keyStorePath) ? SetDefaultKeyStorePath() : keyStorePath;
            KeyStorePassword = string.IsNullOrEmpty(keyStorePassword) ? SetDefaultKeyStorePassword() : keyStorePassword;
            CertificateHash = string.IsNullOrEmpty(certificateHash) ? SetDefaultCertificateHash() : certificateHash;
            GeneratorAddress = string.IsNullOrEmpty(generatorAddress) ? SetDefaultServiceAddress() : generatorAddress;

            if (verify)
            {
                ValidateConnectionSettings();
            }

            Model = model;
            Settings = settings;
        }

        public TestProvider(TestProvider testProvider) 
            : this(
                testProvider.KeyStorePath, testProvider.KeyStorePassword, testProvider.CertificateHash, testProvider.GeneratorAddress, 
                testProvider.Model, testProvider.Settings,
                false) 
        { 
            this.Method = testProvider.Method;
        }

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

        private string SetDefaultCertificateHash()
        {
            return Default.CertificateHash;
        }

        private string SetDefaultServiceAddress()
        {
            return Default.GeneratorAddress;
        }

        public async void ValidateConnectionSettings() 
        {
            ValidateKeyStorePathSyntax();
            ValidateKeyStorePathCorectness();

            ValidateKeyStorePasswordSyntax();
            ValidateKeyStorePasswordCorectness();

            ValidateCertificateHashSyntax();
            ValidateCertificateHashSyntax();

            ValidateServiceAddressSyntax();
            ValidateServiceAddressCorectness();

            await SendRequest(GenerateHealthCheckURL(GeneratorAddress), false);
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
            try 
            {
                new X509Certificate2(KeyStorePath, KeyStorePassword);
            }
            catch (CryptographicException)
            {
                throw new TestProviderException($"The certificate password is incorrect. Keystore path: '{ Path.GetFullPath(KeyStorePath) }'");
            }
        }

        private void ValidateCertificateHashSyntax()
        {
            if (string.IsNullOrEmpty(CertificateHash))
            {
                throw new TestProviderException("The certificate hash is not defined.");
            }
        }

        private void ValidateCertificateHashCorectness() { }

        private void ValidateServiceAddressSyntax()
        {
            if (string.IsNullOrEmpty(GeneratorAddress))
            {
                throw new TestProviderException("The service address is not defined.");
            }
        }

        private void ValidateServiceAddressCorectness() { }

//-------------------------------------------------------------------------------------------

        public async Task<string> Generate(string method,
            string template = Default.Template)
        {
            Dictionary<string, object> additionalData = new Dictionary<string, object> { };

            return await RequestGenerate(additionalData, template);
        }

        // public async IEnumerable<string> GenerateCartesianx(
        //     string template = Default.Template)
        // {
        //     Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Cartesian } };

        //     return await RequestGenerate(additionalData, template);
        // }

        // public async IEnumerable<string> x(
        //     string template = Default.Template,
        //     int n = Default.ParameterN, 
        //     int coverage = Default.ParameterCoverage)
        // {
        //     Dictionary<string, string> additionalDataProperties = new Dictionary<string, string> { { Parameter.N, "" + n }, { Parameter.Coverage, "" + coverage } };
        //     Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.NWise }, { Parameter.Properties, additionalDataProperties } };

        //     return await RequestGenerate(additionalData, template);
        // }

        // public async IEnumerable<string> GenerateRandomx(
        //     string template = Default.Template,
        //     int length = Default.ParameterLength, 
        //     bool duplicates = Default.ParameterDuplicates)
        // {
        //     Dictionary<string, object> additionalDataProperties = new Dictionary<string, object> { { Parameter.Length, "" + length }, { Parameter.Duplicates, duplicates ? "true" : "false" } };
        //     Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Random }, { Parameter.Properties, additionalDataProperties } };

        //     return await RequestGenerate(additionalData, template);
        // }

        // public async IEnumerable<string> GenerateStaticx(
        //     string template = Default.Template,
        //     object testSuites = null)
        // {
        //     object updateTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;
        //     Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Static }, { Parameter.TestSuites, updateTestSuites } };

        //     return await RequestGenerate(additionalData, template);
        // }

        private async Task<string> RequestGenerate(Dictionary<string, object> additionalData, string template)
        {
            TestProvider context = this.Copy();
            context.Settings = MergeTestProviderSettings(additionalData, context.Settings);

            string request = GenerateRequestURL(context, template);
            Task<string> response = SendRequest(request, template.Equals(Template.Stream));

            return await response;
        }

//-------------------------------------------------------------------------------------------

        public IEnumerable<object[]> Generate(
            string method,
            Generator generator,
            GeneratorOptions generatorOptions)
        {
            return GenerateRequest(method, null);
        }

        public IEnumerable<object[]> GenerateCartesian(
            string method)
        {
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Cartesian.GetValue() } };

            return GenerateRequest(method, additionalData);
        }

        public IEnumerable<object[]> GenerateNWise(
            string method,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage)
        {
            Dictionary<string, string> additionalDataProperties = new Dictionary<string, string> { { Parameter.N, "" + n }, { Parameter.Coverage, "" + coverage } };
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.NWise.GetValue() }, { Parameter.Properties, additionalDataProperties } };

            return GenerateRequest(method, additionalData);
        }

        public IEnumerable<object[]> GenerateRandom(
            string method,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates)
        {
            Dictionary<string, object> additionalDataProperties = new Dictionary<string, object> { { Parameter.Length, "" + length }, { Parameter.Duplicates, duplicates ? "true" : "false" } };
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Random.GetValue() }, { Parameter.Properties, additionalDataProperties } };

            return GenerateRequest(method, additionalData);
        }

        public IEnumerable<object[]> GenerateStatic(
            string method,
            object testSuites = null)
        {
            object updateTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;Console.WriteLine(updateTestSuites);
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Static.GetValue() }, { Parameter.TestSuites, updateTestSuites } };

            return GenerateRequest(method, additionalData);
        }

        private TestQueue GenerateRequest(string method, Dictionary<string, object> additionalData = null)
        {
            TestProvider context = this.Copy();

            context.Settings = MergeTestProviderSettings(additionalData == null ? new Dictionary<string, object> { } : additionalData , context.Settings);
            context.Method = method;

            return new TestQueue(context);
        }

        internal async Task<string> GenerateExecute()
        {
            return await SendRequest(GenerateRequestURL(this, Template.Stream.GetValue()), true);
        }

//-------------------------------------------------------------------------------------------

        private Dictionary<string, object> MergeTestProviderSettings(Dictionary<string, object> settingsFrom, Dictionary<string, object> settingsTo)
        {

            if (settingsFrom == null)
            {
                settingsFrom = new Dictionary<string, object> { };
            }

            if (settingsTo == null)
            {
                settingsTo = new Dictionary<string, object> { };
            }

            settingsFrom.ToList().ForEach(x => settingsTo[x.Key] = x.Value);

            return settingsTo;
        }
        private string GetRequestType(string template)
        {
            return template.Equals(Template.Stream.GetValue()) || template.Equals(Template.StreamRaw.GetValue()) ? Request.Data : Request.Export;
        }

        internal void AddTestEventHandler(EventHandler<TestEventArgs> testEventHandler)
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

        internal void RemoveTestEventHandler(EventHandler<TestEventArgs> testEventHandler)
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

        private bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            
            foreach(X509ChainElement certificate in chain.ChainElements) {

                if (certificate.Certificate.GetCertHashString() == CertificateHash) {
                    return true;
                }

            }
 
            return false;
        }

        private string GenerateHealthCheckURL(string address)
        {
            string request = $"{ address }/{ Endpoint.HealthCheck }";

            return request;
        }

        private string GenerateRequestURL(TestProvider provider, string template)
        {
            string requestData = SerializeTestProvider(provider, template);
            string request = $"{ GeneratorAddress }/{ Endpoint.Generator }?requestType={ GetRequestType(template) }&request={ requestData }";

            return request;
        }

        private string SerializeTestProvider(TestProvider context, string template)
        {
            ValidateTestProvider(context);

            var parsedRequest = new
            {
                model = context.Model,
                method = context.Method,
                template = template,
                userData = JsonConvert.SerializeObject(context.Settings, Formatting.None).Replace("\"", "\'")
            };

            return JsonConvert.SerializeObject(parsedRequest);
        }

        private void ValidateTestProvider(TestProvider context)
        {
            if (string.IsNullOrEmpty(context.Model))
            {
                throw new TestProviderException("The model ID is not defined and the default value cannot be used.");
            }

            if (context.Settings == null)
            {
                context.Settings = new Dictionary<string, object> { { Parameter.DataSource, Generator.NWise.GetValue() } };
            }
        }

        private async Task<string> SendRequest(string request, bool streamFilter)
        {
            try 
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest) HttpWebRequest.Create(request);
                httpWebRequest.ServerCertificateValidationCallback = ValidateServerCertificate;
                httpWebRequest.ClientCertificates.Add(new X509Certificate2(KeyStorePath, KeyStorePassword));

                return await ProcessResponse((HttpWebResponse) httpWebRequest.GetResponse(), streamFilter);
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
        }

        private async Task<string> ProcessResponse(HttpWebResponse response, bool streamFilter) 
        {
            if (!response.StatusCode.ToString().Equals("OK"))
            {
                StatusEventArgs statusEventArgs = new StatusEventArgs() { DataRaw = response.StatusDescription, StatusCode = response.StatusCode, IsCompleted = true };
                GenerateStatusEvent(statusEventArgs);

                return response.StatusDescription;
            }

            StringBuilder responseBuilder = new StringBuilder("");

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
                string line;
                
                while ((line = await reader.ReadLineAsync()) != null) {
                    ProcessSingleResponse(line, streamFilter, responseBuilder);
                }

            }

            return responseBuilder.ToString();
        }

        private void ProcessSingleResponse(string line, bool streamFilter, StringBuilder responseBuilder)
        {
            ProcessSingleInfoResponse(line);
            ProcessSingleStatusResponse(line);
            ProcessSingleTestResponse(line, streamFilter, responseBuilder);
        }

        private void ProcessSingleInfoResponse(string line)
        {
            try
            {
                InfoMessage info = StreamParser.ParseInfoMessage(line);
                ArgumentTypes = InfoMessageHelper.ExtractTypes(info);
            }
            catch (JsonReaderException) { }
            catch (JsonSerializationException) { }
        }
        private void ProcessSingleStatusResponse(string line)
        {
            try
            {
                StatusEventArgs statusEventArgs = new StatusEventArgs() { DataRaw = line, StatusCode = HttpStatusCode.OK };
                statusEventArgs.Schema = JsonConvert.DeserializeObject<StatusMessage>(line);

                if (statusEventArgs.Schema.Status != null)
                {

                    if (statusEventArgs.Schema.Status.Equals("END_DATA"))
                    {
                        statusEventArgs.IsCompleted = true;
                    }
                    else 
                    {
                        statusEventArgs.IsCompleted = false;
                    }
                        
                    GenerateStatusEvent(statusEventArgs);
                }
            }
            catch (JsonReaderException) { }
            catch (JsonSerializationException) { }
        }

        private void ProcessSingleTestResponse(string line, bool streamFilter, StringBuilder responseBuilder)
        {
            TestEventArgs testEventArgs = new TestEventArgs() { DataRaw = line };

            try
            {
                testEventArgs.Schema = StreamParser.ParseTestCase(line);

                if (testEventArgs.Schema.TestCaseArguments == null)
                {
                    throw new TestProviderException("The message cannot be parsed.");
                }

                testEventArgs.DataObject = StreamParser.ParseTestCaseToDataType(testEventArgs.Schema, ArgumentTypes);

                responseBuilder.AppendLine(line);
                GenerateTestEvent(testEventArgs);
            }
            catch (Exception)
            {
                if (streamFilter)
                {
                    return;
                }

                responseBuilder.AppendLine(line);
                GenerateTestEvent(testEventArgs);
            }
        }

        private void GenerateStatusEvent(StatusEventArgs args)
        {
            if (StatusEventHandler != null && StatusEventHandler.GetInvocationList().Length > 0)
            {
                StatusEventHandler(this, args);
            }
        }

        private void GenerateTestEvent(TestEventArgs args)
        {
            if (TestEventHandler != null && TestEventHandler.GetInvocationList().Length > 0)
            {
                TestEventHandler(this, args);
            }
        }

        public override string ToString()
        { 
            return
                $"TestProvider:\n" +
                $"\t[KeyStorePath: '{ Path.GetFullPath(KeyStorePath) }']\n" +
                $"\t[KeyStorePassword: '{ KeyStorePassword }']\n" +
                $"\t[CertificateHash: '{ CertificateHash }']\n" +
                $"\t[GeneratorAddress: '{ GeneratorAddress }']\n" +
                $"\t[Model: '{ Model }']\n" +
                $"\t[Settings: '{ Settings }]";
        }
    }
}