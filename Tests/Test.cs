using AssemblyExplorer.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class Test
    {
        private static AssemblyModel res;

        [TestInitialize]
        public void TestInitialize()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            basePath = Directory.GetParent(basePath).FullName;
            basePath = Directory.GetParent(basePath).FullName;
            basePath = Directory.GetParent(basePath).FullName;
            basePath = Directory.GetParent(basePath).FullName;
            string filePath = Path.Combine(basePath, "Resources", "MPP_3_TEST.dll");
            res = new AssemblyModel(Assembly.LoadFrom(filePath));
        }

        [TestMethod]
        public void AssemlyNameTest()
        {
            res.name.Should().Be("MPP_3_TEST");
        }

        [TestMethod]
        public void NamespaceNameTest()
        {
            res.namespaces.Count.Should().Be(1);
            res.namespaces[0].name.Should().Be("-");
        }

        [TestMethod]
        public void ClassesNameTest()
        {
            var classes = res.namespaces[0].classes;
            classes.Count.Should().Be(3);
            classes[0].name.Should().Be("Program");
            classes[1].name.Should().Be("TestClass");
            classes[2].name.Should().Be("MyExtensions");
        }

        [TestMethod]
        public void MethodSignatureTest()
        {
            var testClass = res.namespaces[0].classes[1];
            testClass._methods.Count.Should().Be(2);
            testClass._methods[0].ToString().Should().Be("protected aasd(ref Int32 asdas, in Int32 n, out Int32 a, Int32[] b = null, params Int32[] p) : Int32");
        }

        [TestMethod]
        public void PropertySignatureTest()
        {
            var testClass = res.namespaces[0].classes[1];
            testClass._properties.Count.Should().Be(1);
            testClass._properties[0].ToString().Should().Be("protected internal Int32 prop { internal get; set; }");
        }

        [TestMethod]
        public void FieldSignatureTest()
        {
            var testClass = res.namespaces[0].classes[1];
            testClass._fields.Count.Should().Be(1);
            testClass._fields[0].ToString().Should().Be("public field : Dictionary<Int32, List<Dictionary<Int32, Int32>>>");
        }

        [TestMethod]
        public void EventSignatureTest()
        {
            var testClass = res.namespaces[0].classes[1];
            testClass._events.Count.Should().Be(1);
            testClass._events[0].ToString().Should().Be("protected Notify{ add {} remove {} } : AccountHandler");
        }

        [TestMethod]
        public void InnerClassSignatureTest()
        {
            var testClass = res.namespaces[0].classes[1];
            testClass.innerClasses.Count.Should().Be(2);
            testClass.innerClasses[0].name.Should().Be("InnerClass");
            testClass.innerClasses[1].name.Should().Be("AccountHandler");
            var testClass2 = testClass.innerClasses[0];
            testClass2.innerClasses.Count.Should().Be(1);
            testClass2.innerClasses[0].name.Should().Be("En");
        }

        [TestMethod]
        public void ExtensionSignatureTest()
        {
            var testClass = res.namespaces[0].classes[1];
            testClass._methods.Count.Should().Be(2);
            testClass._methods[1].ToString().Should().Be("extended from MyExtensions public static WordCount(TestClass str) : Int32");
        }
    }
}