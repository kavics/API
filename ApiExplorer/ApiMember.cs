using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kavics.ApiExplorer
{
    public class ApiMember
    {
        public string Representation { get; }
        public string Name { get; }

        public bool IsField { get; private set; }
        public bool IsProperty { get; private set; }
        public bool IsMethod { get; private set; }
        public bool IsConstructor { get; private set; }
        public bool IsEvent { get; private set; }
        public bool IsNestedClass { get; private set; }
        public bool IsOther { get; private set; }

        public bool IsPrivate { get; private set; }
        public bool IsAssembly { get; private set; }
        public bool IsFamily { get; private set; }
        public bool IsFamilyAndAssembly { get; private set; }
        public bool IsFamilyOrAssembly { get; private set; }
        public bool IsPublic { get; private set; }
        public bool IsAbstract { get; private set; }
        public bool IsVirtual { get; private set; }
        public bool IsFinal { get; private set; }
        public bool IsStatic { get; private set; }



        private ApiMember(string name, string representation)
        {
            Name = name;
            Representation = representation;
        }

        public static ApiMember Create(MemberInfo member)
        {
            var fieldInfo = member as FieldInfo;
            if (fieldInfo != null)
            {
                return new ApiMember(fieldInfo.Name, $"{Api.GetTypeName(fieldInfo.FieldType)} {fieldInfo.Name}")
                {
                    IsField = true,

                    IsPrivate = fieldInfo.IsPrivate,
                    IsAssembly = fieldInfo.IsAssembly,
                    IsFamily = fieldInfo.IsFamily,
                    IsFamilyAndAssembly = fieldInfo.IsFamilyAndAssembly,
                    IsFamilyOrAssembly = fieldInfo.IsFamilyOrAssembly,
                    IsPublic = fieldInfo.IsPublic,
                    IsStatic = fieldInfo.IsStatic,
                };
            }

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                var method = propertyInfo.GetMethod ?? propertyInfo.SetMethod;
                return new ApiMember(propertyInfo.Name, $"{Api.GetTypeName(propertyInfo.PropertyType)} {propertyInfo.Name} {{ {(propertyInfo.CanRead ? "get;" : "")} {(propertyInfo.CanWrite ? "set;" : "")} }}")
                {
                    IsProperty = true,

                    IsPrivate = method.IsPrivate,
                    IsAssembly = method.IsAssembly,
                    IsFamily = method.IsFamily,
                    IsFamilyAndAssembly = method.IsFamilyAndAssembly,
                    IsFamilyOrAssembly = method.IsFamilyOrAssembly,
                    IsPublic = method.IsPublic,
                    IsAbstract = method.IsAbstract,
                    IsVirtual = method.IsAbstract ? false : method.IsVirtual,
                    IsFinal = method.IsFinal,
                    IsStatic = method.IsStatic,
                };
            }

            var methodInfo = member as MethodInfo;
            if (methodInfo != null)
            {
                var x = methodInfo.GetParameters()
                    .Select(p => $"{(p.IsOut ? "out " : "")}{Api.GetTypeName(p.ParameterType)} {p.Name}{(p.IsOptional ? " = ?" : "")}")
                    .ToArray();
                var parameters = string.Join(", ", x);

                return new ApiMember(methodInfo.Name, $"{Api.GetTypeName(methodInfo.ReturnType)} {methodInfo.Name} ({parameters})")
                {
                    IsMethod = !methodInfo.Name.StartsWith("get_") && !methodInfo.Name.StartsWith("set_"),

                    IsPrivate = methodInfo.IsPrivate,
                    IsFamily = methodInfo.IsFamily,
                    IsAssembly = methodInfo.IsAssembly,
                    IsFamilyAndAssembly = methodInfo.IsFamilyAndAssembly,
                    IsFamilyOrAssembly = methodInfo.IsFamilyOrAssembly,
                    IsPublic = methodInfo.IsPublic,
                    IsAbstract = methodInfo.IsAbstract,
                    IsVirtual = methodInfo.IsAbstract ? false : methodInfo.IsVirtual,
                    IsFinal = methodInfo.IsFinal,
                    IsStatic = methodInfo.IsStatic,
                };
            }

            var ctorInfo = member as ConstructorInfo;
            if (ctorInfo != null)
            {
                var x = ctorInfo.GetParameters()
                    .Select(p => $"{(p.IsOut ? "out " : "")}{Api.GetTypeName(p.ParameterType)} {p.Name}{(p.IsOptional ? " = ?" : "")}")
                    .ToArray();
                var parameters = string.Join(", ", x);

                return new ApiMember(ctorInfo.Name, $"{ctorInfo.Name} ({parameters})")
                {
                    IsConstructor = true,

                    IsPrivate = ctorInfo.IsPrivate,
                    IsAssembly = ctorInfo.IsAssembly,
                    IsFamily = ctorInfo.IsFamily,
                    IsFamilyAndAssembly = ctorInfo.IsFamilyAndAssembly,
                    IsFamilyOrAssembly = ctorInfo.IsFamilyOrAssembly,
                    IsPublic = ctorInfo.IsPublic,
                    IsAbstract = ctorInfo.IsAbstract,
                    IsVirtual = ctorInfo.IsAbstract ? false : ctorInfo.IsVirtual,
                    IsFinal = ctorInfo.IsFinal,
                    IsStatic = ctorInfo.IsStatic,
                };
            }

            var eventInfo = member as EventInfo;
            if (eventInfo != null)
            {
                var method = eventInfo.AddMethod;
                return new ApiMember(eventInfo.Name, $"{Api.GetTypeName(eventInfo.EventHandlerType)} {eventInfo.Name}")
                {
                    IsEvent = true,

                    IsPrivate = method.IsPrivate,
                    IsAssembly = method.IsAssembly,
                    IsFamily = method.IsFamily,
                    IsFamilyAndAssembly = method.IsFamilyAndAssembly,
                    IsFamilyOrAssembly = method.IsFamilyOrAssembly,
                    IsPublic = method.IsPublic,
                    IsAbstract = method.IsAbstract,
                    IsVirtual = method.IsAbstract ? false : method.IsVirtual,
                    IsFinal = method.IsFinal,
                    IsStatic = method.IsStatic,
                };
            }

            var nestedType = member as Type;
            if (nestedType != null)
            {
                var name = nestedType.Name;
                var apiType = nestedType.IsInterface ? "interface" : (nestedType.IsClass ? "class" : "struct");
                return new ApiMember(Api.GetTypeName(nestedType), $"{apiType} {Api.GetTypeName(nestedType)}")
                {
                    IsNestedClass = true,

                    IsPublic = nestedType.IsNestedPublic,
                    IsPrivate = nestedType.IsNestedPrivate,
                    IsAbstract = nestedType.IsAbstract,
                };
            }

            return new ApiMember(member.Name, "??\t" + member.Name)
            {
                IsOther = true
            };
        }

        private string GetMemberType()
        {
            if (IsField)
                return "Field";
            if (IsProperty)
                return "Property";
            if (IsMethod)
                return "Method";
            if (IsConstructor)
                return "Constructor";
            if (IsEvent)
                return "Event";
            if (IsNestedClass)
                return "Nested class";
            return "??";
        }
        private string GetVisibility()
        {
            var words = new List<string>();
            if (IsPrivate)
                words.Add("private");
            if (IsAssembly)
                words.Add("internal");
            if (IsFamily)
                words.Add("protected");
            if (IsFamilyAndAssembly)
                words.Add("protected private");
            if (IsFamilyOrAssembly)
                words.Add("protected internal");
            if (IsPublic)
                words.Add("public");

            if (IsAbstract)
                words.Add("abstract");
            if (IsVirtual)
                words.Add("virtual");
            if (IsFinal)
                words.Add("sealed");

            if (IsStatic)
                words.Add("static");

            return string.Join(" ", words);
        }
        public override string ToString()
        {
            var x = GetMemberType();
            return $"{GetMemberType()}\t{GetVisibility()} {Representation}";
        }
    }
}
