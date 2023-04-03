using EcFeed;

namespace exampleNUnit
{
    public class ConfigDefault
    {
        internal static bool PROD = true;
        internal static bool DEVELOP = false;

        internal static TestProvider GetTestProvider(bool prod) 
        {

            return prod ? GetTestProviderProd() : GetTestProviderDevelop();
        }

        internal static TestProvider GetTestProviderProd() 
        {
            return new TestProvider(MODEL_PROD, keyStorePath: KEYSTORE_PROD, generatorAddress: GENERATOR_ADDRESS_PROD);
        }

        internal static TestProvider GetTestProviderDevelop() 
        {
            return new TestProvider(MODEL_DEVELOP, keyStorePath: KEYSTORE_DEVELOP, generatorAddress: GENERATOR_ADDRESS_DEVELOP);
        }

        internal static string KEYSTORE_PROD = "C:\\Users\\kskor\\.ecfeed\\security.p12";
        internal static string KEYSTORE_DEVELOP = "C:\\Users\\kskor\\.ecfeed\\security_dev.p12";
        internal static string MODEL_DEVELOP = "QERK-K7BW-ME4G-W3TT-NT32";
        internal static string MODEL_PROD = "IMHL-K0DU-2U0I-J532-25J9";
        internal static string MODEL_DUMMY = "XXXX-XXXX-XXXX-XXXX-XXXX";
        internal static string GENERATOR_ADDRESS_PROD = "https://gen.ecfeed.com";
        internal static string GENERATOR_ADDRESS_DEVELOP = "https://develop-gen.ecfeed.com";
        internal static string KEYSTORE_PASSWORD = "changeit";
        internal static string F_TEST = "QuickStart.test";
        internal static string F_10x10 = "com.example.test.Playground.size_10x10";
        internal static string F_100x2 = "com.example.test.Playground.size_100x2";
        internal static string F_LOAN_2 = "com.example.test.LoanDecisionTest2.generateCustomerData";
        internal static string F_STRUCTURE = "TestStructure.generate";






               internal static TestProvider TestProvider = new TestProvider("IMHL-K0DU-2U0I-J532-25J9");
    }

}