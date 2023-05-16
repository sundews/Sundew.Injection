// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryInterfaceFileTemplate.cs" company="Sundews">
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

internal static class FactoryInterfaceFileTemplate
{
    public static string GetFileContent(Accessibility accessibility, InterfaceDeclaration interfaceDeclaration, Options options)
    {
        var indentation = 4;
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("namespace");
        stringBuilder.Append(' ');
        stringBuilder.Append(interfaceDeclaration.Type.Namespace);
        stringBuilder.AppendLine();
        stringBuilder.Append('{');
        stringBuilder.AppendLine();
        stringBuilder.Append(' ', indentation);
        stringBuilder.Append(accessibility.ToString().ToLowerInvariant());
        stringBuilder.Append(' ');
        stringBuilder.Append("interface");
        stringBuilder.Append(' ');
        stringBuilder.Append(interfaceDeclaration.Type.Name);
        stringBuilder.AppendInterfaces(interfaceDeclaration.InterfaceTypes);

        stringBuilder.AppendLine();
        stringBuilder.Append(' ', indentation).Append('{');
        stringBuilder.AppendLine();
        stringBuilder.AppendMembers(interfaceDeclaration.Members, options, indentation + 4);
        stringBuilder.AppendLine();
        stringBuilder.Append(' ', indentation).Append('}');
        stringBuilder.AppendLine();
        stringBuilder.Append('}');
        stringBuilder.AppendLine();

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

            return stringBuilder.Append(' ', indentation).AppendMethodDeclaration(methodDeclaration, options, indentation).Append(';');
        }

        return stringBuilder.AppendItems(
            members,
            (builder, methodDeclaration) => AppendMethodDeclaration(builder, methodDeclaration, false, options, indentation),
            (builder, methodDeclaration) => AppendMethodDeclaration(builder, methodDeclaration, true, options, indentation),
            Environment.NewLine);
    }
}