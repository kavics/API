using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Kavics.ApiExplorer
{
    public class Api
    {
        private string[] _errors;
        private ApiType[] _types;
        private readonly Filter _filter;

        private string BinPath { get; }

        public Api(string binPath, Filter filter = null)
        {
            BinPath = binPath.Trim('\\');
            _filter = filter ?? new Filter();
        }

        public ApiType[] GetTypes(out string[] errors)
        {
            var errorMessages = new List<string>();
            if (_types == null)
            {
                var assemblyPaths = Directory.GetFiles(BinPath, "*.dll").Union(Directory.GetFiles(BinPath, "*.exe")).ToArray();
                if (assemblyPaths.Length == 0)
                    throw new Exception("Source directory does not contain any dll or exe file.");

                //foreach (var path in assemblyPaths)
                //{
                //    var assembly = Assembly.LoadFrom(path);
                //    foreach(var type in assembly.GetTypes())
                //    {
                //        if (type.Name.StartsWith("<") || type.FullName.StartsWith("<"))
                //            continue;
                //        if(_typeCache.ContainsKey(type.FullName))
                //        {
                //            int q = 1;
                //        }
                //        _typeCache.Add(type.FullName, type);
                //    }
                //}
                foreach (var path in assemblyPaths)
                    Assembly.LoadFrom(path);

                var namespaceRegex = _filter.NamespaceFilter;

                var apiTypes = new List<ApiType>();
                var asms = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var asm in asms)
                {
                    if (Path.GetDirectoryName(asm.Location) == BinPath)
                    {
                        try
                        {
                            var types = _filter.WithInternals ? asm.GetTypes() : asm.GetExportedTypes();
                            foreach (var type in types)
                                if (!type.Name.StartsWith("<"))
                                    if (namespaceRegex == null || namespaceRegex.IsMatch(type.Namespace ?? ""))
                                        apiTypes.Add(new ApiType(type, _filter.WithInternalMembers));
                        }
                        catch(Exception e)
                        {
                            errorMessages.Add($"Error during processing assembly {asm.FullName}: {e.Message}");
                        }
                    }
                }

                var filteredApiTypes = _filter.ContentHandlerFilter
                    ? apiTypes.Where(t => t.IsContentHandler)
                    : apiTypes;

                filteredApiTypes = filteredApiTypes
                    .OrderBy(a => a.Assembly)
                    .ThenBy(a => a.Namespace)
                    .ThenBy(a => a.Name);

                _types = filteredApiTypes.ToArray();
                _errors = errorMessages.ToArray();
            }
            errors = _errors;
            return _types;
        }

        public static string GetTypeName(Type type)
        {
            var origType = type;
            var typeInfo = type.GetTypeInfo();
            if (!type.IsGenericType)
            {
                var name = type.Name;
                if (!name.Contains('`'))
                    return GetSimpleName(type.Name);
                if(type.FullName == null)
                    return origType.Name;
                type = Type.GetType(type.FullName.Replace("&", ""));
                if (type == null)
                    return origType.Name;
            }
            var baseName = GetSimpleName(type.Name.Split('`')[0]);
            var genericPart = type.GenericTypeArguments.Length > 0
                ? string.Join(", ", type.GenericTypeArguments.Select(GetTypeName).ToArray())
                : string.Join(", ", typeInfo.GenericTypeParameters.Select(GetTypeName).ToArray());
            return $"{baseName}<{genericPart}>";
        }

        public static string GetSimpleName(string name)
        {
            switch (name)
            {
                default: return name;
                case "String": return "string";
                case "SByte": return "sbyte";
                case "Byte": return "byte";
                case "Boolean": return "bool";
                case "Char": return "char";
                case "Int16": return "short";
                case "UInt16": return "ushort";
                case "Int32": return "int";
                case "UInt32": return "uint";
                case "Int64": return "long";
                case "UInt64": return "ulong";
                case "Single": return "float";
                case "Double": return "double";
                case "Decimal": return "decimal";
                case "Void": return "void";
            }
        }

    }
}
