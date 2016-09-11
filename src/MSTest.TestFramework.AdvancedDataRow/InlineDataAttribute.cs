using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTest.TestFramework.AdvancedDataRow
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InlineDataAttribute : DataAttribute
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        public object[] Data { get; private set; }

        public InlineDataAttribute(params object[] data)
        {
            Data = data;
        }

        public override IEnumerable<object[]> GetData(ITestMethod testMethod)
        {
            return new object[][] { Data };
        }
    }
}
