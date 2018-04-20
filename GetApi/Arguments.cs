using SenseNet.Tools.CommandLineArguments;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Kavics.ApiExplorer.GetApi
{
    internal class Arguments
    {
        [NoNameOption(order: 0, required: true, nameInHelp: "source", helpText: "Path of the directory containing assemblies.")]
        private string SourceDirectoryArg { get; set; }
        private string _sourceDirectory;
        public string SourceDirectory
        {
            get
            {
                if (_sourceDirectory == null)
                    _sourceDirectory = Path.GetFullPath(SourceDirectoryArg ?? AppDomain.CurrentDomain.BaseDirectory);
                return _sourceDirectory;
            }
        }

        [NoNameOption(order: 1, required: false, nameInHelp: "target", helpText: "Path of output file. Default: <source\\api.txt>")]
        private string TargetFileArg { get; set; }
        private string _targetFile;
        public string TargetFile
        {
            get
            {
                if (_targetFile == null)
                {
                    _targetFile = TargetFileArg != null
                        ? Path.GetFullPath(TargetFileArg)
                        : Path.Combine(SourceDirectory, "api.txt");
                }
                return _targetFile;
            }
        }

        [CommandLineArgument(name: "allinternals", required: false, aliases: "i", helpText: "Shows internal classes and members")]
        public bool AllInternals { get; set; }

        [CommandLineArgument(name: "internalmembers", required: false, aliases: "im", helpText: "Shows internal members of public classes.")]
        public bool InternalMembers { get; set; }

        private string _namespaceFilterArg;
        [CommandLineArgument(name: "namespace", required: false, aliases: "n,ns", helpText: "Valid regex that filters the namespaces. For example: \".*sensenet..*\"")]
        private string NamespaceFilterArg
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