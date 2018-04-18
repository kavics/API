using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kavics.ApiExplorer;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class MemberTests
    {
        [TestMethod]
        public void Api_OneMember_AbstractMethodIsNotVirtual()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter { Namespace = ".*.TestClasses2.*" };
            var types = new Api(binPath, filter).GetTypes();

            var members = types.SelectMany(a => a.Methods, (a, m) => m).Where(m => m.IsAbstract);
            if (!members.Any())
                Assert.Inconclusive("There is no any abstract method.");
            foreach (var member in members)
                Assert.IsFalse(member.IsVirtual, $"abstract {member.Name} is virtual.");
        }
        [TestMethod]
        public void Api_OneMember_AbstractPropertyIsNotVirtual()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter { Namespace = ".*.TestClasses2.*" };
            var types = new Api(binPath, filter).GetTypes();

            var members = types.SelectMany(a => a.Properties, (a, m) => m).Where(m => m.IsAbstract);
            if (!members.Any())
                Assert.Inconclusive("There is no any abstract ctor.");
            foreach (var member in members)
                Assert.IsFalse(member.IsVirtual, $"abstract {member.Name} is virtual.");
        }
    }
}
