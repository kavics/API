using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpaceBender.ApiExplorer
{
    public class Api
    {
        private ApiType[] _types;
        private bool _withInternals;
        private bool _withInternalMembers;

        private string BinPath { get; set; }

        public Api(string path, bool withInternals, bool withInternalMembers)
        {
            _withInternals = withInternals;
            _withInternalMembers = withInternalMembers;
            BinPath = path;
        }

        public ApiType[] GetTypes()
        {
            if (_types == null)
            {
                foreach (var path in Directory.GetFiles(BinPath, "*.dll").Union(Directory.GetFiles(BinPath, "*.exe")))
                    Assembly.LoadFrom(path);

                //Regex rx = new Regex("SpaceBender.*", RegexOptions.IgnoreCase);
                var asms = AppDomain.CurrentDomain.GetAssemblies();
                var relevantAsms = asms.Where(a => Path.GetDirectoryName(a.Location) == BinPath).ToArray();
                _types = relevantAsms
                    .SelectMany(a => _withInternals ? a.GetTypes() : a.GetExportedTypes(), (a, t) => t)
                    //.Where(t => !t.Name.StartsWith("<")) // skip auto implementations e.g. "<>c__DisplayClass29_0"
                    //.Where(x => rx.IsMatch(x.Namespace ?? ""))
                    .Select(t => new ApiType(t, _withInternalMembers))
                    .OrderBy(a => a.Assembly)
                    .ThenBy(a => a.Namespace)
                    .ThenBy(a => a.Name)
                    .ToArray();
            }
            return _types;
        }
    }
}
