using System;
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
    public sealed class TestProvider : ITestProvider
    {
        static TestProvider()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }

        public string KeyStorePath { get; set; }
        public string KeyStorePassword { get; set; }
        public string CertificateHash { get; set; }
        public string GeneratorAddress { get; set; }

        public string Model { get; set; }
        public string Method { get; set; }
        public Dictionary<string, object> Settings { get; set; }       

        public event EventHandler<TestEventArgs> TestEventHandler;
        public event EventHandler<StatusEventArgs> StatusEventHandler;

        public TestProvider(
            string keyStorePath = null, string keyStorePassword = null, string certificateHash = null, string generatorAddress = null, 
            string model = null, string method = null, Dictionary<string, object> settings = null,
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
            Method = method;
            Settings = settings;
        }

        public TestProvider(ITestProvider testProvider) 
            : this(
                testProvider.KeyStorePath, testProvider.KeyStorePassword, testProvider.CertificateHash, testProvider.GeneratorAddress, 
                testProvider.Model, testProvider.Method, testProvider.Settings,
                false) { }

        public ITestProvider Copy()
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

        public async Task<string> Generate(
            string template = Default.Template)
        {
            Dictionary<string, object> additionalData = new Dictionary<string, object> { };

            return await RequestGenerate(additionalData, template);
        }

        public async Task<string> GenerateCartesian(
            string template = Default.Template)
        {
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Cartesian } };

            return await RequestGenerate(additionalData, template);
        }

        public async Task<string> GenerateNWise(
            string template = Default.Template,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage)
        {
            Dictionary<string, string> additionalDataProperties = new Dictionary<string, string> { { Parameter.N, "" + n }, { Parameter.Coverage, "" + coverage } };
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.NWise }, { Parameter.Properties, additionalDataProperties } };

            return await RequestGenerate(additionalData, template);
        }

        public async Task<string> GenerateRandom(
            string template = Default.Template,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates)
        {
            Dictionary<string, object> additionalDataProperties = new Dictionary<string, object> { { Parameter.Length, "" + length }, { Parameter.Duplicates, duplicates ? "true" : "false" } };
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Random }, { Parameter.Properties, additionalDataProperties } };

            return await RequestGenerate(additionalData, template);
        }

        public async Task<string> GenerateStatic(
            string template = Default.Template,
            object testSuites = null)
        {
            object updateTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Static }, { Parameter.TestSuites, updateTestSuites } };

            return await RequestGenerate(additionalData, template);
        }

        private async Task<string> RequestGenerate(Dictionary<string, object> additionalData, string template)
        {
            ITestProvider context = this.Copy();
            context.Settings = MergeTestProviderSettings(additionalData, context.Settings);

            string request = GenerateRequestURL(context, template);
            Task<string> response = SendRequest(request, template.Equals(Template.Stream));

            return await response;
        }

        public TestQueue Queue()
        {
            Dictionary<string, object> additionalData = new Dictionary<string, object> { };
            
            return RequestQueue(additionalData);
        }

        public TestQueue QueueCartesian()
        {
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Cartesian } };

            return RequestQueue(additionalData);
        }

        public TestQueue QueueNWise(
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage)
        {
            Dictionary<string, string> additionalDataProperties = new Dictionary<string, string> { { Parameter.N, "" + n }, { Parameter.Coverage, "" + coverage } };
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.NWise }, { Parameter.Properties, additionalDataProperties } };

            return RequestQueue(additionalData);
        }

        public TestQueue QueueRandom(
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates)
        {
            Dictionary<string, object> additionalDataProperties = new Dictionary<string, object> { { Parameter.Length, "" + length }, { Parameter.Duplicates, duplicates ? "true" : "false" } };
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Random }, { Parameter.Properties, additionalDataProperties } };

            return RequestQueue(additionalData);
        }

        public TestQueue QueueStatic(
            object testSuites = null)
        {
            object updateTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;Console.WriteLine(updateTestSuites);
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Static }, { Parameter.TestSuites, updateTestSuites } };

            return RequestQueue(additionalData);
        }

        private TestQueue RequestQueue(Dictionary<string, object> additionalData)
        {
            ITestProvider context = this.Copy();
            context.Settings = MergeTestProviderSettings(additionalData, context.Settings);

            return new TestQueue(context);
        }

        public TestList List()
        {
            Dictionary<string, object> additionalData = new Dictionary<string, object> { };

            return RequestList(additionalData);
        }

        public TestList ListCartesian()
        {
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Cartesian } };

            return RequestList(additionalData);
        }

        public TestList ListNWise(
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage)
        {
            Dictionary<string, string> additionalDataProperties = new Dictionary<string, string> { { Parameter.N, "" + n }, { Parameter.Coverage, "" + coverage } };
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.NWise }, { Parameter.Properties, additionalDataProperties } };

            return RequestList(additionalData);
        }

        public TestList ListRandom(
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates)
        {
            Dictionary<string, object> additionalDataProperties = new Dictionary<string, object> { { Parameter.Length, "" + length }, { Parameter.Duplicates, duplicates ? "true" : "false" } };
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Random }, { Parameter.Properties, additionalDataProperties } };

            return RequestList(additionalData);
        }

        public TestList ListStatic(
            object testSuites = null)
        {
            object updateTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;
            Dictionary<string, object> additionalData = new Dictionary<string, object> { { Parameter.DataSource, Generator.Static }, { Parameter.TestSuites, updateTestSuites } };

            return RequestList(additionalData);
        }

        private TestList RequestList(Dictionary<string, object> additionalData)
        {
            ITestProvider context = this.Copy();
            context.Settings = MergeTestProviderSettings(additionalData, context.Settings);

            return new TestList(context);
        }

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
            return template.Equals(Template.Stream) || template.Equals(Template.StreamRaw) ? Request.Data : Request.Export;
        }

        public void AddTestEventHandler(EventHandler<TestEventArgs> testEventHandler)
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

        public void RemoveTestEventHandler(EventHandler<TestEventArgs> testEventHandler)
        {
            
            if (TestEventHandler != null)
            {
                if (Array.IndexOf(TestEventHandler.GetInvocationList(), testEventHandler) != -1)
                {
                    TestEventHandler -= testEventHandler;
                }
            }

        }

        public void AddStatusEventHandler(EventHandler<StatusEventArgs> statusEventHandler)
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

        public void RemoveStatusEventHandler(EventHandler<StatusEventArgs> statusEventHandler)
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

        private string GenerateRequestURL(ITestProvider provider, string template)
        {
            string requestData = SerializeTestProvider(provider, template);
            string request = $"{ GeneratorAddress }/{ Endpoint.Generator }?requestType={ GetRequestType(template) }&request={ requestData }";

            return request;
        }

        private string SerializeTestProvider(ITestProvider context, string template)
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

        private void ValidateTestProvider(ITestProvider context)
        {

            if (string.IsNullOrEmpty(context.Model))
            {
                throw new TestProviderException("The model ID is not defined and the default value cannot be used.");
            }

            if (string.IsNullOrEmpty(context.Method))
            {
                throw new TestProviderException("The method name is not defined and the default value cannot be used.");
            }

            if (context.Settings == null)
            {
                context.Settings = new Dictionary<string, object> { { Parameter.DataSource, Generator.NWise } };
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
            ProcessSingleStatusResponse(line);
            ProcessSingleTestResponse(line, streamFilter, responseBuilder);
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
                testEventArgs.Schema = JsonConvert.DeserializeObject<TestCase>(line);

                if (testEventArgs.Schema.TestCaseArguments == null)
                {
                    throw new TestProviderException("The message cannot be parsed.");
                }

                testEventArgs.DataObject = StreamParser.ParseTestCaseToDataType(testEventArgs.Schema);
                testEventArgs.DataTest = StreamParser.ParseTestToNUnit(testEventArgs.Schema);

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
            List<string> settings = new List<string>();

            if (Settings != null)
            {
                foreach (var (key, value) in Settings)
                {
                    settings.Add("{" + $"{ key }: { value }" + "}");
                }
            }
            else
            {
                settings.Add("EMPTY");
            }

            return
                $"TestProvider:\n" +
                $"\t[KeyStorePath: '{ Path.GetFullPath(KeyStorePath) }']\n" +
                $"\t[KeyStorePassword: '{ KeyStorePassword }']\n" +
                $"\t[CertificateHash: '{ CertificateHash }']\n" +
                $"\t[GeneratorAddress: '{ GeneratorAddress }']\n" +
                $"\t[Model: '{ Model }']\n" +
                $"\t[Method: '{ Method }']\n" +
                $"\t[Settings: '{ string.Join(", ", settings) }']";
        }
    }
}