
// #define VERBOSE

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using Newtonsoft.Json;

namespace EcFeed
{
    internal static class RequestHelper 
    {
        public static void ValidateConnection(string generatorAddress, string keyStorePath, string keyStorePassword) 
        {
            HttpWebResponse response = RequestHelper.SendRequest(RequestHelper.GenerateHealthCheckURL(generatorAddress), keyStorePath, keyStorePassword);
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
                string line;
                    
                while ((line = reader.ReadLine()) != null) {
                    DebugHelper.PrintTrace("VALIDATE", line);
                }
               
            }
            
        }  

        public static string GenerateHealthCheckURL(string address, string endpoint = Endpoint.HealthCheck)
        {
            string request = $"{ address }/{ endpoint }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            DebugHelper.PrintTrace("HEALTH CHECK REQUEST", request);

            return request;
        }

        public static string GenerateRequestURL(SessionData feedback, string address, string endpoint = Endpoint.Generator)
        {
            string requestData = $"{ SerializeRequestData(feedback) }";
            string requestType = feedback.Template.GetValue().Equals(Template.Stream.GetValue()) ? Request.Data : Request.Export;
            string request = $"{ address }/{ endpoint }?requestType={ requestType }&request={ requestData }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            DebugHelper.PrintTrace("DATA REQUEST", request);

            return request;
        }

        private static string SerializeRequestData(SessionData feedback)
        {
            if (string.IsNullOrEmpty(feedback.MethodName))
            {
                throw new TestProviderException("The method is not defined and the default value cannot be used.");
            }

            var parsedRequest = new
            {
                model = feedback.ModelId,
                method = feedback.MethodName,
                template = feedback.Template.GetValue(),
                userData = feedback.GeneratorData.ToString()
            };

            return JsonConvert.SerializeObject(parsedRequest);
        } 

        public static HttpWebResponse SendRequest(String request, string keyStorePath, string keyStorePassword)
        {
            X509Certificate2 certificate = null;

            try 
                {
                    certificate = new X509Certificate2(keyStorePath, keyStorePassword);

                    HttpWebRequest httpWebRequest = (HttpWebRequest) HttpWebRequest.Create(request);
                    httpWebRequest.ServerCertificateValidationCallback = ValidateServerCertificate;
                    httpWebRequest.ClientCertificates.Add(certificate);

                    return (HttpWebResponse) httpWebRequest.GetResponse();
                }
                catch (CryptographicException e)
                {
                    string message = $"The keystore password is incorrect. Keystore path: '{ Path.GetFullPath(keyStorePath) }'";
                    throw new TestProviderException(message, e);
                }
                catch (WebException e)
                {
                    string message = $"The connection could not be established.";
                    throw new TestProviderException(message, e);
                }
                finally
                {
                    certificate.Dispose();
                }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
        {
            foreach(X509ChainElement certificate in chain.ChainElements) {
                if (certificate.Certificate.IssuerName.Name.Contains("C=NO, L=Oslo, O=EcFeed AS, OU=EcFeed, CN=ecfeed.com")) {
                    return true;
                }
            }
    
            return false;
        }
    }
    internal static class DebugHelper
    {
        [Conditional("VERBOSE")]
        public static void PrintTrace(string header, string trace)
        {
            Console.WriteLine($"{ DateTime.Now.ToString()} - { header }\n{ trace }\n");
        }
        
    }
}