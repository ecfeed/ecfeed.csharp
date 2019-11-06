using System;
using System.IO;

using System.Net;  
using System.Text;  
using System.Security.Cryptography.X509Certificates;

using System.Net.Security;

namespace EcFeed {

    public class Runner {

        private static string serverVersion = @"genServiceVersion";
        private static string serverService = @"testCaseService";
        private static string serverServiceRequestType = @"requestType=requestData";
        private static string serverServiceCertificate = @"AAE72557A7DB47EA4CF4C649108E422528EFDA1B";

        private string server = "https://gen.ecfeed.com";
        private string keystore = Environment.GetEnvironmentVariable("HOME") + "/.ecfeed/security.p12";
        private string password = "changeit";
        
        static Runner() {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }

        public string Server {
            get { return server; }
            set { server = value; }
        }

        public string KeyStore {
            get { return keystore; }
            set { keystore = value; }
        }

        public string Password {
            get { return password; }
            set { password = value; }
        }

        public void WebVersion() {
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(server + "/" + serverVersion);
            request.ServerCertificateValidationCallback = ValidateServerCertficate;
            request.ClientCertificates.Add(GetUserCerificate());

            ProcessResponse((HttpWebResponse)request.GetResponse());
        }

        public void WebService(String body) {
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(server + "/" + serverService + "?" + serverServiceRequestType);
            request.ServerCertificateValidationCallback = ValidateServerCertficate;
            request.ClientCertificates.Add(GetUserCerificate());
            request.ContentType = "application/json";
            request.Method = "POST";

            using (Stream bodyStream = request.GetRequestStream()) {
                bodyStream.Write(Encoding.UTF8.GetBytes(body), 0, body.Length);
            }   

            ProcessResponse((HttpWebResponse)request.GetResponse());
        }

        private X509Certificate2 GetUserCerificate() {
             return new X509Certificate2(keystore, password);
        }

        private bool ValidateServerCertficate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            
            foreach(X509ChainElement certificate in chain.ChainElements) {
                if (certificate.Certificate.GetCertHashString() == serverServiceCertificate) {
                    return true;
                }
            }
 
            return false;
        }

        private void ProcessResponse(HttpWebResponse response) {
            
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    ProcessResponseHandler(line);
                }
            }

        }

        virtual public void ProcessResponseHandler(String line) {
            Console.WriteLine(line);
        }

    }
    
}
