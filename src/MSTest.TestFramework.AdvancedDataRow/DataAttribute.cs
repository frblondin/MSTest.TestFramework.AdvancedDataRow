using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTest.TestFramework.AdvancedDataRow
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class DataAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public IEnumerable<TestResult> RunDataDrivenTest(ITestMethod testMethod)
        {
            var memberData = GetData(testMethod);
            foreach (var data in memberData)
            {
                var testResult = testMethod.Invoke(data);
                if (!string.IsNullOrEmpty(DisplayName))
                {
                    testResult.DisplayName = DisplayName;
                }
                yield return testResult;
            }
        }

        public abstract IEnumerable<object[]> GetData(ITestMethod testMethod);
    }
}
