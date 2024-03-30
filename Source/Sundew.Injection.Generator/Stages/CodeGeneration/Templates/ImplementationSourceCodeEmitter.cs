// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImplementationSourceCodeEmitter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Templates;

using System;
using System.Collections.Generic;
using System.Text;
using Sundew.Base.Collections;
using Sundew.Base.Text;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;
using CreationExpression = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.CreationExpression;
using Expression = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Expression;
using Member = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Member;
using Statement = Sundew.Injection.Generator.Stages.CodeGeneration.Syntax.Statement;

internal static class ImplementationSourceCodeEmitter
{
    public static string Emit(Accessibility accessibility, ClassDeclaration classDeclaration, Options options)
    {
        var indentation = 4;
        var stringBuilder = new StringBuilder();
        stringBuilder
            .If(
                options.AreNullableAnnotationsSupported,
                x => x.Append(SourceCodeEmitterExtensions.NullableEnable).AppendLine())
            .Append(Trivia.Namespace)
            .Append(' ')
            .Append(classDeclaration.Type.Namespace)
            .AppendLine()
            .Append('{')
            .AppendLine()
            .AppendTypeAttributes(classDeclaration.AttributeDeclarations, indentation)
            .Append(' ', indentation)
            .Append('[')
            .Append(SourceCodeEmitterExtensions.ExcludeFromCodeCoverage)
            .Append(']')
            .AppendLine()
            .Append(' ', indentation)
            .Append(accessibility.ToString().ToLowerInvariant())
            .Append(' ')
            .If(
                classDeclaration.IsSealed,
                x => x.Append(Trivia.Sealed).Append(' '))
            .Append(Trivia.Partial)
            .Append(' ')
            .Append(Trivia.Class)
            .Append(' ')
            .Append(classDeclaration.Type.Name)
            .AppendInterfaces(classDeclaration.Interfaces)
            .AppendLine()
            .Append(' ', indentation).Append('{')
            .AppendLine()
            .AppendMembers(classDeclaration.Members, options, indentation)
            .AppendLine()
            .Append(' ', indentation).Append('}')
            .AppendLine()
            .Append('}')
            .AppendLine();

        return stringBuilder.ToString();
    }

