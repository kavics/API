using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
// ReSharper disable ArrangeThisQualifier

namespace Kavics.ApiExplorer
{
    /// <summary>
    /// Decorator of a type. Contains simplified names to display.
    /// Contains grouped wrapped members of the decorated type.
    /// </summary>
    public class ApiType
    {
        public Type Type { get; }
        public string Namespace => this.Type.Namespace;

        private string _asmName;
        public string Assembly => _asmName ?? (_asmName = this.Type.Assembly.GetName().Name);

        private string _name;
        public string Name => _name ?? (_name = Api.GetTypeName(this.Type));

        private string _baseType;
        public string BaseType => _baseType ?? (_baseType = this.Type.BaseType == null ? string.Empty : Api.GetTypeName(this.Type.BaseType));

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
        public ApiMember[] NestedClasses { get; }
        public ApiMember[] OtherMembers { get; }

        public bool IsContentHandler { get; }

        public ApiType Parent { get; set; }
        public List<ApiType> Children { get; } = new List<ApiType>();

        private static BindingFlags _bindingFlagsForAllMembers =
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public ApiType(Type type, bool withInternalMembers)
        {
            Type = type;

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
            var memberArray = members.ToArray();

            Fields = memberArray.Where(m => m.IsField).ToArray();
            Properties = memberArray.Where(m => m.IsProperty).ToArray();
            Events = memberArray.Where(m => m.IsEvent).ToArray();
            Constructors = memberArray.Where(m => m.IsConstructor).ToArray();
            Methods = memberArray.Where(m => m.IsMethod).ToArray();
            NestedClasses = memberArray.Where(m => m.IsNestedClass).ToArray();
            OtherMembers = memberArray.Where(m => m.IsOther).ToArray();

            var attributes = type.GetCustomAttributes(false);
            IsContentHandler = attributes.Any(a => a.GetType().Name.StartsWith("ContentHandler"));
        }

    }
}
