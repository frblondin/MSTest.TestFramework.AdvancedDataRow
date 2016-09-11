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
    public class InlineDataAttributeTests
    {
        public class DummyException : Exception { public DummyException(string message) : base(message) { } }

        [TestClass]
        public class UsesInlineData
        {
            public static object[][] PropertyData { get { return new object[][] { new object[] { "foo" }, new object[] { "bar" } }; } }
            [AdvancedDataTestMethod, Ignore]
            [InlineData("foo")]
            [InlineData("bar")]
            public void UsingInlineData(string value)
            {
                throw new DummyException(value);
            }
        }

        [TestMethod]
        public void InlineDataTestMethodContainsData()
        {
            var testCase = typeof(UsesInlineData).Assembly.DiscoverTests(typeof(UsesInlineData), nameof(UsesInlineData.UsingInlineData));
            var method = typeof(UsesInlineData).GetMethod(nameof(UsesInlineData.UsingInlineData));
            var testMethod = testCase.ToTestMethod();
            var theoryAttribute = method.GetCustomAttribute<AdvancedDataTestMethodAttribute>();
            var results = theoryAttribute.Execute(testMethod);
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("foo", results[0].TestFailureException?.Message);
            Assert.AreEqual("bar", results[1].TestFailureException?.Message);
        }
    }
}
