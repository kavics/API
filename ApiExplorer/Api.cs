using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Kavics.ApiExplorer
{
    public class Api
    {
        private ApiType[] _types;
        private Filter _filter;

        private string BinPath { get; set; }

        public Api(string binPath, Filter filter = null)
        {
            BinPath = binPath.Trim('\\');
            this._filter = filter ?? new Filter();
        }

        public ApiType[] GetTypes()
        {
            if (_types == null)
            {
                foreach (var path in Directory.GetFiles(BinPath, "*.dll").Union(Directory.GetFiles(BinPath, "*.exe")))
                    Assembly.LoadFrom(path);

                Regex namespaceRegex = null;
                if(_filter.Namespace != null)
                    namespaceRegex = new Regex(_filter.Namespace, RegexOptions.IgnoreCase);

                var asms = AppDomain.CurrentDomain.GetAssemblies();
                var relevantAsms = asms.Where(a => Path.GetDirectoryName(a.Location) == BinPath).ToArray();
                var types = relevantAsms
                    .SelectMany(a => _filter.WithInternals ? a.GetTypes() : a.GetExportedTypes(), (a, t) => t);
                //.Where(t => !t.Name.StartsWith("<")) // skip auto implementations e.g. "<>c__DisplayClass29_0"
                if (namespaceRegex != null)
                    types = types.Where(x => namespaceRegex.IsMatch(x.Namespace ?? ""));
                var apiTypes = types
                    .Select(t => new ApiType(t, _filter.WithInternalMembers))
                    .OrderBy(a => a.Assembly)
                    .ThenBy(a => a.Namespace)
                    .ThenBy(a => a.Name);
                _types = apiTypes.ToArray();
            }
            return _types;
        }
    }
}
