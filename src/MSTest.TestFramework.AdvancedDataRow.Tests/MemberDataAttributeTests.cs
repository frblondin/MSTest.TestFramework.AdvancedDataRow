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
    public class MemberDataAttributeTests
    {
        public class DummyException : Exception { public DummyException(string message) : base(message) { } }

        [TestClass]
        public class UsesMemberData
        {
            public static object[][] PropertyData { get { return new object[][] { new object[] { "foo" }, new object[] { "bar" } }; } }
            [AdvancedDataTestMethod, Ignore]
            [MemberData(nameof(PropertyData))]
            public void UsingInlinePropertyData(string value)
            {
                throw new DummyException(value);
            }

            public static object[][] FieldData = new object[][] { new object[] { "foo" }, new object[] { "bar" } };
            [AdvancedDataTestMethod, Ignore]
            [MemberData(nameof(FieldData))]
            public void UsingInlineFieldData(string value)
            {
                throw new DummyException(value);
            }

            public static object[][] MethodData(string val1, string val2) { return new object[][] { new object[] { val1 }, new object[] { val2 } }; }
            [AdvancedDataTestMethod, Ignore]
            [MemberData(nameof(MethodData), new object[] { "foo", "bar" })]
            public void UsingInlineMethodData(string value)
            {
                throw new DummyException(value);
            }

            [AdvancedDataTestMethod, Ignore]
            [MemberData("UnexistingMember")]
            public void UnexistingMemberData(string value)
            {
            }
        }

        [TestMethod]
        public void MemberDataTestMethodIsDiscovered()
        {
            var testCases = typeof(UsesMemberData).Assembly.DiscoverTests();
            var name = $"{typeof(UsesMemberData).FullName}.{nameof(UsesMemberData.UsingInlinePropertyData)}";
            Assert.IsTrue(testCases.Any(tc => tc.FullyQualifiedName == name), $"{nameof(UsesMemberData.UsingInlinePropertyData)} could not be resolved.");
        }

        [TestMethod]
        public void PropertyDataTestMethodContainsData()
        {
            var testCase = typeof(UsesMemberData).Assembly.DiscoverTests(typeof(UsesMemberData), nameof(UsesMemberData.UsingInlinePropertyData));
            var method = typeof(UsesMemberData).GetMethod(nameof(UsesMemberData.UsingInlinePropertyData));
            var testMethod = testCase.ToTestMethod();
            var theoryAttribute = method.GetCustomAttribute<AdvancedDataTestMethodAttribute>();
            var results = theoryAttribute.Execute(testMethod);
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("foo", results[0].TestFailureException?.Message);
            Assert.AreEqual("bar", results[1].TestFailureException?.Message);
        }

        [TestMethod]
        public void FieldDataTestMethodContainsData()
        {
            var testCase = typeof(UsesMemberData).Assembly.DiscoverTests(typeof(UsesMemberData), nameof(UsesMemberData.UsingInlineMethodData));
            var method = typeof(UsesMemberData).GetMethod(nameof(UsesMemberData.UsingInlineMethodData));
            var testMethod = testCase.ToTestMethod();
            var theoryAttribute = method.GetCustomAttribute<AdvancedDataTestMethodAttribute>();
            var results = theoryAttribute.Execute(testMethod);
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("foo", results[0].TestFailureException?.Message);
            Assert.AreEqual("bar", results[1].TestFailureException?.Message);
        }

        [TestMethod]
        public void UnexistingMemberDataTestThrowsException()
        {
            var testCase = typeof(UsesMemberData).Assembly.DiscoverTests(typeof(UsesMemberData), nameof(UsesMemberData.UnexistingMemberData));
            var method = typeof(UsesMemberData).GetMethod(nameof(UsesMemberData.UnexistingMemberData));
            var testMethod = testCase.ToTestMethod();
            var theoryAttribute = method.GetCustomAttribute<AdvancedDataTestMethodAttribute>();
            Assert.ThrowsException<ArgumentException>(() => theoryAttribute.Execute(testMethod));
        }
    }
}
