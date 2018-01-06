using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestClasses
{
    public enum PublicEnum { A, B, C }
    internal enum InternalEnum { A, B, C }

    public class PublicClass
    {
        public class PublicNestedClass1 { }
        internal class InternalNestedClass1 { }
        private class PrivateNestedClass1 { }
    }

    internal class InternalClass
    {
        public class PublicNestedClass2 { }
        internal class InternalNestedClass2 { }
        private class PrivateNestedClass2 { }
    }

    public static class PublicStaticClass { }
    internal static class InternalStaticClass { }

    public abstract class PublicAbstractClass { }
    internal abstract class InternalAbstractClass { }

    //public sealed class PublicSealedClass { }
    //internal sealed class InternalSealedClass { }

    public interface IPublicInterface { }
    internal interface IInternalInterface { }

    public struct PublicStruct { }
    internal struct InternalStruct { }
}
