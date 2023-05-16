// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryImplementationFileGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Templates;

using System;
using System.Collections.Generic;
using System.Text;
using Sundew.Base.Collections;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.Stages.FactoryDataStage;

internal static class FactoryImplementationFileGenerator
{
    public static string GetFileContent(Accessibility accessibility, ClassDeclaration classDeclaration, Options options)
    {
        var indentation = 4;
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Trivia.Namespace);
        stringBuilder.Append(' ');
        stringBuilder.Append(classDeclaration.Type.Namespace);
        stringBuilder.AppendLine();
        stringBuilder.Append('{');
        stringBuilder.AppendLine();
        stringBuilder.Append(' ', indentation);
        stringBuilder.Append(accessibility.ToString().ToLowerInvariant());
        stringBuilder.Append(' ');
        if (classDeclaration.IsSealed)
        {
            stringBuilder.Append(Trivia.Sealed);
            stringBuilder.Append(' ');
        }

        stringBuilder.Append(Trivia.Class);
        stringBuilder.Append(' ');
        stringBuilder.Append(classDeclaration.Type.Name);
        stringBuilder.AppendInterfaces(classDeclaration.Interfaces);
        stringBuilder.AppendLine();
        stringBuilder.Append(' ', indentation).Append('{');
        stringBuilder.AppendLine();
        stringBuilder.AppendMembers(classDeclaration.Members, options, indentation);
        stringBuilder.AppendLine();
        stringBuilder.Append(' ', indentation).Append('}');
        stringBuilder.AppendLine();
        stringBuilder.Append('}');
        stringBuilder.AppendLine();

