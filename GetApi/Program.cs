using SpaceBender.ApiExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBender.ApiExplorer.GetApi
{
    class Program
    {
        private static string _sourceDirectory = null; // first argument. default: appdomain
        private static string _output = null; // -o -output
        private static bool _withInternalTypes = true; // -i -internals
        //private static bool _internalTypesOnly = false; // -io -internalsonly

        static void Main(string[] args)
        {
            var binPath = _sourceDirectory ?? AppDomain.CurrentDomain.BaseDirectory;

            foreach (var path in Directory.GetFiles(binPath, "*.dll").Union(Directory.GetFiles(binPath, "*.exe")))
                Assembly.LoadFrom(path);

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => _withInternalTypes ? a.GetTypes() : a.GetExportedTypes(), (a, t) => t)
                .Where(x => (x.Namespace ?? "").StartsWith("SpaceBender", StringComparison.OrdinalIgnoreCase))
                .Select(t => new ApiType(t))
                .OrderBy(a => a.Assembly)
                .ThenBy(a => a.Namespace)
                .ThenBy(a => a.Name)
                .ToArray();

            //var relevantTypes = types.Where(t => t.Namespace.Contains(".Search") && !t.Namespace.Contains("Tests")).ToArray();
            //var relevantTypes = types.Where(t => t.IsContentHandler).ToArray();
            //var relevantTypes = types.Where(t => t.Namespace.StartsWith("SenseNet.ContentRepository.Storage.Data") || t.Name.Contains("DataProvider")).ToArray();
            var relevantTypes = types; //.Where(t => t.Name == "DataProvider" || t.BaseType == "DataProvider").ToArray();

            using (var writer = new StreamWriter(_output ?? $@"{binPath}\api.txt"))
            {
                Print(writer, relevantTypes, false);

                writer.WriteLine();
                writer.WriteLine("=================================================================================================");
                writer.WriteLine();
                writer.WriteLine("MEMBERS");
                writer.WriteLine();

                Print(writer, relevantTypes, true);
            }
        }

        private static void Print(TextWriter writer, ApiType[] relevantTypes, bool withMembers)
        {
            if (!withMembers)
            {
                foreach (var asm in relevantTypes.Select(t => t.Assembly).Distinct().OrderBy(x => x))
                    writer.WriteLine($"Assembly\t{asm}");

                foreach (var ns in relevantTypes.Select(t => t.Namespace).Distinct().OrderBy(x => x))
                    writer.WriteLine($"Namespace\t{ns}");
            }

            foreach (var t in relevantTypes.Where(t => t.IsEnum))
            {
                writer.WriteLine($"Enum\t{t.Assembly}\t{t.Namespace}\t{t.Name}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

            foreach (var t in relevantTypes.Where(t => t.IsInterface))
            {
                var baseType = (!string.IsNullOrEmpty(t.BaseType) && t.BaseType != "Object") ? ": " + t.BaseType : "";
                writer.WriteLine($"Interface\t{t.Assembly}\t{t.Namespace}\t{t.Name}\t{baseType}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

            foreach (var t in relevantTypes.Where(t => t.IsAbstractClass))
            {
                writer.WriteLine($"Abstract class\t{t.Assembly}\t{t.Namespace}\t{t.Name}\t{(t.BaseType != "Object" ? ": " + t.BaseType : "")}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

            foreach (var t in relevantTypes.Where(t => t.IsStaticClass))
            {
                writer.WriteLine($"Static class\t{t.Assembly}\t{t.Namespace}\t{t.Name}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

            foreach (var t in relevantTypes.Where(t => !t.IsEnum && !t.IsInterface && !t.IsAbstractClass && !t.IsStaticClass))
            {
                writer.WriteLine($"Class\t{t.Assembly}\t{t.Namespace}\t{t.Name}\t{(t.BaseType != "Object" ? ": " + t.BaseType : "")}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

        }

        private static void WriteMembers(TextWriter writer, ApiType t)
        {
            foreach (var item in t.Fields)
                writer.WriteLine("\t\t\t\t\t" + item);
            foreach (var item in t.Properties)
                writer.WriteLine("\t\t\t\t\t" + item);
            foreach (var item in t.Events)
                writer.WriteLine("\t\t\t\t\t" + item);
            foreach (var item in t.Constructors)
                writer.WriteLine("\t\t\t\t\t" + item);
            foreach (var item in t.Methods)
                writer.WriteLine("\t\t\t\t\t" + item);
            foreach (var item in t.NestecClasses)
                writer.WriteLine("\t\t\t\t\t" + item);
            foreach (var item in t.OtherMembers)
                writer.WriteLine("\t\t\t\t\t" + item);
        }
    }
}
