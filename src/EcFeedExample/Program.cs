using System;
using EcFeed;

namespace EcFeedExample
{
    class Example : Runner {

        public override void ProcessResponseHandler(String line) {
            Console.WriteLine(line);
        }

        static void Main(string[] args) {
            string body = "{\"method\":\"public void com.example.test.LoanDecisionTest.generateCustomerData(java.lang.String,java.lang.String,com.example.test.Gender,int,java.lang.String,com.example.test.DocumentType)\",\"model\":\"0568-9381-1319-0545-5890\",\"userData\":\"{'dataSource':'genCartesian'}\"}";

            Runner program = new Example();
           
            program.WebTask();
            program.WebService(body);
        }

    }

}
