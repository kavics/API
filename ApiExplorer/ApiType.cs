using System;
using System.Linq;
using System.Reflection;

namespace Kavics.ApiExplorer
{
    /// <summary>
    /// Decorator of a type. Contains simplified names to display.
    /// Contains grouped wrapped members of the decorated type.
    /// </summary>
    public class ApiType
    {
        private Type _type;

        public string Assembly => _type.Assembly.GetName().Name;
        public string Namespace => _type.Namespace;
        public string Name => _type.Name;
        public string BaseType => _type.BaseType?.Name ?? "";

        public bool IsEnum { get; }
        public bool IsInterface { get; }
        public bool IsStaticClass { get; }
        public bool IsAbstractClass { get; }
        public bool IsClass { get; }
        public string Visibility { get; set; }

        public ApiMember[] Fields { get; }
        public ApiMember[] Properties { get; }
        public ApiMember[] Methods { get; }
        public ApiMember[] Constructors { get; }
        public ApiMember[] Events { get; }
        public ApiMember[] NestecClasses { get; }
        public ApiMember[] OtherMembers { get; }

        public bool IsContentHandler { get; }

        private static BindingFlags _bindingFlagsForAllMembers =
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public ApiType(Type type, bool withInternalMembers)
        {
            _type = type;

            IsEnum = type.IsEnum;
            IsInterface = type.IsInterface;
            IsStaticClass = type.IsAbstract && !type.IsInterface && type.IsSealed;
            IsAbstractClass = type.IsAbstract && !type.IsInterface && !type.IsSealed;
            Visibility = type.IsPublic ? "public" : "not public";
            IsClass = type.IsClass;

            var members = type.GetMembers(_bindingFlagsForAllMembers)
                .Where(m => !m.Name.StartsWith("<")) // skip auto implementations e.g. "<>c__DisplayClass29_0"
                .Where(m => m.DeclaringType == type)
                .Select(ApiMember.Create);
            if (!withInternalMembers)
                members = members
                    .Where(m => !m.IsPrivate) // skip private members
                    .Where(m => !(m.IsAssembly && !m.IsFamilyOrAssembly)); // skip internal (and not protected) members
            members = members.ToArray();

            Fields = members.Where(m => m.IsField).ToArray();
            Properties = members.Where(m => m.IsProperty).ToArray();
            Events = members.Where(m => m.IsEvent).ToArray();
            Constructors = members.Where(m => m.IsConstructor).ToArray();
            Methods = members.Where(m => m.IsMethod).ToArray();
            NestecClasses = members.Where(m => m.IsNestedClass).ToArray();
            OtherMembers = members.Where(m => m.IsOther).ToArray();

            var attributes = type.GetCustomAttributes(false);
            IsContentHandler = attributes.Any(a => a.GetType().Name.StartsWith("ContentHandler"));
        }

    }
}
