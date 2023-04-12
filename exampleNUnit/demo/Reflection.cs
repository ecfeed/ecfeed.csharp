// using System;
// using NUnit.Framework;
// using EcFeed;
// using System.Reflection;
// using System.Collections.Generic;

// namespace exampleNUnit
// {
//     [TestFixture]
//     public class ReflectionTest
//     {
//         private static Assembly assembly;
//         private StructureInitializer initializer;

//         [OneTimeSetUp]
//         public static void GlobalSetup()
//         {
//             assembly = Assembly.GetExecutingAssembly();
//         }

//         [SetUp]
//         public void LocalSetup()
//         {
//             initializer = Factory.GetStructureInitializer();
//         }

//         [Test]
//         public void ListStructuresSingleTest()
//         {
//             initializer.Source(assembly, "Source");

//             Assert.AreEqual(8, initializer.GetStructuresRaw().Count);
//         }

//         [Test]
//         public void ListStructuresMultipleSequentialTest()
//         {
//             initializer.Source(assembly, "Source", "SourceExtension");

//             Assert.AreEqual(9, initializer.GetStructuresRaw().Count);
//         }

//         [Test]
//         public void ListStructuresDuplicateNameTest()
//         {
//             Assert.Throws<SystemException>(() => initializer.Source(assembly, "Source", "SourceErroneous"));
//         }

//         [Test]
//         public void ListStructuresWrongNamespaceTest()
//         {
//             Assert.Throws<SystemException>(() => initializer.Source(assembly, "Source", "SourceError"));
//         }

//         [Test]
//         public void ListStructuresNameSimpleTest()
//         {
//             initializer.Source(assembly, "Source", "SourceExtension");

//             Assert.True(initializer.GetNamesSimpleRaw().Contains("Element1"));
//             Assert.True(initializer.GetNamesSimpleRaw().Contains("Element1Nested1"));
//             Assert.True(initializer.GetNamesSimpleRaw().Contains("Element1Nested2"));
//             Assert.True(initializer.GetNamesSimpleRaw().Contains("Element1Nested3"));
//             Assert.True(initializer.GetNamesSimpleRaw().Contains("Element2"));
//             Assert.True(initializer.GetNamesSimpleRaw().Contains("Element2Nested1"));
//             Assert.True(initializer.GetNamesSimpleRaw().Contains("Element2Nested2"));
//             Assert.True(initializer.GetNamesSimpleRaw().Contains("Element2Nested3"));
//             Assert.True(initializer.GetNamesSimpleRaw().Contains("Extension1"));
//         }

//         [Test]
//         public void ListStructuresNameQualifiedTest()
//         {
//             initializer.Source(assembly, "Source", "SourceExtension");

//             Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element1"));
//             Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element1Nested1"));
//             Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element1Nested2"));
//             Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element1Nested3"));
//             Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element2"));
//             Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element2Nested1"));
//             Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element2Nested2"));
//             Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element2Nested3"));
//             Assert.True(initializer.GetNamesQualifiedRaw().Contains("SourceExtension.Extension1"));
//         }

// //---------------------------------------------------------------------------------------

//         [Test]
//         public void InstantiateSimpleClassTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element2(int,int,int)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2");
//             queue.Enqueue("3");

//             Source.Element2 element = initializer.Instantiate<Source.Element2>(queue);

//             Assert.AreEqual(1, element.a);
//             Assert.AreEqual(2, element.b);
//             Assert.AreEqual(3, element.c);
//         }

//         [Test]
//         public void InstantiateSimpleSignatureTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element2(int,int,int)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2");
//             queue.Enqueue("3");

//             var element = (Source.Element2) initializer.Instantiate("Element2", queue);

//             Assert.AreEqual(1, element.a);
//             Assert.AreEqual(2, element.b);
//             Assert.AreEqual(3, element.c);
//         }

//         [Test]
//         public void InstantiateNestedClassTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element1(int,double,String,Element2)");
//             initializer.Activate("Element2(int,int,int)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2.5");
//             queue.Enqueue("test");
//             queue.Enqueue("-1");
//             queue.Enqueue("-2");
//             queue.Enqueue("-3");

//             var element = initializer.Instantiate<Source.Element1>(queue);

//             Assert.AreEqual(1, element.a);
//             Assert.AreEqual(2.5, element.b);
//             Assert.AreEqual("test", element.c);
//             Assert.NotNull(element.d);
//             Assert.AreEqual(-1, element.d.a);
//             Assert.AreEqual(-2, element.d.b);
//             Assert.AreEqual(-3, element.d.c);
//         }

//         [Test]
//         public void InstantiateNestedSignatureTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element1(int,double,String,Element2)");
//             initializer.Activate("Element2(int,int,int)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2.5");
//             queue.Enqueue("test");
//             queue.Enqueue("-1");
//             queue.Enqueue("-2");
//             queue.Enqueue("-3");

//             var element = (Source.Element1) initializer.Instantiate("Element1", queue);

