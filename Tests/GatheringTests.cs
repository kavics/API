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
        public void Api_Types_Public()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter { Namespace = ".*Tests.TestClasses1.*" };
            var types = new Api(binPath, filter).GetTypes();

            var expected = "IPub, PubAC, PubC, PubEnum, PubNCofPub, PubSeC, PubStC, PubStruct";
            var actual = string.Join(", ", types.Select(t => t.Name).OrderBy(s => s));

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void Api_Types_All()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter { WithInternals = true, Namespace = ".*Tests.TestClasses1.*" };
            var types = new Api(binPath, filter).GetTypes();

            var expected = "IInt, IntAC, IntC, IntEnum, IntNCofInt, IntNCofPub, IntSeC, IntStC, IntStruct, IPub, " +
                           "PriNCofInt, PriNCofPub, " +
                           "PubAC, PubC, PubEnum, PubNCofInt, PubNCofPub, PubSeC, PubStC, PubStruct";
            var actual = string.Join(", ", types.Select(t => t.Name).OrderBy(s => s));

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void Api_Members_Public()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter { Namespace = ".*.TestClasses2.*" };
            var types = new Api(binPath, filter).GetTypes();

            var expected = "_protectedField, _protectedInternalField, _publicField";
            var actual = string.Join(", ", types[0].Fields.Select(f => f.Name).OrderBy(s => s));

            Assert.AreEqual(expected, actual);

            Assert.Inconclusive(" ?? property, method, etc?");
        }
    }
}
