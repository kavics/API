using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestClasses2
{
    public abstract class PublicClass
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
        public static int P8 { get; set; }
    }
}
