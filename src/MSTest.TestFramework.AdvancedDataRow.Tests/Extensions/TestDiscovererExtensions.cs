
using Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MSTest.TestAdapter.XUnitLookAlike.Tests.Extensions
{
    internal static class TestDiscovererExtensions
    {
        internal static IList<TestCase> DiscoverTests(this Assembly assembly)
        {
            var discoverer = new MSTestDiscoverer();
            var sink = Substitute.For<ITestCaseDiscoverySink>();
            var result = new List<TestCase>();
            sink.SendTestCase(Arg.Do<TestCase>(tc => result.Add(tc)));
            discoverer.DiscoverTests(
                new[] { assembly.Location },
                null,
                Substitute.For<IMessageLogger>(),
                sink);
            return result;
        }

        internal static TestCase DiscoverTests(this Assembly assembly, Type type, string testName)
        {
            return assembly.DiscoverTests().Single(tc => tc.FullyQualifiedName == $"{type.FullName}.{testName}");
        }
    }
}
