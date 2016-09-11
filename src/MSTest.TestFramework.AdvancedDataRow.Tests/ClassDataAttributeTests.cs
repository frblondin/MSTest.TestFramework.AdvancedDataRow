using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.TestAdapter.XUnitLookAlike.Tests.Extensions;
using MSTest.TestFramework.AdvancedDataRow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MSTest.TestAdapter.XUnitLookAlike.Tests
{
    [TestClass]
    public class ClassDataAttributeTests
    {
        public class DummyException : Exception { public DummyException(string message) : base(message) { } }

        public class Data : List<object[]>
        {
            public Data() : base(new[] { new object[] { "foo" }, new object[] { "bar" } }) { }
        }

        [TestClass]
        public class UsesClassData
        {
            public static object[][] PropertyData { get { return new object[][] { new object[] { "foo" }, new object[] { "bar" } }; } }
            [AdvancedDataTestMethod, Ignore]
            [ClassData(typeof(Data))]
            public void UsingClassData(string value)
            {
                throw new DummyException(value);
            }
        }

        [TestMethod]
        public void ClassDataTestMethodContainsData()
        {
            var testCase = typeof(UsesClassData).Assembly.DiscoverTests(typeof(UsesClassData), nameof(UsesClassData.UsingClassData));
            var method = typeof(UsesClassData).GetMethod(nameof(UsesClassData.UsingClassData));
            var testMethod = testCase.ToTestMethod();
            var theoryAttribute = method.GetCustomAttribute<AdvancedDataTestMethodAttribute>();
            var results = theoryAttribute.Execute(testMethod);
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("foo", results[0].TestFailureException?.Message);
            Assert.AreEqual("bar", results[1].TestFailureException?.Message);
        }
    }
}
