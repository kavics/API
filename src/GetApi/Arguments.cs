using SenseNet.Tools.CommandLineArguments;
using System.IO;
using System.Text.RegularExpressions;

namespace Kavics.ApiExplorer.GetApi
{
    internal class Arguments
    {
        [NoNameOption(0, true, "source", "Path of the directory containing assemblies.")]
        public string SourceDirectory { get; set; }

        [NoNameOption(1, false, "target", "Path of output file. Default: <source\\api.txt>")]
        private string TargetFileArg { get; set; }
        private string _targetFile;
        public string TargetFile =>
            _targetFile ?? (_targetFile = TargetFileArg != null
                ? Path.GetFullPath(TargetFileArg)
                : Path.Combine(SourceDirectory, "api.txt"));

        [CommandLineArgument("AllInternals", false, "i", "Shows internal classes and members")]
        public bool AllInternals { get; set; }

        [CommandLineArgument("InternalMembers", false, "im", "Shows internal members of public classes.")]
        public bool InternalMembers { get; set; }

        [CommandLineArgument("ContentHandlers", false, "ch", "Shows only ContentHandler classes of the sensenet.")]
        public bool ContentHandlerFilter { get; set; }

        [CommandLineArgument("OData", false, "o,od", "Shows only OData functions and actions of the sensenet.")]
        public bool OdataFilter { get; set; }

        private string _namespaceFilterArg;
        [CommandLineArgument("Namespace", false, "n,ns", "Valid regex that filters the namespaces. For example: \".*sensenet..*\"")]
        internal string NamespaceFilterArg
        {
            get => _namespaceFilterArg;
            set
            {
                _namespaceFilterArg = value;
                NamespaceFilter = string.IsNullOrEmpty(value) ? null : new Regex(value, RegexOptions.IgnoreCase);
            }
        }

        public Regex NamespaceFilter { get; private set; }
    }
}