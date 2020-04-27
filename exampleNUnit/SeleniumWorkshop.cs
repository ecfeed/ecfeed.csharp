using System;
using System.Threading;
using System.Collections;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using EcFeed;

namespace exampleNUnit
{
    [TestFixture]
    public class SeleniumWorkshop
    {
        static public IEnumerable DataString = new TestProvider("7482-5194-2849-1943-2448").GenerateNWise("com.example.test.Demo.typeString");

        static RemoteWebDriver SeleniumDriver { get; set; }
        static string PageAddress { get; set; }
        static string[] PageFormControl { get; set; }
        static string[] PageFormOutput { get; set; }
        static string[][] PageFormInput { get; set; }

        private static void SetForm(RemoteWebDriver driver, string[][] values) 
        {
            ValidateInput(PageFormInput, values);

            SetFormText(driver, values[0]);
            SetFormSelect(driver, values[1]);
        }

        private static void ValidateInput(string[][] reference, string[][] input) 
        {
            if (input.Length != 2) 
            {
                throw new ArgumentException("The dimension of the input array is incorrect.");
            }
            if (reference[0].Length != input[0].Length) 
            {
                throw new ArgumentException("The number of the input text fields is incorrect.");
            }
            if (reference[1].Length != input[1].Length) 
            {
                throw new ArgumentException("The number of the input select fields is incorrect.");
            }
        }

        private static void SetFormText(RemoteWebDriver driver, string[] values) 
        {
            for (int i = 0 ; i < PageFormInput[0].Length ; i++) 
            {
                IWebElement element = driver.FindElementById(PageFormInput[0][i]);
                element.Clear();
                element.SendKeys(values[i]);
            }
        }

        private static void SetFormSelect(RemoteWebDriver driver, string[] values) 
        {
            for (int i = 0 ; i < PageFormInput[1].Length ; i++) 
            {
                (new SelectElement(driver.FindElementById(PageFormInput[1][i]))).SelectByText(values[i]);
            }
        }

        private static void Execute(RemoteWebDriver driver) 
        {
            foreach (string element in PageFormControl) 
            {
                driver.FindElementById(element).Click();
            }
        }

        private string[] GetResponse(RemoteWebDriver driver) 
        {
            string[] response = new string[PageFormOutput.Length];

            for (int i = 0 ; i < PageFormOutput.Length ; i++) 
            {
                response[i] = driver.FindElementById(PageFormOutput[i]).GetAttribute("value");
            }

            return response;
        }

        [OneTimeSetUp]
        public static void BeforeAll()
        {
            SeleniumDriver = new FirefoxDriver("/home/krzysztof") { Url = "http://www.workshop-2020-march.ecfeed.com"};
            PageAddress = "http://www.workshop-2020-march.ecfeed.com";
            PageFormControl = new string[] { "submit" };
            PageFormOutput = new string[] { "status", "response" };
            PageFormInput = new string[2][] { new string[] {"name", "address", "quantity", "phone", "email"}, new string[] {"country", "product", "color", "size", "payment", "delivery"} };
        }

        [OneTimeTearDown]
        public static void AfterAll()
        {
            SeleniumDriver.Close();
        }

        [TestCaseSource("DataString")]
        public void Test(string country, string name, string address, string product, string color, string size, string quantity, string payment, string delivery, string phone, string email)
        {
            string[][] input = new string[2][] { new string[] {name, address, quantity, phone, email}, new string[] {country, product, color, size, payment, delivery} };

            try 
            {
                SetForm(SeleniumDriver, input);
                Thread.Sleep(1000);
                Execute(SeleniumDriver);
            } 
            catch (ArgumentOutOfRangeException e1) 
            {
                Console.WriteLine(e1.Message);
            }
            catch (ArgumentException e2)
            {
                Console.WriteLine(e2.Message);
            }

            foreach (string val in GetResponse(SeleniumDriver))
            {
                Console.WriteLine(val);
            }
        }
    }

}