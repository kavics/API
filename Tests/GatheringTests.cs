using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Kavics.ApiExplorer;
using System.Text.RegularExpressions;

namespace Tests
{
    [TestClass]
    public class GatheringTests
    {
        [TestMethod]
        public void Api_Types_Public()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter { NamespaceFilter = new Regex(".*Tests.TestClasses1.*", RegexOptions.IgnoreCase) };
            var types = new Api(binPath, filter).GetTypes();

            var expected = "IPub, PubAC, PubC, PubEnum, PubNCofPub, PubSeC, PubStC, PubStruct";
            var actual = string.Join(", ", types.Select(t => t.Name).OrderBy(s => s));

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void Api_Types_All()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter { WithInternals = true, NamespaceFilter = new Regex(".*Tests.TestClasses1.*", RegexOptions.IgnoreCase) };
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
            var filter = new Filter { NamespaceFilter = new Regex(".*.TestClasses2.*", RegexOptions.IgnoreCase) };
            var types = new Api(binPath, filter).GetTypes();

            var expectedFields = "_protectedField, _protectedInternalField, _publicField";
            var actualFields = string.Join(", ", types[0].Fields.Select(f => f.Name).OrderBy(s => s));
            Assert.AreEqual(expectedFields, actualFields);

            var expectedProperties = "P1, P2, P4, P5, P6, P7";
            var actualProperties = string.Join(", ", types[0].Properties.Select(f => f.Name).OrderBy(s => s));
            Assert.AreEqual(expectedProperties, actualProperties);

            var expectedMethods = "M1, M2, M4, MA0, MA1, MA3, MS1, MS2, MS4, MV1, MV2, MV4, ToString";
            var actualMethods = string.Join(", ", types[0].Methods.Select(f => f.Name).OrderBy(s => s));
            Assert.AreEqual(expectedMethods, actualMethods);
        }
        [TestMethod]
        public void Api_Members_All()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter { NamespaceFilter = new Regex(".*.TestClasses2.*", RegexOptions.IgnoreCase), WithInternalMembers = true };
            var types = new Api(binPath, filter).GetTypes();

            var expectedFields = "_internalField, _privateField, _protectedField, _protectedInternalField, _publicField";
            var actualFields = string.Join(", ", types[0].Fields.Select(f => f.Name).OrderBy(s => s));
            Assert.AreEqual(expectedFields, actualFields);

            var expectedProperties = "P0, P1, P2, P3, P4, P5, P6, P7";
            var actualProperties = string.Join(", ", types[0].Properties.Select(f => f.Name).OrderBy(s => s));
            Assert.AreEqual(expectedProperties, actualProperties);

            var expectedMethods = "M0, M1, M2, M3, M4, MA0, MA1, MA2, MA3, MS0, MS1, MS2, MS3, MS4, MV1, MV2, MV3, MV4, ToString";
            var actualMethods = string.Join(", ", types[0].Methods.Select(f => f.Name).OrderBy(s => s));
            Assert.AreEqual(expectedMethods, actualMethods);
        }
    }
}
/*

*/
