using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kavics.ApiExplorer;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class MemberTests
    {
        [TestMethod]
        public void Api_OneMember_AbstractMethodIsNotVirtual()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter { NamespaceFilter = new Regex(".*.TestClasses2.*", RegexOptions.IgnoreCase)  };
            var types = new Api(binPath, filter).GetTypes(out _);

            var members = types.SelectMany(a => a.Methods, (a, m) => m).Where(m => m.IsAbstract).ToArray();
            if (!members.Any())
                Assert.Inconclusive("There is no any abstract method.");
            foreach (var member in members)
                Assert.IsFalse(member.IsVirtual, $"abstract {member.Name} is virtual.");
        }
        [TestMethod]
        public void Api_OneMember_AbstractPropertyIsNotVirtual()
        {
            var binPath = AppDomain.CurrentDomain.BaseDirectory;
            var filter = new Filter { NamespaceFilter = new Regex(".*.TestClasses2.*", RegexOptions.IgnoreCase) };
            var types = new Api(binPath, filter).GetTypes(out _);

            var members = types.SelectMany(a => a.Properties, (a, m) => m).Where(m => m.IsAbstract).ToArray();
            if (!members.Any())
                Assert.Inconclusive("There is no any abstract ctor.");
            foreach (var member in members)
                Assert.IsFalse(member.IsVirtual, $"abstract {member.Name} is virtual.");
        }

        [TestMethod]
        public void Api_TypeName_GenericOutParameter()
        {
            var method = GetType().GetMethod("TestMethod1");
            var names = method.GetParameters().Select(p => Api.GetTypeName(p.ParameterType)).ToArray();
            Assert.AreEqual("List<string>", names[0]);
            Assert.AreEqual("List<string>", names[1]);
        }

        // ReSharper disable once UnusedMember.Global
        public void TestMethod1(List<string> prm, out List<string> outPrm) { outPrm = null; }
    }
}
