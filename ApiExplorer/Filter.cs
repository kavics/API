using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBender.ApiExplorer
{
    /// <summary>
    /// Filters the types and members when assemlies are scanned.
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Regex for filter namespace of a type. The typelist will contain only matching types.
        /// For example ".*SpaceBender.*" means types that's namespace contains the "SpaceBender".
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Adds internal types to the type list.
        /// </summary>
        public bool WithInternals { get; set; }

        private bool _withInternalMembers;
        /// <summary>
        /// Adds internal or private members to the member list of a type.
        /// The value is true if the WithInternals property is true.
        /// </summary>
        public bool WithInternalMembers {
            get { return WithInternals || _withInternalMembers; }
            set { _withInternalMembers = value; }
        }
    }
}
