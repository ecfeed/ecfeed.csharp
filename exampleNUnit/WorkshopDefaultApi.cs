// using System.Collections;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Net;
// using System.IO;
// using System.Text;
// using NUnit.Framework;
// using Newtonsoft.Json;
// using EcFeed;

// namespace exampleNUnit
// {
//     [TestFixture]
//     public class SeleniumApi
//     {
//         private static IEnumerable dataString = new TestProvider("PZS2-W9NH-FRGZ-LZ4N-VGMR").GenerateNWise("com.example.test.Demo.typeString", feedback:true, label:"Workshop 'api'");
       
//         private void HandleError(dynamic error, int duration, TestHandle ecfeed)
//         {
//             HandleErrorType(error.errorInput.ToObject<string[]>(), duration, "Input", ecfeed);
//             HandleErrorType(error.errorOutput.ToObject<string[]>(), duration, "Output", ecfeed);
//         }

//         private void HandleErrorType(string[] error, int duration, string type, TestHandle ecfeed)
//         {
//             if (error.Length > 0)
//             {
//                 ecfeed.addFeedback(false, comment:string.Join("\n", error), duration:duration, custom:new Dictionary<string, string>{{"Error type", type}});
//                 Assert.Fail();
//             }
//         }

//         private int GetTime(Stopwatch stopwatch)
//         {
//             stopwatch.Stop();
//             return (int)stopwatch.ElapsedMilliseconds;
//         }

//         [TestCaseSource("dataString")]
//         public void Test(string country, string name, string address, string product, string color, string size, string quantity, string payment, string delivery, string phone, string email, TestHandle ecfeed)
//         {
//             try 
//             {
//                 Stopwatch stopwatch = Stopwatch.StartNew();

//                 HttpWebRequest httpWebRequest = (HttpWebRequest) HttpWebRequest.Create("https://api.ecfeed.com/" + 
//                     "?mode=error" +
//                     "&country=" + country +
//                     "&name=" + name +
//                     "&address=" + address +
//                     "&product=" + product +
//                     "&color=" + color +
//                     "&size=" + size +
//                     "&quantity=" + quantity +
//                     "&payment=" + payment +
//                     "&delivery=" + delivery +
//                     "&phone=" + phone.Replace("+", "%2B") +
//                     "&email=" + email);
//                 httpWebRequest.Method = "Post";

//                 HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();
//                 // NUnit.Framework.TestContext.Progress.WriteLine(httpWebResponse.StatusDescription);

//                 using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.ASCII)) {
//                     HandleError(JsonConvert.DeserializeObject(reader.ReadLine()), GetTime(stopwatch), ecfeed);
//                 }

//                 ecfeed.addFeedback(true, duration:GetTime(stopwatch));
//             }
//             catch (WebException e)
//             {
//                 string message = $"The connection could not be established.";
//                 ecfeed.addFeedback(false, comment:message);
//                 throw new TestProviderException(message, e);
//             }    
//         }
//     }

// }