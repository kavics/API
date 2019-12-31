using System.Text.RegularExpressions;

namespace Kavics.ApiExplorer
{
    /// <summary>
    /// Filters the types and members when assemblies are scanned.
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Regex for filter namespace of a type. The type list will contain only matching types.
        /// For example ".*Kavics.*" means types that's namespace contains the "Kavics".
        /// </summary>
        public Regex NamespaceFilter { get; set; }

        /// <summary>
        /// The type list will contain only ContentHandler classes of the sensenet.
        /// </summary>
        public bool ContentHandlerFilter { get; set; }

        /// <summary>
        /// Adds internal types to the type list.
        /// </summary>
        public bool WithInternals { get; set; }

        private bool _withInternalMembers;
        /// <summary>
        /// Adds internal or private members to the member list of a type.
        /// The value is true if the WithInternals property is true.
        /// </summary>
        public bool WithInternalMembers
        {
            get => WithInternals || _withInternalMembers;
            set => _withInternalMembers = value;
        }
    }
}
