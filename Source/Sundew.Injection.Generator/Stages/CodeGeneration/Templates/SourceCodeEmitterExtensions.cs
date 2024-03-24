// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceCodeEmitterExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Templates;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using Sundew.Injection.Generator.TypeSystem;

internal static class SourceCodeEmitterExtensions
{
    public const string NullableEnable = "#nullable enable";
    public const string ExcludeFromCodeCoverage = "global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage";
    private const string GeneratedCodeAttribute = "global::System.CodeDom.Compiler.GeneratedCodeAttribute";
    private const string SundewInjectionGenerator = "Sundew.Injection.Generator";

    public static StringBuilder AppendTypeAttributes(this StringBuilder stringBuilder, IEnumerable<AttributeDeclaration> attributeDeclarations, int indentation)
    {
        stringBuilder
            .Append(' ', indentation)
            .Append('[')
            .Append(GeneratedCodeAttribute)
            .Append('(')
            .Append('\"').Append(SundewInjectionGenerator).Append('\"')
            .Append(Trivia.ListSeparator)
            .Append('\"').Append(Assembly.GetExecutingAssembly().GetName().Version).Append('\"')
            .Append(')')
            .Append(']')
            .AppendLine();
        stringBuilder.AppendItems(attributeDeclarations, (builder, declaration) => builder.Append(' ', indentation).AppendLine(declaration.Value));
        return stringBuilder;
    }

    public static StringBuilder AppendAccessibility(this StringBuilder stringBuilder, DeclaredAccessibility accessibility)
    {
        return stringBuilder.Append(accessibility switch
        {
            DeclaredAccessibility.Internal => Trivia.Internal,
            DeclaredAccessibility.Public => Trivia.Public,
            DeclaredAccessibility.Protected => Trivia.Protected,
            DeclaredAccessibility.Private => Trivia.Private,
            _ => throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null),
        });
    }

    public static StringBuilder AppendFullyQualifiedType(this StringBuilder stringBuilder, DefiniteType definiteType)
    {
        static StringBuilder AppendTypeArguments(StringBuilder stringBuilder, ValueArray<DefiniteTypeArgument> typeArguments)
        {
            return stringBuilder.AppendItems(
                typeArguments,
                (builder, argument) => builder.AppendFullyQualifiedType(argument.Type),
                Trivia.ListSeparator);
        }

        if (definiteType.Namespace != string.Empty)
        {
            stringBuilder.Append(Trivia.Global).Append(Trivia.DoubleColon).Append(definiteType.Namespace).Append('.');
        }

        switch (definiteType)
        {
            case DefiniteArrayType arrayType:
                stringBuilder.Append(arrayType.Name).Append('[').Append(']');
                break;
            case DefiniteClosedGenericType definiteBoundGenericType:
                stringBuilder.Append(definiteBoundGenericType.Name).Append('<');
                AppendTypeArguments(stringBuilder, definiteBoundGenericType.TypeArguments).Append('>');
                break;
            case DefiniteNestedType definiteNestedType:
                stringBuilder.Append(definiteNestedType.Name);
                break;
            case NamedType namedType:
                stringBuilder.Append(namedType.Name);
                break;
        }

        return stringBuilder;
    }

    public static StringBuilder AppendInterfaces(this StringBuilder stringBuilder, IReadOnlyList<DefiniteType> interfaces)
    {
        if (interfaces.Count > 0)
        {
            stringBuilder.Append(' ').Append(':').Append(' ');
            stringBuilder.AppendItems(interfaces, (builder, type) => builder.AppendFullyQualifiedType(type), Trivia.ListSeparator);
        }

        return stringBuilder;
    }

    public static StringBuilder AppendMethodDeclaration(this StringBuilder stringBuilder, MethodDeclaration methodDeclaration, Options options, int indentation)
    {
        stringBuilder.If(
            methodDeclaration.ReturnType,
            (builder, returnType) => builder
                .AppendFullyQualifiedType(returnType.Type)
                .If(returnType.CanHaveDefaultValue && !returnType.Type.IsValueType && options.AreNullableAnnotationsSupported, builder => builder.Append('?'))
                .Append(' '));

        return stringBuilder.Append(methodDeclaration.Name).Append('(').AppendParameters(methodDeclaration.Parameters, options.AreNullableAnnotationsSupported, indentation + 4).Append(')');
    }

    public static StringBuilder AppendAttributes(this StringBuilder stringBuilder, IEnumerable<AttributeDeclaration> attributes, int indentation)
    {
        return stringBuilder.AppendItems(attributes, (builder, declaration) => builder.Append(' ', indentation).AppendLine(declaration.Value));
    }

    public static StringBuilder AppendParameters(this StringBuilder stringBuilder, ImmutableList<ParameterDeclaration> parameters, bool areNullableAnnotationsSupported, int indentation)
    {
        var useNewLine = parameters.Count > 3;
        var actualSeparator = Trivia.ListSeparator;
        if (useNewLine)
        {
            actualSeparator = Trivia.NewLineListSeparator;
            stringBuilder.Append(Environment.NewLine);
        }
        else
        {
            indentation = 0;
        }

        return stringBuilder.AppendItems(
            parameters,
            (builder, declaration) =>
            {
                builder.Append(' ', indentation);
                builder.AppendFullyQualifiedType(declaration.Type);
                if (areNullableAnnotationsSupported && !declaration.Type.IsValueType && declaration.DefaultValue != null)
                {
                    stringBuilder.Append('?');
                }

                stringBuilder.Append(' ').Append(declaration.Name);
                if (declaration.DefaultValue != null)
                {
                    builder.Append(' ').Append('=').Append(' ').Append(declaration.DefaultValue);
                }
            },
            actualSeparator);
    }
}