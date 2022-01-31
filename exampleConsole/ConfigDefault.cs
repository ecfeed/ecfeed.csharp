using EcFeed;

namespace Testify.EcFeed.Example
{
    public class ConfigDefault
    {

        internal static string MODEL_DUMMY = "XXXX-XXXX-XXXX-XXXX-XXXX";
        internal static string MODEL_PROD = "IMHL-K0DU-2U0I-J532-25J9";
        internal static string MODEL_DEVELOP = "QERK-K7BW-ME4G-W3TT-NT32";
        internal static TestProvider TestProviderProd = new TestProvider(MODEL_PROD, keyStorePath: "/home/krzysztof/.ecfeed/security_prod.p12");
        internal static TestProvider TestProviderDevelop = new TestProvider(MODEL_DEVELOP, generatorAddress: "https://develop-gen.ecfeed.com");
        internal static TestProvider GetTestProvider(bool prod = false)
        {
            return prod ? TestProviderProd : TestProviderDevelop;
        }

        internal static string F_LOAN_2 = "com.example.test.LoanDecisionTest2.generateCustomerData";
        internal static string F_10x10 = "com.example.test.Playground.size_10x10";
        internal static string F_100x2 = "com.example.test.Playground.size_100x2";
        internal static string F_TEST = "QuickStart.test";
    }

}