    private static StringBuilder AppendMembers(this StringBuilder stringBuilder, IReadOnlyList<Member> members, Options options, int indentation)
    {
        static void AppendMember(StringBuilder builder, Member member, bool wasAggregated, Options options, int indentation)
        {
            switch (member)
            {
                case Member.Field field:
                    builder.AppendField(field, indentation);
                    break;
                case Member.MethodImplementation methodImplementation:
                    if (wasAggregated)
                    {
                        builder.AppendLine();
                    }

                    builder.AppendMethodImplementation(methodImplementation, options, indentation);
                    break;
                case Member.Raw raw:
                    builder.Append(raw.Value);
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

    private static void AppendField(this StringBuilder stringBuilder, Member.Field field, int indentation)
    {
        var formattingOptions = field.Declaration.FieldModifier == FieldModifier.Static ? new FormattingOptions(true) : new FormattingOptions(false);
        stringBuilder.Append(' ', indentation)
            .Append(Trivia.Private)
            .AppendFieldScope(field.Declaration.FieldModifier)
            .Append(' ')
            .AppendFullyQualifiedType(field.Declaration.Type)
            .Append(' ')
            .Append(field.Declaration.Name)
            .If(
                field.Declaration.CreationExpression,
                (x, value) => x.Append(' ').Append('=').Append(' ').AppendExpression(value, indentation, formattingOptions))
            .Append(';');
    }

    private static StringBuilder AppendFieldScope(this StringBuilder builder, FieldModifier fieldModifier)
    {
        return fieldModifier switch
        {
            FieldModifier.Const => builder.Append(' ').Append(Trivia.Const),
            FieldModifier.Static => builder.Append(' ').Append(Trivia.Static).Append(' ').Append(Trivia.Readonly),
            FieldModifier.Instance => builder.Append(' ').Append(Trivia.Readonly),
        };
    }

    private static void AppendMethodImplementation(this StringBuilder stringBuilder, Member.MethodImplementation methodImplementation, Options options, int indentation)
    {
        stringBuilder
            .Append(' ', indentation)
            .AppendLine(Trivia.MethodImpl)
            .AppendAttributes(methodImplementation.MethodDeclaration.Attributes, indentation)
            .Append(' ', indentation)
            .AppendAccessibility(methodImplementation.MethodDeclaration.Accessibility)
            .If(
                methodImplementation.MethodDeclaration.IsAsync,
                x => x.Append(' ').Append(Trivia.Async))
            .If(
                methodImplementation.MethodDeclaration.IsVirtual,
                x => x.Append(' ').Append(Trivia.Virtual))
            .Append(' ')
            .AppendMethodDeclaration(methodImplementation.MethodDeclaration, options, indentation)
            .AppendLine()
            .Append(' ', indentation)
            .Append('{')
            .AppendLine()
            .AppendStatements(methodImplementation.Statements, options, indentation)
            .Append(' ', indentation)
            .Append('}');
    }

    private static StringBuilder AppendStatements(this StringBuilder stringBuilder, IReadOnlyList<Statement> statements, Options options, int indentation)
    {
        indentation += 4;
        foreach (var statement in statements)
        {
            stringBuilder.AppendStatement(statement, options, indentation);
        }

        return stringBuilder;
    }

    private static void AppendStatement(this StringBuilder stringBuilder, Statement statement, Options options, int indentation)
    {
        var formattingOptions = new FormattingOptions(false);
        switch (statement)
        {
            case CreateOptionalParameterIfStatement createOptionalParameterIfStatement:
                stringBuilder.Append(' ', indentation).Append(Trivia.If).Append(' ').Append('(')
                    .AppendExpression(createOptionalParameterIfStatement.ConditionAccess, indentation, formattingOptions).Append(' ')
                    .Append(Trivia.Equals).Append(' ').Append(Trivia.Null).Append(')')
                    .AppendLine()
                    .Append(' ', indentation).Append('{')
                    .AppendLine()
                    .AppendStatements(createOptionalParameterIfStatement.TrueStatements, options, indentation)
                    .Append(' ', indentation).Append('}')
                    .AppendLine()
                    .If(
                        createOptionalParameterIfStatement.FalseStatements,
                        (x, falseStatements) => x.Append(' ', indentation).Append(Trivia.Else)
                            .AppendLine()
                            .Append(' ', indentation).Append('{')
                            .AppendLine()
                            .AppendStatements(falseStatements, options, indentation)
                            .Append(' ', indentation).Append('}')
                            .AppendLine());

                break;
            case ExpressionStatement expressionStatement:
                stringBuilder.Append(' ', indentation).AppendExpression(expressionStatement.Expression, indentation, formattingOptions).Append(';');
                break;
            case ReturnStatement returnStatement:
                stringBuilder.Append(' ', indentation).Append(Trivia.Return)
                    .Append(' ')
                    .AppendExpression(returnStatement.Expression, indentation, formattingOptions)
                    .Append(';');
                break;
            case LocalDeclarationStatement localDeclarationExpression:
                stringBuilder.Append(' ', indentation).Append(Trivia.Var)
                    .Append(' ')
                    .Append(localDeclarationExpression.Name)
                    .Append(' ')
                    .Append('=')
                    .Append(' ')
                    .AppendExpression(localDeclarationExpression.Initializer, indentation, formattingOptions)
                    .Append(';');
                break;
            case LocalFunctionStatement localFunctionStatement:
                stringBuilder.AppendLine()
                    .Append(' ', indentation)
                    .If(localFunctionStatement.IsStatic, x => x.Append(Trivia.Static)).Append(' ').AppendFullyQualifiedType(localFunctionStatement.ReturnType).Append(' ').Append(localFunctionStatement.Name)
                    .Append('(').AppendParameters(localFunctionStatement.Parameters, options.AreNullableAnnotationsSupported, 0).Append(')')
                    .AppendLine()
                    .Append(' ', indentation).Append('{')
                    .AppendLine()
                    .AppendStatements(localFunctionStatement.Statements, options, indentation)
                    .Append(' ', indentation).Append('}')
                    .AppendLine();
                break;
            case YieldReturnStatement yieldReturnStatement:
                stringBuilder.Append(' ', indentation).Append(Trivia.Yield).Append(' ').Append(Trivia.Return).Append(' ').AppendExpression(yieldReturnStatement.Expression, 0, formattingOptions).Append(';');
                break;
            case Raw raw:
                stringBuilder.Append(raw.Source);
                break;
        }

        stringBuilder.AppendLine();
    }

    private static StringBuilder AppendExpression(this StringBuilder stringBuilder, Expression expression, int indentation, FormattingOptions formattingOptions)
    {
        switch (expression)
        {
            case AssignmentExpression assignmentExpression:
                stringBuilder.AppendExpression(assignmentExpression.Lhs, indentation, formattingOptions)
                    .Append(' ').Append('=').Append(' ')
                    .AppendExpression(assignmentExpression.Rhs, indentation, formattingOptions);
                break;
            case AwaitExpression awaitExpression:
                stringBuilder.Append(Trivia.Await).Append(' ').AppendExpression(awaitExpression.Expression, indentation, formattingOptions);
                break;
            case CreationExpression creationExpression:
                stringBuilder.AppendCreationExpression(creationExpression, indentation, formattingOptions);
                break;
            case FuncInvocationExpression funcInvocationExpression:
                stringBuilder.AppendExpression(funcInvocationExpression.DelegateAccessor, indentation, formattingOptions)
                    .If(
                        funcInvocationExpression.IsNullable,
                        x => x.Append('?'))
                    .Append('.')
                    .Append(Trivia.InvokeCall);
                break;
            case Identifier identifier:
                stringBuilder.Append(identifier.Name);
                break;
            case IndexerAccess indexerAccess:
                stringBuilder.AppendExpression(indexerAccess.Source, indentation, formattingOptions).Append('[').Append(indexerAccess.Index).Append(']');
                break;
            case InvocationExpression invocationExpression:
                stringBuilder.AppendExpression(invocationExpression.Expression, indentation, formattingOptions).Append('(').AppendArguments(invocationExpression.Arguments, indentation + 4, formattingOptions).Append(')');
                break;
            case MemberAccessExpression memberAccessExpression:
                stringBuilder.AppendExpression(memberAccessExpression.Expression, indentation, formattingOptions).Append('.').Append(memberAccessExpression.Name);
                break;
            case NullCoalescingOperatorExpression nullCoalescingOperatorExpression:
                stringBuilder
                    .AppendExpression(nullCoalescingOperatorExpression.Lhs, indentation, formattingOptions)
                    .Append(' ').Append(Trivia.NullCoalescing).If(nullCoalescingOperatorExpression.IsAssignment, builder => builder.Append('=')).Append(' ')
                    .AppendExpression(nullCoalescingOperatorExpression.Rhs, indentation, formattingOptions);
                break;
            case Lambda lambda:
                stringBuilder.Append('(')
                    .AppendItems(lambda.Parameters, (builder, expression1) => builder.AppendExpression(expression1, indentation, formattingOptions), Trivia.ListSeparator)
                    .Append(')')
                    .Append(' ')
                    .Append(Trivia.LambdaArrow)
                    .Append(' ')
                    .AppendExpression(lambda.Expression, indentation, formattingOptions);
                break;
            case TypeOf typeOf:
                stringBuilder.Append(Trivia.TypeOf).Append('(').AppendFullyQualifiedType(typeOf.Type).Append(')');
                break;
            case Cast cast:
                stringBuilder.Append('(')
                    .AppendFullyQualifiedType(cast.TargetType.Type)
                    .If(cast.TargetType.CanHaveDefaultValue && !cast.TargetType.Type.IsValueType, builder => builder.Append('?'))
                    .Append(')')
                    .AppendExpression(cast.Source, 0, formattingOptions);
                break;
        }

        return stringBuilder;
    }

    private static StringBuilder AppendCreationExpression(this StringBuilder stringBuilder, CreationExpression expression, int indentation, FormattingOptions formattingOptions)
    {
        var newIndentation = indentation + 4;
        switch (expression)
        {
            case CreationExpression.Array arrayCreation:

                stringBuilder.Append(Trivia.New)
                    .Append(' ')
                    .AppendFullyQualifiedType(arrayCreation.ElementType)
                    .Append('[').Append(']')
                    .Append(' ')
                    .Append('{')
                    .Append(' ')
                    .AppendArguments(arrayCreation.Arguments, newIndentation, formattingOptions)
                    .Append(' ')
                    .Append('}');

                break;
            case CreationExpression.ConstructorCall constructorCall:
                stringBuilder.Append(Trivia.New)
                    .Append(' ')
                    .AppendFullyQualifiedType(constructorCall.Type)
                    .Append('(')
                    .AppendArguments(constructorCall.Arguments, newIndentation, formattingOptions)
                    .Append(')');

                break;
            case CreationExpression.InstanceMethodCall instanceMethodCall:
                stringBuilder.AppendExpression(instanceMethodCall.FactoryAccessExpression, indentation, formattingOptions)
                    .Append('.')
                    .Append(instanceMethodCall.Name)
                    .If(
                        !instanceMethodCall.TypeArguments.IsEmpty(),
                        x => x.Append('<').AppendItems(instanceMethodCall.TypeArguments, (builder, argument) => builder.AppendFullyQualifiedType(argument.Type), Trivia.ListSeparator).Append('>'))
                    .Append('(')
                    .AppendArguments(instanceMethodCall.Arguments, newIndentation, formattingOptions)
                    .Append(')');
                break;
            case CreationExpression.StaticMethodCall staticMethodCall:
                stringBuilder
                    .If(
                        staticMethodCall.Type,
                        (builder, type) => builder.AppendFullyQualifiedType(type).Append('.'))
                    .Append(staticMethodCall.Name)
                    .If(
                        !staticMethodCall.TypeArguments.IsEmpty(),
                        x => x.Append('<').AppendItems(staticMethodCall.TypeArguments, (builder, argument) => builder.AppendFullyQualifiedType(argument.Type), Trivia.ListSeparator).Append('>'))
                    .Append('(')
                    .AppendArguments(staticMethodCall.Arguments, newIndentation, formattingOptions)
                    .Append(')');

                break;
            case CreationExpression.DefaultValue defaultValue:
                stringBuilder.Append(Trivia.Default).Append('(').AppendFullyQualifiedType(defaultValue.Type).Append(')');
                break;
            case Literal literal:
                stringBuilder.Append(literal.Value);
                break;
        }

        return stringBuilder;
    }

    private static StringBuilder AppendArguments(this StringBuilder stringBuilder, IReadOnlyList<Expression> arguments, int indentation, FormattingOptions formattingOptions)
    {
        var useNewLine = arguments.Count > 3 || formattingOptions.ForceArgumentNewLine;
        var actualSeparator = Trivia.ListSeparator;
        var preArguments = string.Empty;
        var argumentIndentation = indentation;
        if (useNewLine)
        {
            actualSeparator = Trivia.NewLineListSeparator;
            preArguments = Environment.NewLine;
        }
        else
        {
            argumentIndentation = 0;
        }

        return stringBuilder.AppendItems(arguments, builder => builder.Append(preArguments), (builder, expression) => builder.Append(' ', argumentIndentation).AppendExpression(expression, indentation, formattingOptions), actualSeparator);
    }
}