using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.TestFramework.AdvancedDataRow;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MSTest.TestAdapter.XUnitLookAlike.Tests.Extensions
{
    public static class TestCaseExtensions
    {
        public static ITestMethod ToTestMethod(this TestCase source)
        {
            var dotPos = source.FullyQualifiedName.LastIndexOf('.');
            var type = Type.GetType(source.FullyQualifiedName.Substring(0, dotPos));
            var method = type.GetMethod(source.FullyQualifiedName.Substring(dotPos + 1));
            return source.ToTestMethod(method);
        }

        public static ITestMethod ToTestMethod(this TestCase source, MethodInfo method)
        {
            var result = Substitute.For<ITestMethod>();
            result.MethodInfo.Returns(method);
            result.GetAttributes<DataRowAttribute>(false).Returns(method.GetCustomAttributes<DataRowAttribute>(false));
            result.GetAttributes<DataAttribute>(false).Returns(method.GetCustomAttributes<DataAttribute>(false));
            result.Invoke(Arg.Any<object[]>())
                .ReturnsForAnyArgs(x => GetTestResult(method, x.Arg<object[]>()));
            return result;
        }

        private static Microsoft.VisualStudio.TestTools.UnitTesting.TestResult GetTestResult(MethodInfo method, object[] data)
        {
            Exception exception = null;
            object returnValue = null;
            try
            {
                returnValue = method.Invoke(Activator.CreateInstance(method.ReflectedType), data);
            }
            catch (Exception e)
            {
                exception = (e as TargetInvocationException)?.InnerException ?? e;
            }
            return new Microsoft.VisualStudio.TestTools.UnitTesting.TestResult
            {
                TestFailureException = exception,
                Outcome = exception == null ? UnitTestOutcome.Passed : UnitTestOutcome.Failed,
                ReturnValue = returnValue
            };
        }
    }
}
