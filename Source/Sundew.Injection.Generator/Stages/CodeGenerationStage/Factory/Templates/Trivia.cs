namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Templates;

using System;

public class Trivia
{
    public const string Namespace = "namespace";
    public const string Public = "public";
    public const string Internal = "internal";
    public const string Private = "private";
    public const string PrivateName = "Private";
    public const string Sealed = "sealed";
    public const string Interface = "interface";
    public const string Class = "class";
    public const string Global = "global";
    public const string Readonly = "readonly";
    public const string Protected = "protected";
    public const string Virtual = "virtual";
    public const string InvokeCall = "Invoke()";
    public const string If = "if";
    public const string Else = "else";
    public const string Null = "null";
    public const string NullCoalescing = "??";
    public const string Var = "var";
    public const string New = "new";
    public const string ListSeparator = ", ";
    public const string DoubleColon = "::";
    public const string Return = "return";
    public const string AsyncName = "Async";
    public const string Async = "async";
    public const string Await = "await";
    public new const string Equals = "==";

    public static readonly string NewLineListSeparator = $",{Environment.NewLine}";
}