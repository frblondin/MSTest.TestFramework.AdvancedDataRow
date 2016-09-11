using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTest.TestFramework.AdvancedDataRow
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MemberDataAttribute : DataAttribute
    {
        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string MemberName { get; private set; }
        /// <summary>
        /// Gets or sets the type to retrieve the member from. If not set, then the property will be
        /// retrieved from the unit test class.
        /// </summary>
        public Type MemberType { get; set; }
        /// <summary>
        /// Gets or sets the parameters passed to the member. Only supported for static methods.
        /// </summary>
        public object[] Parameters { get; private set; }

        public MemberDataAttribute(string memberName, params object[] parameters)
        {
            MemberName = memberName;
            Parameters = parameters;
        }

        public override IEnumerable<object[]> GetData(ITestMethod testMethod)
        {
            var type = MemberType ?? testMethod.MethodInfo.ReflectedType;
            IEnumerable<object[]> result = null;
            if ((Parameters?.Length ?? 0) == 0)
            {
                result = TryGetPropertyData(type) ?? TryGetFieldData(type);
            }
            if (result == null)
            {
                result = TryGetMethodData(type);
            }
            if (result == null)
            {
                var arguments = Parameters != null ? $"({string.Join(", ", Parameters)})" : "";
                throw new ArgumentException($"Could not find public static member (property, field, or method) named '{MemberName}' on {type.FullName}{arguments}.");
            }
            return result;
        }

        private IEnumerable<object[]> TryGetPropertyData(Type type)
        {
            return (IEnumerable<object[]>)type.GetProperty(MemberName)?.GetValue(null, null);
        }
        private IEnumerable<object[]> TryGetFieldData(Type type)
        {
            return (IEnumerable<object[]>)type.GetField(MemberName)?.GetValue(null);
        }
        private IEnumerable<object[]> TryGetMethodData(Type type)
        {
            return (IEnumerable<object[]>)type.GetMethod(MemberName)?.Invoke(null, Parameters);
        }
    }
}
