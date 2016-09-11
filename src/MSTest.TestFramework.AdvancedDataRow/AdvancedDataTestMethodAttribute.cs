using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTest.TestFramework.AdvancedDataRow
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AdvancedDataTestMethodAttribute : TestMethodAttribute
    {
        private static TestResult[] RunDataDrivenTest(ITestMethod testMethod, DataAttribute[] attributes)
        {
            return attributes.SelectMany(a => a.RunDataDrivenTest(testMethod)).ToArray();
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var attributes = testMethod.GetAttributes<DataAttribute>(false);
            if (attributes == null || attributes.Length == 0)
            {
                return base.Execute(testMethod);
            }
            return RunDataDrivenTest(testMethod, attributes);
        }
    }
}