//             Assert.AreEqual(1, element.a);
//             Assert.AreEqual(2.5, element.b);
//             Assert.AreEqual("test", element.c);
//             Assert.NotNull(element.d);
//             Assert.AreEqual(-1, element.d.a);
//             Assert.AreEqual(-2, element.d.b);
//             Assert.AreEqual(-3, element.d.c);
//         }

// //---------------------------------------------------------------------------------------    

//         [Test]
//         public void InstantiateNestedNotActiveTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element1(int,double,String,Element2)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2.5");
//             queue.Enqueue("test");
//             queue.Enqueue("-1");
//             queue.Enqueue("-2");
//             queue.Enqueue("-3");

//             Assert.Throws<SystemException>(() => initializer.Instantiate<Source.Element1>(queue));
//         }

//         [Test]
//         public void InstantiateNestedWrongSourceTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element1(int,double,String,Element2)");
//             initializer.Activate("Element2(int,int,int)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1.5");
//             queue.Enqueue("2.5");
//             queue.Enqueue("test");
//             queue.Enqueue("-1");
//             queue.Enqueue("-2");
//             queue.Enqueue("-3");

//             Assert.Throws<SystemException>(() => initializer.Instantiate<Source.Element1>(queue));
//         }

//         [Test]
//         public void InstantiateNestedTooShortParameterListTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element1(int,double,String,Element2)");
//             initializer.Activate("Element2(int,int,int)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2.5");
//             queue.Enqueue("test");
//             queue.Enqueue("-1");
//             queue.Enqueue("-2");

//             Assert.Throws<SystemException>(() => initializer.Instantiate<Source.Element1>(queue));
//         }

//         [Test]
//         public void InstantiateNestedTooLongParameterListTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element1(int,double,String,Element2)");
//             initializer.Activate("Element2(int,int,int)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2.5");
//             queue.Enqueue("test");
//             queue.Enqueue("-1");
//             queue.Enqueue("-2");
//             queue.Enqueue("-3");
//             queue.Enqueue("-4");

//             Assert.Throws<SystemException>(() => initializer.Instantiate<Source.Element1>(queue));
//         }

// //---------------------------------------------------------------------------------------        

//         [Test]
//         public void GetTestCaseTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element1(int,double,String,Element2)");
//             initializer.Activate("Element2(int,int,int)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2.5");
//             queue.Enqueue("test");
//             queue.Enqueue("-1");
//             queue.Enqueue("-2");
//             queue.Enqueue("-3");
//             queue.Enqueue("ecFeed");

//             var testCase = initializer.GetTestCase("method(Element1,String)", queue);

//             Assert.AreEqual(2, testCase.Length);
//             Assert.NotNull(testCase[0]);
//             Assert.AreEqual(1, ((Source.Element1) testCase[0]).a);
//             Assert.AreEqual(2.5, ((Source.Element1) testCase[0]).b);
//             Assert.AreEqual("test", ((Source.Element1) testCase[0]).c);
//             Assert.NotNull(((Source.Element1) testCase[0]).d);
//             Assert.AreEqual(-1, ((Source.Element1) testCase[0]).d.a);
//             Assert.AreEqual(-2, ((Source.Element1) testCase[0]).d.b);
//             Assert.AreEqual(-3, ((Source.Element1) testCase[0]).d.c);
//             Assert.AreEqual("ecFeed", testCase[1]);
//         }

//         [Test]
//         public void GetTestCaseNotActiveTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element1(int,double,String,Element2)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2.5");
//             queue.Enqueue("test");
//             queue.Enqueue("-1");
//             queue.Enqueue("-2");
//             queue.Enqueue("-3");
//             queue.Enqueue("ecFeed");

//             Assert.Throws<SystemException>(() => initializer.GetTestCase("method(Element1,String)", queue));
//         }

//         [Test]
//         public void GetTestCaseTooShortParameterListTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element1(int,double,String,Element2)");
//             initializer.Activate("Element2(int,int,int)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2.5");
//             queue.Enqueue("test");
//             queue.Enqueue("-1");
//             queue.Enqueue("-2");
//             queue.Enqueue("-3");

//             Assert.Throws<SystemException>(() => initializer.GetTestCase("method(Element1,String)", queue));
//         }

//         [Test]
//         public void GetTestCaseTooLongParameterListTest()
//         {
//             initializer.Source(assembly, "Source");
//             initializer.Activate("Element1(int,double,String,Element2)");
//             initializer.Activate("Element2(int,int,int)");

//             var queue = new Queue<string>();
//             queue.Enqueue("1");
//             queue.Enqueue("2.5");
//             queue.Enqueue("test");
//             queue.Enqueue("-1");
//             queue.Enqueue("-2");
//             queue.Enqueue("-3");
//             queue.Enqueue("ecFeed");
//             queue.Enqueue("ecFeed");

//             Assert.Throws<SystemException>(() => initializer.GetTestCase("method(Element1,String)", queue));
//         }
//     }
// }