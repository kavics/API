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
        private bool _withInternalTypes;
        private ApiType[] _types;

        private string BinPath { get; set; }

        public Api(string path)
        {
            BinPath = path;
        }

        public ApiType[] GetTypes()
        {
            if (_types == null)
            {
                foreach (var path in Directory.GetFiles(BinPath, "*.dll").Union(Directory.GetFiles(BinPath, "*.exe")))
                    Assembly.LoadFrom(path);

                Regex rx = new Regex("SpaceBender.*", RegexOptions.IgnoreCase);

                _types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => _withInternalTypes ? a.GetTypes() : a.GetExportedTypes(), (a, t) => t)
                    //.Where(x => (x.Namespace ?? "").StartsWith("SpaceBender", StringComparison.OrdinalIgnoreCase))
                    .Where(x => rx.IsMatch(x.Namespace))
                    .Select(t => new ApiType(t))
                    .OrderBy(a => a.Assembly)
                    .ThenBy(a => a.Namespace)
                    .ThenBy(a => a.Name)
                    .ToArray();
            }
            return _types;
        }
    }
}
