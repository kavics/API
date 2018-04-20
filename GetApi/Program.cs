using SenseNet.Tools.CommandLineArguments;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Kavics.ApiExplorer.GetApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var exit = false;
            var arguments = new Arguments();
            ArgumentParser parser;
            try
            {
                parser = ArgumentParser.Parse(args, arguments);
                if (parser.IsHelp)
                {
                    Console.WriteLine(parser.GetHelpText());
                    exit = true;
                }
            }
            catch (ParsingException e)
            {
                parser = ArgumentParser.Parse(new[] { "?" }, arguments);
                parser.GetAppNameAndVersion();
                Console.WriteLine(e.FormattedMessage);
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine(parser.GetUsage());
                exit = true;
            }
            catch (TargetInvocationException e)
            {
                parser = ArgumentParser.Parse(new[] { "?" }, arguments);
                parser.GetAppNameAndVersion();
                Console.WriteLine(e.InnerException.Message);
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine(parser.GetUsage());
                exit = true;
            }
            catch (Exception e)
            {
                parser = ArgumentParser.Parse(new[] { "?" }, arguments);
                parser.GetAppNameAndVersion();
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine(parser.GetUsage());
                exit = true;
            }

            if (!exit)
            {
                Run(arguments);
                if (File.Exists(arguments.TargetFile))
                    Process.Start(arguments.TargetFile);
                else
                    Console.Write("Target file does not exist.");
            }

            if (Debugger.IsAttached)
            {
                Console.Write("Press <enter> to exit...");
                Console.ReadLine();
            }
        }

        private static void Run(Arguments arguments)
        {
            var binPath = arguments.SourceDirectory;
            var filter = new Filter
            {
                NamespaceFilter = arguments.NamespaceFilter,
                WithInternals = arguments.AllInternals,
                WithInternalMembers = arguments.InternalMembers
            };
            var types = new Api(binPath, filter).GetTypes();

            //var relevantTypes = types.Where(t => t.Namespace.Contains(".Search") && !t.Namespace.Contains("Tests")).ToArray();
            //var relevantTypes = types.Where(t => t.IsContentHandler).ToArray();
            //var relevantTypes = types.Where(t => t.Namespace.StartsWith("SenseNet.ContentRepository.Storage.Data") || t.Name.Contains("DataProvider")).ToArray();
            var relevantTypes = types; //.Where(t => t.Name == "DataProvider" || t.BaseType == "DataProvider").ToArray();

            using (var writer = new StreamWriter(arguments.TargetFile))
            {
                Print(writer, relevantTypes, false);

                writer.WriteLine();
                writer.WriteLine("=================================================================================================");
                writer.WriteLine();
                writer.WriteLine("MEMBERS");
                writer.WriteLine();

                Print(writer, relevantTypes, true);

                writer.WriteLine();
                writer.WriteLine("=================================================================================================");
                writer.WriteLine();
                writer.WriteLine("TYPE TREE");
                writer.WriteLine();

                PrintTree(writer, relevantTypes);
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
                writer.WriteLine($"{t.Visibility}\tEnum\t{t.Assembly}\t{t.Namespace}\t{t.Name}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

            foreach (var t in relevantTypes.Where(t => t.IsInterface))
            {
                var baseType = (!string.IsNullOrEmpty(t.BaseType) && t.BaseType != "Object") ? ": " + t.BaseType : "";
                writer.WriteLine($"{t.Visibility}\tInterface\t{t.Assembly}\t{t.Namespace}\t{t.Name}\t{baseType}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

            foreach (var t in relevantTypes.Where(t => t.IsAbstractClass))
            {
                writer.WriteLine($"{t.Visibility}\tAbstract class\t{t.Assembly}\t{t.Namespace}\t{t.Name}\t{(t.BaseType != "Object" ? ": " + t.BaseType : "")}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

            foreach (var t in relevantTypes.Where(t => t.IsStaticClass))
            {
                writer.WriteLine($"{t.Visibility}\tStatic class\t{t.Assembly}\t{t.Namespace}\t{t.Name}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

            foreach (var t in relevantTypes.Where(t => t.IsClass && !t.IsStaticClass && !t.IsAbstractClass))
            {
                writer.WriteLine($"{t.Visibility}\tClass\t{t.Assembly}\t{t.Namespace}\t{t.Name}\t{(t.BaseType != "Object" ? ": " + t.BaseType : "")}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

            foreach (var t in relevantTypes.Where(t => !t.IsEnum && !t.IsInterface && !t.IsAbstractClass && !t.IsStaticClass && !t.IsClass))
            {
                writer.WriteLine($"{t.Visibility}\tStruct\t{t.Assembly}\t{t.Namespace}\t{t.Name}\t{(t.BaseType != "Object" ? ": " + t.BaseType : "")}");
                if (withMembers)
                    WriteMembers(writer, t);
            }

        }
        private static void WriteMembers(TextWriter writer, ApiType t)
        {
            foreach (var item in t.Fields)
                writer.WriteLine("\t\t\t\t\t\t" + item);
            foreach (var item in t.Properties)
                writer.WriteLine("\t\t\t\t\t\t" + item);
            foreach (var item in t.Events)
                writer.WriteLine("\t\t\t\t\t\t" + item);
            foreach (var item in t.Constructors)
                writer.WriteLine("\t\t\t\t\t\t" + item);
            foreach (var item in t.Methods)
                writer.WriteLine("\t\t\t\t\t\t" + item);
            foreach (var item in t.NestedClasses)
                writer.WriteLine("\t\t\t\t\t\t" + item);
            foreach (var item in t.OtherMembers)
                writer.WriteLine("\t\t\t\t\t\t" + item);
        }

        private static void PrintTree(TextWriter writer, ApiType[] types)
        {
            var roots = DiscoverTree(types);
            var nameSpaceWidth = types.Max(t => t.Namespace?.Length ?? 0) + 2;
            foreach (var root in roots)
                PrintTreeNode(writer, root, nameSpaceWidth, "");
        }

        private static void PrintTreeNode(TextWriter writer, ApiType node, int nameSpaceWidth, string indent)
        {
            writer.Write((node.Namespace ?? " ").PadRight(nameSpaceWidth));
            writer.Write("| ");
            writer.Write(indent);
            writer.WriteLine(node.Name);
            var childIndent = indent + "  ";
            foreach (var child in node.Children)
                PrintTreeNode(writer, child, nameSpaceWidth, childIndent);
        }

        private static IEnumerable<ApiType> DiscoverTree(ApiType[] apiTypes)
        {
            foreach (var apiType in apiTypes)
            {
                var parentType = apiType.Type.BaseType;
                apiType.Parent = apiTypes.FirstOrDefault(t => t.Type == parentType);
                apiType.Parent?.Children.Add(apiType);
            }

            var roots = apiTypes.Where(t => t.Parent == null).ToArray();
            return roots;
        }
    }
}