        return stringBuilder.ToString();
    }

    private static StringBuilder AppendMembers(this StringBuilder stringBuilder, IReadOnlyList<Member> members, Options options, int indentation)
    {
        static void AppendMember(StringBuilder builder, Member member, bool wasAggregated, Options options, int indentation)
        {
            switch (member)
            {
                case Field field:
                    builder.AppendField(field, indentation);
                    break;
                case MethodImplementation methodImplementation:
                    if (wasAggregated)
                    {
                        builder.AppendLine();
                    }

                    builder.AppendMethodImplementation(methodImplementation, options, indentation);
                    break;
            }
        }

        indentation += 4;
        return stringBuilder.AppendItems(
            members,
            (builder, member) => AppendMember(builder, member, false, options, indentation),
            (builder, member) => AppendMember(builder, member, true, options, indentation),
            Environment.NewLine);
    }

    private static void AppendField(this StringBuilder stringBuilder, Field field, int indentation)
    {
        stringBuilder.Append(' ', indentation)
            .Append(Trivia.Private)
            .Append(' ')
            .Append(Trivia.Readonly)
            .Append(' ')
            .AppendFullyQualifiedType(field.Declaration.Type)
            .Append(' ')
            .Append(field.Declaration.Name);
        if (field.Declaration.CreationExpression != null)
        {
            stringBuilder.Append(' ').Append('=').Append(' ').AppendExpression(field.Declaration.CreationExpression, indentation);
        }

        stringBuilder.Append(';');
    }

    private static void AppendMethodImplementation(this StringBuilder stringBuilder, MethodImplementation methodImplementation, Options options, int indentation)
    {
        stringBuilder.Append(' ', indentation)
            .AppendAccessibility(methodImplementation.MethodDeclaration.Accessibility);
        if (methodImplementation.MethodDeclaration.IsVirtual)
        {
            stringBuilder.Append(' ').Append(Trivia.Virtual);
        }

        stringBuilder.Append(' ')
            .AppendMethodDeclaration(methodImplementation.MethodDeclaration, options, indentation)
            .AppendLine()
            .Append(' ', indentation)
            .Append('{')
            .AppendLine()
            .AppendStatements(methodImplementation.Statements, indentation)
            .Append(' ', indentation)
            .Append('}');
    }

    private static StringBuilder AppendStatements(this StringBuilder stringBuilder, IReadOnlyList<Statement> statements, int indentation)
    {
        indentation += 4;
        foreach (var statement in statements)
        {
            stringBuilder.AppendStatement(statement, indentation);
        }

        return stringBuilder;
    }

    private static void AppendStatement(this StringBuilder stringBuilder, Statement statement, int indentation)
    {
        stringBuilder.Append(' ', indentation);
        switch (statement)
        {
            case CreateOptionalParameterIfStatement createOptionalParameterIfStatement:
                stringBuilder.Append(Trivia.If).Append(' ').Append('(')
                    .AppendExpression(createOptionalParameterIfStatement.ConditionAccess, indentation).Append(' ').Append(Trivia.Equals).Append(' ').Append(Trivia.Null).Append(')')
                    .AppendLine()
                    .Append(' ', indentation).Append('{')
                    .AppendLine()
                    .AppendStatements(createOptionalParameterIfStatement.TrueStatements, indentation)
                    .Append(' ', indentation).Append('}')
                    .AppendLine();

                if (createOptionalParameterIfStatement.FalseStatements != null)
                {
                    stringBuilder.Append(' ', indentation).Append(Trivia.Else)
                        .AppendLine()
                        .Append(' ', indentation).Append('{')
                        .AppendLine()
                        .AppendStatements(createOptionalParameterIfStatement.FalseStatements, indentation)
                        .Append(' ', indentation).Append('}')
                        .AppendLine();
                }

                break;
            case ExpressionStatement expressionStatement:
                stringBuilder.AppendExpression(expressionStatement.Expression, indentation).Append(';');
                break;
            case ReturnStatement returnStatement:
                stringBuilder.Append(Trivia.Return)
                    .Append(' ')
                    .AppendExpression(returnStatement.Expression, indentation)
                    .Append(';');
                break;
            case LocalDeclarationStatement localDeclarationExpression:
                stringBuilder.Append(Trivia.Var)
                    .Append(' ')
                    .Append(localDeclarationExpression.Name)
                    .Append(' ')
                    .Append('=')
                    .Append(' ')
                    .AppendExpression(localDeclarationExpression.Initializer, indentation)
                    .Append(';');
                break;
        }

        stringBuilder.AppendLine();
    }

    private static StringBuilder AppendExpression(this StringBuilder stringBuilder, Expression expression, int indentation)
    {
        switch (expression)
        {
            case AssignmentExpression assignmentExpression:
                stringBuilder.AppendExpression(assignmentExpression.Lhs, indentation);
                stringBuilder.Append(' ').Append('=').Append(' ');
                stringBuilder.AppendExpression(assignmentExpression.Rhs, indentation);
                break;
            case CreationExpression createExpression:
                var newIndentation = indentation + 4;
                switch (createExpression.CreationSource)
                {
                    case ArrayCreation arrayCreation:
                        stringBuilder.Append(Trivia.New)
                            .Append(' ')
                            .AppendFullyQualifiedType(arrayCreation.ElementType)
                            .Append('[').Append(']')
                            .Append(' ')
                            .Append('{')
                            .Append(' ')
                            .AppendArguments(createExpression.Arguments, newIndentation)
                            .Append(' ')
                            .Append('}');

                        break;
                    case ConstructorCall constructorCall:
                        stringBuilder.Append(Trivia.New)
                            .Append(' ')
                            .AppendFullyQualifiedType(constructorCall.Type)
                            .Append('(')
                            .AppendArguments(createExpression.Arguments, newIndentation)
                            .Append(')');

                        break;
                    case StaticMethodCall staticMethodCall:
                        stringBuilder.AppendFullyQualifiedType(staticMethodCall.Method.ContainingType)
                            .Append('.')
                            .Append(staticMethodCall.Method.Name);
                        if (!staticMethodCall.Method.TypeArguments.IsEmpty())
                        {
                            stringBuilder.Append('<').AppendItems(staticMethodCall.Method.TypeArguments, (builder, argument) => builder.AppendFullyQualifiedType(argument.Type), Trivia.ListSeparator).Append('>');
                        }

                        stringBuilder.Append('(')
                            .AppendArguments(createExpression.Arguments, newIndentation)
                            .Append(')');

                        break;
                }

                break;
            case FuncInvocationExpression funcInvocationExpression:
                stringBuilder.AppendExpression(funcInvocationExpression.DelegateAccessor, indentation);
                if (funcInvocationExpression.IsNullable)
                {
                    stringBuilder.Append('?');
                }

                stringBuilder.Append('.').Append(Trivia.InvokeCall);
                break;
            case Identifier identifier:
                stringBuilder.Append(identifier.Name);
                break;
            case InvocationExpression invocationExpression:
                stringBuilder.AppendExpression(invocationExpression.Expression, indentation).Append('(').AppendArguments(invocationExpression.Arguments, indentation + 4).Append(')');
                break;
            case MemberAccessExpression memberAccessExpression:
                stringBuilder.AppendExpression(memberAccessExpression.Expression, indentation).Append('.').Append(memberAccessExpression.Name);
                break;
            case NullCoalescingOperatorExpression nullCoalescingOperatorExpression:
                stringBuilder.AppendExpression(nullCoalescingOperatorExpression.Lhs, indentation).Append(' ').Append(Trivia.NullCoalescing).Append(' ').AppendExpression(nullCoalescingOperatorExpression.Rhs, indentation);
                break;
        }

        return stringBuilder;
    }

    private static StringBuilder AppendArguments(this StringBuilder stringBuilder, IReadOnlyList<Expression> arguments, int indentation)
    {
        var useNewLine = arguments.Count > 3;
        var actualSeparator = Trivia.ListSeparator;
        var argumentIndentation = indentation;
        if (useNewLine)
        {
            actualSeparator = Trivia.NewLineListSeparator;
            stringBuilder.Append(Environment.NewLine);
        }
        else
        {
            argumentIndentation = 0;
        }

        return stringBuilder.AppendItems(arguments, (builder, expression) => builder.Append(' ', argumentIndentation).AppendExpression(expression, indentation), actualSeparator);
    }
}