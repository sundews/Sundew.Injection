﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceSourceCodeEmitter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Templates;

using System;
using System.Collections.Generic;
using System.Text;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

internal static class InterfaceSourceCodeEmitter
{
    public static string Emit(Accessibility accessibility, InterfaceDeclaration interfaceDeclaration, Options options)
    {
        var indentation = 4;
        var stringBuilder = new StringBuilder();
        if (options.AreNullableAnnotationsSupported)
        {
            stringBuilder.Append(SourceCodeEmitterExtensions.NullableEnable)
                .AppendLine();
        }

        stringBuilder
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
            .Append(Trivia.Partial)
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

    private static StringBuilder AppendMembers(this StringBuilder stringBuilder, IReadOnlyList<MemberDeclaration> members, Options options, int indentation)
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

        static StringBuilder AppendPropertyDeclaration(StringBuilder stringBuilder, PropertyDeclaration propertyDeclaration, bool isSuccessive, Options options, int indentation)
        {
            if (isSuccessive)
            {
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendAttributes(propertyDeclaration.Attributes, indentation);
            return stringBuilder.Append(' ', indentation).AppendPropertyDeclaration(propertyDeclaration, options, indentation);
        }

        StringBuilder AppendMember(MemberDeclaration memberDeclaration, StringBuilder builder, bool isSuccessive)
        {
            return memberDeclaration switch
            {
                MethodDeclaration methodDeclaration => AppendMethodDeclaration(builder, methodDeclaration, isSuccessive, options, indentation),
                PropertyDeclaration propertyDeclaration => AppendPropertyDeclaration(builder, propertyDeclaration, isSuccessive, options, indentation),
            };
        }

        return stringBuilder.AppendItems(
            members,
            (builder, memberDeclaration) => AppendMember(memberDeclaration, builder, false),
            (builder, memberDeclaration) => AppendMember(memberDeclaration, builder, true),
            Environment.NewLine);
    }
}