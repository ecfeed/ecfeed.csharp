
// #define VERBOSE

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

        public static string GenerateHealthCheckURL(string address, string endpoint = RequestEndpoint.HealthCheck)
        {
            string request = $"{ address }/{ endpoint }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            DebugHelper.PrintTrace("HEALTH CHECK REQUEST", request);

            return request;
        }

        public static string GenerateRequestURL(DataSession sessionData, string address, string endpoint = RequestEndpoint.Generator)
        {
            string requestModel = "";

            if (sessionData.ModelId == "TestUuid11")
            {
                requestModel = "&clientType=localTestRap";
            }

            string requestData = $"{ SerializeRequestData(sessionData) }";
            string requestType = sessionData.Template.GetValue().Equals(Template.Stream.GetValue()) ? RequestTestType.Data : RequestTestType.Export;
            string request = $"{ address }/{ endpoint }?requestType={ requestType }{ requestModel }&request={ requestData }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            DebugHelper.PrintTrace("DATA REQUEST", request);

            return request;
        }

        public static string GenerateFeedbackURL(DataSession sessionData, string address, string endpoint = RequestEndpoint.Feedback)
        {
            string requestModel = "";

            if (sessionData.ModelId == "TestUuid11")
            {
                requestModel = "?clientType=localTestRap";
            }

            string requestData = $"{ SerializeFeedbackData(sessionData) }";
            string request = $"{ address }/{ endpoint }{ requestModel }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            DebugHelper.PrintTrace("FEEDBACK REQUEST", request);

            return request;
        }

        private static string SerializeRequestData(DataSession feedback)
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

        private static string SerializeFeedbackData(DataSession sessionData)
        {
            return sessionData.ToString();
        }
        
        public static HttpWebResponse SendRequest(String request, string keyStorePath, string keyStorePassword, string body = null)
        {
            X509Certificate2 certificate = null;

            try 
                {
                    certificate = new X509Certificate2(keyStorePath, keyStorePassword);

                    HttpWebRequest httpWebRequest = (HttpWebRequest) HttpWebRequest.Create(request);
                    httpWebRequest.ServerCertificateValidationCallback = ValidateServerCertificate;
                    httpWebRequest.ClientCertificates.Add(certificate);

                    if (body != null) {
                        byte[] byteArray = Encoding.UTF8.GetBytes(body);
                        httpWebRequest.Method = "PUT";
                        httpWebRequest.ContentLength = byteArray.Length;
                        Stream dataStream = httpWebRequest.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();
                    } 

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
}