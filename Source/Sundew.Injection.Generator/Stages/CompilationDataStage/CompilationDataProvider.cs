// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationDataProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CompilationDataStage;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;
using Type = System.Type;

internal static class CompilationDataProvider
{
    private const string InitializationParameters = "initializationParameters";
    private const string DisposalParameters = "disposalParameters";
    private static readonly NamedType VoidType = new NamedType("void", string.Empty, string.Empty);
    private static readonly NamedType ValueTaskType = new NamedType("ValueTask", "System.Threading.Tasks", string.Empty);

    public static IncrementalValueProvider<CompilationData> SetupCompilationInfoStage(this IncrementalValueProvider<Compilation> compilationProvider)
    {
        return compilationProvider.Select((compilation, _) => GetCompilationData(compilation));
    }

    internal static CompilationData GetCompilationData(Compilation compilation)
    {
        var areNullableAnnotationsSupported = compilation.Options.NullableContextOptions != NullableContextOptions.Disable;

        var iInitializableType = CreateNamedType(compilation.GetIInitializableTypeSymbol());
        var iAsyncInitializableType = CreateNamedType(compilation.GetIAsyncInitializableTypeSymbol());

        var iDisposableType = CreateNamedType(compilation.GetIDisposableTypeSymbol());
        var iAsyncDisposableType = CreateNamedType(compilation.GetIAsyncDisposableTypeSymbol());

        var func = compilation.GetFunc();
        var task = compilation.GetTask();

        var lifecycleHandlerTypeSymbol = compilation.GetLifecycleHandler();
        var lifecycleHandlerType = TypeConverter.GetNamedType(lifecycleHandlerTypeSymbol);
        var constructor = lifecycleHandlerTypeSymbol.Constructors.FirstOrDefault(x => x.Parameters.Length == 2 && x.DeclaredAccessibility == Accessibility.Public && !x.IsStatic);
        var defaultMetadata = new TypeMetadata(null, false, false, false);
        var lifecycleHandlerBinding = new Binding(
            lifecycleHandlerType,
            lifecycleHandlerType,
            Scope.SingleInstancePerFactory,
            new DefiniteMethod(
                lifecycleHandlerType,
                lifecycleHandlerType.Name,
                constructor!.Parameters.Select(x => new DefiniteParameter(TypeConverter.GetNamedType((INamedTypeSymbol)x.Type), x.Name, defaultMetadata, ParameterNecessity._Optional(null))).ToImmutableArray(),
                ImmutableArray<DefiniteTypeArgument>.Empty,
                MethodKind._Constructor),
            false,
            false,
            false);
        return new CompilationData(
            areNullableAnnotationsSupported,
            iInitializableType,
            iAsyncInitializableType,
            iDisposableType,
            iAsyncDisposableType,
            lifecycleHandlerBinding,
            CreateNamedType(typeof(IGeneratedFactory)),
            GenericTypeConverter.GetGenericType(typeof(Sundew.Injection.Constructed<>), string.Empty),
            VoidType,
            ValueTaskType,
            GenericTypeConverter.GetGenericType(task),
            GenericTypeConverter.GetGenericType(func),
            compilation.AssemblyName ?? string.Empty);
    }

    private static NamedType CreateNamedType(Type type)
    {
        return new NamedType(type.Name, type.Namespace, type.Assembly.GetName().Name);
    }

    private static NamedType CreateNamedType(INamedTypeSymbol namedTypeSymbol)
    {
        return new NamedType(namedTypeSymbol.MetadataName, TypeHelper.GetNamespace(namedTypeSymbol.ContainingNamespace), namedTypeSymbol.ContainingAssembly.Identity.ToString());
    }
}