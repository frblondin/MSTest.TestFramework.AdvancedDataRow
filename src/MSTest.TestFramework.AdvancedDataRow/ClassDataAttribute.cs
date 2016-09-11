using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTest.TestFramework.AdvancedDataRow
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ClassDataAttribute : DataAttribute
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type { get; private set; }

        public ClassDataAttribute(Type type)
        {
            Type = type;
        }

        public override IEnumerable<object[]> GetData(ITestMethod testMethod)
        {
            return (IEnumerable<object[]>)Activator.CreateInstance(Type);
        }
    }
}
