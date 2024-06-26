﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Trivia.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Templates;

using System;

public class Trivia
{
    public const string Namespace = "namespace";
    public const string Public = "public";
    public const string Internal = "internal";
    public const string Private = "private";
    public const string PrivateName = "Private";
    public const string Sealed = "sealed";
    public const string Partial = "partial";
    public const string Interface = "interface";
    public const string Class = "class";
    public const string Global = "global";
    public const string Const = "const";
    public const string Static = "static";
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
    public const string Default = "default";
    public const string TypeOf = "typeof";
    public const string LambdaArrow = "=>";
    public const string Yield = "yield";
    public const string This = "this";
    public const string MethodImpl = "[global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]";
    public const string PropertyGetter = "{ get; }";
    public const string Get = "get";
    public new const string Equals = "==";

    public static readonly string NewLineListSeparator = $",{Environment.NewLine}";
}