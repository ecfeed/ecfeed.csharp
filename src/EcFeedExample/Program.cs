using System;
using EcFeed;

namespace EcFeedExample
{
    class Example : Runner {

        public override void ProcessResponseHandler(String line) {
            Console.WriteLine(line);
        }

        static void Main(string[] args) {
            string body = "{" +
                "\"method\":\"void com.ecfeed.Model.simple(String,String,String,String,String,String,String,String,String,String,String)\"," +
                "\"model\":\"9835-3029-2264-1682-5114\",\"userData\":\"{'dataSource':'genNWise','constraints':'NONE'}\"," +
                "\"sessionId\":0" +
            "}";

            Runner program = new Example();
           
            program.WebService(body);
        }

    }

}
