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
        public void Api_PublicApi()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter
            {
                WithInternals = false,
                WithInternalMembers = false,
                Namespace = ".*Tests.TestClasses1.*"
            };
            var types = new Api(binPath, filter).GetTypes();

            var expected = "IPub, PubAC, PubC, PubEnum, PubNCofPub, PubSeC, PubStC, PubStruct";
            var actual = string.Join(", ", types.Select(t => t.Name).OrderBy(s => s));

            Assert.AreEqual(expected, actual);
        }
    }
}
/*
Expected:<IPub, PubAC, PubC, PubEnum, PubNCoP  , PubSeC, PubStC, PubStruct>
. Actual:<IPub, PubAC, PubC, PubEnum, PubNCoPub, PubSeC, PubStC, PubStruct>.

*/
