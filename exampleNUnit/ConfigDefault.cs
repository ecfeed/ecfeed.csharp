using EcFeed;

namespace exampleNUnit
{
    public class ConfigDefault
    {
        internal static string MODEL = "IMHL-K0DU-2U0I-J532-25J9";
        internal static TestProvider TestProvider = new TestProvider(MODEL);
        internal static string F_LOAN_2 = "com.example.test.LoanDecisionTest2.generateCustomerData";
        internal static string F_10x10 = "com.example.test.Playground.size_10x10";
        internal static string F_100x2 = "com.example.test.Playground.size_100x2";
        internal static string F_TEST = "QuickStart.test";
    }

}