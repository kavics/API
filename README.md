# API
Very simple API explorer for the .NET libraries and executables. Reads all assemblies from a file system directory, discovers containing types by the reflection API and writes information to one (big) text file. The main goal is extracting API so gathering the visible classes and their members from the original environment of the assemblies. The environment often contains referenced outer or irrelevant components that can be omitted by a primitive namespace filter.

## USAGE

GetApi &lt;source> [target] [-InternalMembers:Boolean] [-AllInternals:Boolean] [-Namespace:String] [-ContentHandlers:Boolean] [-OData:Boolean] [?]

### Arguments

- **source**: (*required path*) Path of the directory containing assemblies.
- **target**: (*optional path*) Path of the output file. Default: &lt;source>\api.txt. The recommended file type is any textual file that is associated a windows application because the file will be opened if that is possible after the program execution.
- **-AllInternals**: (*optional switch*) Shows internal classes and members. Alias: i
- **-InternalMembers**: (*optional switch*) Shows internal members of public classes. Alias: im.
- **-Namespace**: (*optional regex*) Valid regex that filters the namespaces (e.g.: -ns ".*sensenet..*"). Alias: n, ns.
- **-ContentHandlers**: (*optional switch*) Shows only ContentHandler classes of the sensenet. Alias: ch
- **-OData**: (*optional switch*) Shows only OData functions and actions of the sensenet. Alias: o,od
- **help**: [?, -?, /?, -h, -H, /h /H -help --help] Display the help text.

The &lt;source> need to be the first argument and [target] is the second if it exists.

## OUTPUT
The output file contains tab separated lines organized into several sections intended to be Excel friendly.

### HEAD SECTION
Contains three group: assemblies, namespaces, and type headers. Types have these columns:
- visibility
- kind (enum, class, etc.)
- assembly
- namespace
- name
- base type name

### MEMBERS SECTION
In this section all types are listed with their members. Columns of member (after the class-head line):
- kind (property, constructor, etc.)
- visibility and other modifiers (static, virtual etc.)
- member ype
- name
- other member dependent info: parameters, getter-setter existence, etc.

### TYPE TREE SECTION
Visualizes the inheritance tree in two columns
- namespace (fixed width, aligned left)
- type name (indent by inheritance level)

### ODATA SECTION
*sensenet specific*

If the "-odata" argument is present, only the OData functions and actions are printed. The format is similar to that described on "MEMBERS SECTION". The OData members are always written but not categorized if the "-odata" argument is missing.

## KNOWN ISSUES
1. The type tree cannot draw the real parent-child relations if any filter hides elements of the inheritance chain.
2. Some method parameter is not printed well if it is out or ref param.