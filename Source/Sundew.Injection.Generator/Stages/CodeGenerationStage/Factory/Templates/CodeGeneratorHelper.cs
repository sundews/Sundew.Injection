// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeGeneratorHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Templates;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.TypeSystem;

internal static class CodeGeneratorHelper
{
    public static StringBuilder AppendAccessibility(this StringBuilder stringBuilder, DeclaredAccessibility accessibility)
    {
        return stringBuilder.Append(accessibility switch
        {
            DeclaredAccessibility.Internal => Trivia.Internal,
            DeclaredAccessibility.Public => Trivia.Public,
            DeclaredAccessibility.Protected => Trivia.Protected,
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
                ", ");
        }

        if (definiteType.Namespace != string.Empty)
        {
            stringBuilder.Append("global::");
            stringBuilder.Append(definiteType.Namespace);
            stringBuilder.Append('.');
        }

        switch (definiteType)
        {
            case DefiniteArrayType arrayType:
                stringBuilder.Append(arrayType.Name);
                stringBuilder.Append("[]");
                break;
            case DefiniteBoundGenericType definiteBoundGenericType:
                stringBuilder.Append(definiteBoundGenericType.Name);
                stringBuilder.Append('<');
                AppendTypeArguments(stringBuilder, definiteBoundGenericType.TypeArguments);
                stringBuilder.Append('>');
                break;
            case NamedType namedType:
                stringBuilder.Append(namedType.Name);
                break;
        }

        return stringBuilder;
    }

    public static void AppendInterfaces(this StringBuilder stringBuilder, IReadOnlyList<DefiniteType> interfaces)
    {
        if (interfaces.Count > 0)
        {
            stringBuilder.Append(" : ");
            stringBuilder.AppendItems(interfaces, (builder, type) => builder.AppendFullyQualifiedType(type), ", ");
        }
    }

    public static StringBuilder AppendMethodDeclaration(this StringBuilder stringBuilder, MethodDeclaration methodDeclaration, Options options, int indentation)
    {
        if (methodDeclaration.ReturnType != null)
        {
            AppendFullyQualifiedType(stringBuilder, methodDeclaration.ReturnType);
            stringBuilder.Append(' ');
        }

        return stringBuilder.Append(methodDeclaration.Name).Append('(').AppendParameters(methodDeclaration.Parameters, options.AreNullableAnnotationsSupported, indentation + 4).Append(')');
    }

    private static StringBuilder AppendParameters(this StringBuilder stringBuilder, ImmutableList<ParameterDeclaration> parameters, bool areNullableAnnotationsSupported, int indentation)
    {
        var requiredAndOptionalParameters = parameters.GroupBy(x => x.DefaultValue != null);
        var orderedParameters = requiredAndOptionalParameters.OrderBy(x => x.Key).SelectMany(x => x);
        var useNewLine = parameters.Count > 3;
        const string separator = ", ";
        var actualSeparator = separator;
        if (useNewLine)
        {
            actualSeparator += Environment.NewLine;
            stringBuilder.Append(Environment.NewLine);
        }
        else
        {
            indentation = 0;
        }

        return stringBuilder.AppendItems(
            orderedParameters,
            (builder, declaration) =>
            {
                builder.Append(' ', indentation);
                builder.AppendFullyQualifiedType(declaration.Type);
                if (areNullableAnnotationsSupported && declaration.DefaultValue != null)
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