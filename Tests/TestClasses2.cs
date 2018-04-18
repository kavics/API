using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestClasses2
{
    public abstract class PublicClass1
    {
        private int _privateField;
        protected int _protectedField;
        protected internal int _protectedInternalField;
        internal int _internalField;
        public int _publicField;

        private int P0 { get; set; }
        protected int P1 { get; set; }
        protected internal int P2 { get; set; }
        internal int P3 { get; set; }
        public int P4 { get; set; }

        public abstract int P5 { get; set; }
        public virtual int P6 { get; set; }
        public static int P7 { get; set; }

        private PublicClass1(int a) { }
        protected PublicClass1(int a, int b) { }
        protected internal PublicClass1(int a, int b, int c) { }
        internal PublicClass1(int a, int b, int c, int d) { }
        public PublicClass1() { }

        private void M0() { }
        protected void M1() { }
        protected internal void M2() { }
        internal void M3() { }
        public void M4() { }

        protected abstract void MA0();
        protected internal abstract void MA1();
        internal abstract void MA2();
        public abstract void MA3();

        private static void MS0() { }
        protected static void MS1() { }
        protected static internal void MS2() { }
        internal static void MS3() { }
        public static void MS4() { }

        protected virtual void MV1() { }
        protected internal virtual void MV2() { }
        internal virtual void MV3() { }
        public virtual void MV4() { }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}