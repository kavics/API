using SenseNet.Tools.CommandLineArguments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBender.ApiExplorer.GetApi
{
    internal class Arguments
    {
        [NoNameOption(order: 0, required: true, nameInHelp: "source", helpText: "Path of the directory containing assemblies.")]
        private string sourceDirectoryArg { get; set; }
        private string _sourceDirectory;
        public string SourceDirectory
        {
            get
            {
                if (_sourceDirectory == null)
                    _sourceDirectory = Path.GetFullPath(sourceDirectoryArg);
                return _sourceDirectory;
            }
        }

        [NoNameOption(order: 1, required: false, nameInHelp: "target", helpText: "Path of output file. Default: <source\\api.txt>")]
        private string targetFileArg { get; set; }
        private string _targetFile;
        public string TargetFile
        {
            get
            {
                if (_targetFile == null)
                {
                    _targetFile = targetFileArg != null
                        ? Path.GetFullPath(targetFileArg)
                        : Path.Combine(SourceDirectory, "api.txt");
                }
                return _targetFile;
            }
        }

        [CommandLineArgument(name: "allinternals", required: false, aliases: "i", helpText: "Shows internal classes and members")]
        public bool AllInternals { get; set; }
        [CommandLineArgument(name: "internalmembers", required: false, aliases: "im", helpText: "Shows internal members of public classes.")]
        public bool InternalMembers { get; set; }
    }
}
