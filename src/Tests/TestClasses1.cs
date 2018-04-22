namespace Tests.TestClasses1
{
    public enum PubEnum { A, B, C }
    internal enum IntEnum { A, B, C }

    public class PubC
    {
        public class PubNCofPub { }
        internal class IntNCofPub { }
        private class PriNCofPub { }
    }

    internal class IntC
    {
        public class PubNCofInt { }
        internal class IntNCofInt { }
        private class PriNCofInt { }
    }

    public static class PubStC { }
    internal static class IntStC { }

    public abstract class PubAC { }
    internal abstract class IntAC { }

    public sealed class PubSeC { }
    internal sealed class IntSeC { }

    public interface IPub { }
    internal interface IInt { }

    public struct PubStruct { }
    internal struct IntStruct { }
}
