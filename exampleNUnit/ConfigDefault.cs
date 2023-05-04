using EcFeed;

namespace exampleNUnit
{
    public class ConfigDefault
    {
        internal enum Stage
        {
            PROD,
            DEVELOP,
            LOCAL
        }

        internal static TestProvider GetTestProvider(Stage stage) 
        {
            switch (stage)
            {
                case Stage.PROD: return GetTestProviderProd();
                case Stage.DEVELOP: return GetTestProviderDevelop();
                case Stage.LOCAL: return GetTestProviderLocal();
            }

            return null;
        }

        internal static TestProvider GetTestProviderProd() 
        {
            return new TestProvider(MODEL_PROD, keyStorePath: KEYSTORE_PROD, generatorAddress: GENERATOR_ADDRESS_PROD);
        }

        internal static TestProvider GetTestProviderDevelop() 
        {
            return new TestProvider(MODEL_DEVELOP, keyStorePath: KEYSTORE_DEVELOP, generatorAddress: GENERATOR_ADDRESS_DEVELOP);
        }

        internal static TestProvider GetTestProviderLocal()
        {
            return new TestProvider(MODEL_LOCAL, keyStorePath: KEYSTORE_LOCAL, generatorAddress: GENERATOR_ADDRESS_LOCAL);
        }

        internal static string KEYSTORE_PROD = "C:\\Users\\kskor\\.ecfeed\\security.p12";
        internal static string KEYSTORE_DEVELOP = "C:\\Users\\kskor\\.ecfeed\\security_dev.p12";
        internal static string KEYSTORE_LOCAL = "C:\\Users\\kskor\\.ecfeed\\security_rap.p12";
        internal static string MODEL_PROD = "IMHL-K0DU-2U0I-J532-25J9";
        internal static string MODEL_DEVELOP = "QERK-K7BW-ME4G-W3TT-NT32";
        internal static string MODEL_LOCAL = "TestUuid11";
        internal static string MODEL_DUMMY = "XXXX-XXXX-XXXX-XXXX-XXXX";
        internal static string GENERATOR_ADDRESS_PROD = "https://gen.ecfeed.com";
        internal static string GENERATOR_ADDRESS_DEVELOP = "https://develop-gen.ecfeed.com";
        internal static string GENERATOR_ADDRESS_LOCAL = "https://localhost:8090";
        internal static string KEYSTORE_PASSWORD = "changeit";
        internal static string F_TEST = "QuickStart.test";
        internal static string F_10x10 = "com.example.test.Playground.size_10x10";
        internal static string F_100x2 = "com.example.test.Playground.size_100x2";
        internal static string F_LOAN_2 = "com.example.test.LoanDecisionTest2.generateCustomerData";
        internal static string F_STRUCTURE = "TestStructure.generate";
    }

}