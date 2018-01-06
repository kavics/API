using SenseNet.Tools.CommandLineArguments;
using SpaceBender.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBender.ApiExplorer.GetApi
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[]
            {
                ".", //"-im"
            };

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

            if (!exit)
                Run(arguments);

            if (Debugger.IsAttached)
            {
                Console.Write("Press <enter> to exit...");
                Console.ReadLine();
            }
        }

        private static void Run(Arguments arguments)
        {
            var binPath = arguments.SourceDirectory;
            var types = new Api(binPath, arguments.AllInternals, arguments.AllInternals || arguments.InternalMembers).GetTypes();

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
            foreach (var item in t.NestecClasses)
                writer.WriteLine("\t\t\t\t\t\t" + item);
            foreach (var item in t.OtherMembers)
                writer.WriteLine("\t\t\t\t\t\t" + item);
        }
    }

    public struct asdf
    {
        internal int qwer;
        internal int YXCV { get; }
    }
}
