// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryInterfaceSourceCodeEmitter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Templates;

using System;
using System.Collections.Generic;
using System.Text;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

internal static class FactoryInterfaceSourceCodeEmitter
{
    public static string GetFileContent(Accessibility accessibility, InterfaceDeclaration interfaceDeclaration, Options options)
    {
        var indentation = 4;
        var stringBuilder = new StringBuilder()
            .Append(Trivia.Namespace)
            .Append(' ')
            .Append(interfaceDeclaration.Type.Namespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendTypeAttributes(interfaceDeclaration.AttributeDeclarations, indentation)
            .Append(' ', indentation)
            .Append(accessibility.ToString().ToLowerInvariant())
            .Append(' ')
            .Append(Trivia.Interface)
            .Append(' ')
            .Append(interfaceDeclaration.Type.Name)
            .AppendInterfaces(interfaceDeclaration.InterfaceTypes)
            .AppendLine()
            .Append(' ', indentation).Append('{')
            .AppendLine()
            .AppendMembers(interfaceDeclaration.Members, options, indentation + 4)
            .AppendLine()
            .Append(' ', indentation).Append('}')
            .AppendLine()
            .Append('}')
            .AppendLine();

        return stringBuilder.ToString();
    }

    private static StringBuilder AppendMembers(this StringBuilder stringBuilder, IReadOnlyList<MethodDeclaration> members, Options options, int indentation)
    {
        static StringBuilder AppendMethodDeclaration(StringBuilder stringBuilder, MethodDeclaration methodDeclaration, bool isSuccessive, Options options, int indentation)
        {
            if (isSuccessive)
            {
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendAttributes(methodDeclaration.Attributes, indentation);
            return stringBuilder.Append(' ', indentation).AppendMethodDeclaration(methodDeclaration, options, indentation).Append(';');
        }

        return stringBuilder.AppendItems(
            members,
            (builder, methodDeclaration) => AppendMethodDeclaration(builder, methodDeclaration, false, options, indentation),
            (builder, methodDeclaration) => AppendMethodDeclaration(builder, methodDeclaration, true, options, indentation),
            Environment.NewLine);
    }
}