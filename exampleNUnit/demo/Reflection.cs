using System;
using NUnit.Framework;
using EcFeed;
using System.Reflection;

namespace exampleNUnit
{
    [TestFixture]
    public class ReflectionTest
    {
        private static Assembly assembly;
        private StructureInitializer initializer;

        [OneTimeSetUp]
        public static void GlobalSetup()
        {
            assembly = Assembly.GetExecutingAssembly();
        }

        [SetUp]
        public void LocalSetup()
        {
            initializer = Factory.GetStructureInitializer();
        }

        [Test]
        public void ListStructuresSingleTest()
        {
            initializer.Source(assembly, "Source");

            Assert.AreEqual(8, initializer.GetStructuresRaw().Count);
        }

        [Test]
        public void ListStructuresMultipleSequentialTest()
        {
            initializer.Source(assembly, "Source", "SourceExtension");

            Assert.AreEqual(9, initializer.GetStructuresRaw().Count);
        }

        [Test]
        public void ListStructuresDuplicateNameTest()
        {
            Assert.Throws<SystemException>(() => initializer.Source(assembly, "Source", "SourceErroneous"));
        }

        [Test]
        public void ListStructuresWrongNamespaceTest()
        {
            Assert.Throws<SystemException>(() => initializer.Source(assembly, "Source", "SourceError"));
        }

        [Test]
        public void ListStructuresNameSimpleTest()
        {
            initializer.Source(assembly, "Source", "SourceExtension");

            Assert.True(initializer.GetNamesSimpleRaw().Contains("Element1"));
            Assert.True(initializer.GetNamesSimpleRaw().Contains("Element1Nested1"));
            Assert.True(initializer.GetNamesSimpleRaw().Contains("Element1Nested2"));
            Assert.True(initializer.GetNamesSimpleRaw().Contains("Element1Nested3"));
            Assert.True(initializer.GetNamesSimpleRaw().Contains("Element2"));
            Assert.True(initializer.GetNamesSimpleRaw().Contains("Element2Nested1"));
            Assert.True(initializer.GetNamesSimpleRaw().Contains("Element2Nested2"));
            Assert.True(initializer.GetNamesSimpleRaw().Contains("Element2Nested3"));
            Assert.True(initializer.GetNamesSimpleRaw().Contains("Extension1"));
        }

        [Test]
        public void ListStructuresNameQualifiedTest()
        {
            initializer.Source(assembly, "Source", "SourceExtension");

            Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element1"));
            Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element1Nested1"));
            Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element1Nested2"));
            Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element1Nested3"));
            Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element2"));
            Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element2Nested1"));
            Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element2Nested2"));
            Assert.True(initializer.GetNamesQualifiedRaw().Contains("Source.Element2Nested3"));
            Assert.True(initializer.GetNamesQualifiedRaw().Contains("SourceExtension.Extension1"));
        }
    }
}