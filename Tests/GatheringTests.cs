using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpaceBender.ApiExplorer;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class GatheringTests
    {
        [TestMethod]
        public void Api()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter
            {
                WithInternals = false,
                WithInternalMembers = false,
                Namespace = ".*Tests.TestClasses.*"
            };
            var types = new Api(binPath, filter).GetTypes();

            var expected = "IPublicInterface, PublicAbstractClass, PublicClass, PublicEnum, PublicNestedClass1, PublicStaticClass, PublicStruct";
            var actual = string.Join(", ", types.Select(t => t.Name).OrderBy(s => s));

            Assert.AreEqual(expected, actual);
        }
    }
}
