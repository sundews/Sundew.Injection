// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationDataProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CompilationDataStage;

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Primitives.Computation;
using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
using Sundew.Injection.Generator.TypeSystem;
using MethodKind = Sundew.Injection.Generator.TypeSystem.MethodKind;
using Type = System.Type;

internal static class CompilationDataProvider
{
    private const string InitializationParameters = "initializationParameters";
    private const string DisposalParameters = "disposalParameters";
    private static readonly NamedType VoidType = new("void", string.Empty, string.Empty);
    private static readonly NamedType ValueTaskType = new("ValueTask", "System.Threading.Tasks", string.Empty);

    public static IncrementalValueProvider<R<CompilationData, ValueList<Diagnostic>>> SetupCompilationDataStage(this IncrementalValueProvider<Compilation> compilationProvider)
    {
        return compilationProvider.Select(GetCompilationData);
    }

    internal static R<CompilationData, ValueList<Diagnostic>> GetCompilationData(Compilation compilation, CancellationToken cancellationToken)
    {
        var areNullableAnnotationsSupported = compilation.Options.NullableContextOptions != NullableContextOptions.Disable;

        var requiredTypeSymbols = new[]
        {
            compilation.GetIInitializableTypeSymbol(),
            compilation.GetIAsyncInitializableTypeSymbol(),
            compilation.GetIDisposableTypeSymbol(),
            compilation.GetIAsyncDisposableTypeSymbol(),
            compilation.GetFunc(),
            compilation.GetTask(),
            compilation.GetILifecycleHandler(),
            compilation.GetLifecycleHandler(),
        }.AllOrFailed();

        cancellationToken.ThrowIfCancellationRequested();
        if (requiredTypeSymbols.TryGet(out var all, out var errors))
        {
            var index = 0;
            var iInitializableType = CreateNamedType(all[index++]);
            var iAsyncInitializableType = CreateNamedType(all[index++]);

            var iDisposableType = CreateNamedType(all[index++]);
            var iAsyncDisposableType = CreateNamedType(all[index++]);

            var func = all[index++];
            var task = all[index++];

            var ilifecycleHandlerTypeSymbol = all[index++];
            var lifecycleHandlerTypeSymbol = all[index++];
            var ilifecycleHandlerType = TypeConverter.GetNamedType(ilifecycleHandlerTypeSymbol);
            var lifecycleHandlerType = TypeConverter.GetNamedType(lifecycleHandlerTypeSymbol);
            var lifecycleHandlerConstructor = lifecycleHandlerTypeSymbol.Constructors.FirstOrDefault(x => x.Parameters.Length == 2 && x.DeclaredAccessibility == Accessibility.Public && !x.IsStatic);
            var defaultMetadata = new TypeMetadata(null, false, false, false);
            var lifecycleHandlerBinding = new Binding(
                lifecycleHandlerType,
                lifecycleHandlerType,
                Scope.SingleInstancePerFactory,
                new DefiniteMethod(
                    lifecycleHandlerType,
                    lifecycleHandlerType.Name,
                    lifecycleHandlerConstructor!.Parameters.Select(x => new DefiniteParameter(TypeConverter.GetNamedType((INamedTypeSymbol)x.Type), x.Name, defaultMetadata, ParameterNecessity._Optional(null))).ToImmutableArray(),
                    ImmutableArray<DefiniteTypeArgument>.Empty,
                    MethodKind._Constructor),
                false,
                false,
                false);
            return R.Success(new CompilationData(
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
                compilation.AssemblyName ?? string.Empty));
        }

        return R.Error((ValueList<Diagnostic>)errors.Select(x => Diagnostic.Create(Diagnostics.RequiredTypeNotFoundError, null, x.Error)).ToImmutableList());
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