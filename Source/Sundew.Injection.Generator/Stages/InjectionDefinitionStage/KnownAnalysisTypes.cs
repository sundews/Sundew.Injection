// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownAnalysisTypes.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using System.Collections;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Linq;
using Sundew.Injection.Generator.TypeSystem;

public sealed class KnownAnalysisTypes : IKnownInjectableTypes
{
    private KnownAnalysisTypes(
        INamedTypeSymbol funcTypeSymbol,
        INamedTypeSymbol enumerableTypeSymbol,
        INamedTypeSymbol disposableTypeSymbol,
        INamedTypeSymbol asyncDisposableTypeSymbol,
        INamedTypeSymbol initializableTypeSymbol,
        INamedTypeSymbol asyncInitializableTypeSymbol,
        INamedTypeSymbol injectionDeclarationType,
        INamedTypeSymbol injectionBuilderType,
        INamedTypeSymbol factoryMethodSelectorTypeSymbol,
        INamedTypeSymbol createMethodSelectorTypeSymbol,
        INamedTypeSymbol constructedTypeSymbol,
        INamedTypeSymbol readonlyListTypeSymbol,
        INamedTypeSymbol enumerableOfTTypeSymbol)
    {
        this.FuncTypeSymbol = funcTypeSymbol;
        this.IEnumerableTypeSymbol = enumerableTypeSymbol;
        this.IDisposableTypeSymbol = disposableTypeSymbol;
        this.IAsyncDisposableTypeSymbol = asyncDisposableTypeSymbol;
        this.IInitializableTypeSymbol = initializableTypeSymbol;
        this.IAsyncInitializableTypeSymbol = asyncInitializableTypeSymbol;
        this.InjectionDeclarationType = injectionDeclarationType;
        this.InjectionBuilderType = injectionBuilderType;
        this.FactoryMethodSelectorTypeSymbol = factoryMethodSelectorTypeSymbol;
        this.CreateMethodSelectorTypeSymbol = createMethodSelectorTypeSymbol;
        this.ConstructedTypeSymbol = constructedTypeSymbol;
        this.IReadOnlyListOfTTypeSymbol = readonlyListTypeSymbol;
        this.IEnumerableOfTTypeSymbol = enumerableOfTTypeSymbol;
    }

    public INamedTypeSymbol FuncTypeSymbol { get; }

    public INamedTypeSymbol IEnumerableTypeSymbol { get; }

    public INamedTypeSymbol IDisposableTypeSymbol { get; }

    public INamedTypeSymbol IAsyncDisposableTypeSymbol { get; }

    public INamedTypeSymbol IInitializableTypeSymbol { get; }

    public INamedTypeSymbol IAsyncInitializableTypeSymbol { get; }

    public INamedTypeSymbol IReadOnlyListOfTTypeSymbol { get; }

    public INamedTypeSymbol IEnumerableOfTTypeSymbol { get; }

    public INamedTypeSymbol InjectionDeclarationType { get; }

    public INamedTypeSymbol InjectionBuilderType { get; }

    public INamedTypeSymbol FactoryMethodSelectorTypeSymbol { get; }

    public INamedTypeSymbol CreateMethodSelectorTypeSymbol { get; }

    public INamedTypeSymbol ConstructedTypeSymbol { get; }

    public static R<KnownAnalysisTypes, Diagnostics> Get(Compilation compilation)
    {
        var requiredTypes = new[]
        {
            compilation.GetFunc(),
            R.From(compilation.GetTypeByMetadataName(typeof(IEnumerable).FullName), () => "IEnumerable was not found"),
            compilation.GetIDisposableTypeSymbol(),
            compilation.GetIAsyncDisposableTypeSymbol(),
            compilation.GetIInitializableTypeSymbol(),
            compilation.GetIAsyncInitializableTypeSymbol(),
            R.From(compilation.GetTypeByMetadataName(typeof(IInjectionDeclaration).FullName), () => "IInjectionDeclaration was not found, Sundew.Injection must be referenced"), R.From(compilation.GetTypeByMetadataName(typeof(IInjectionDeclaration).FullName), () => "IInjectionDeclaration was not found, Sundew.Injection must be referenced"),
            R.From(compilation.GetTypeByMetadataName(typeof(IFactoryMethodSelector).FullName), () => "IFactoryMethodSelector was not found, Sundew.Injection must be referenced"),
            R.From(compilation.GetTypeByMetadataName(typeof(ICreateMethodSelector<>).FullName), () => "ICreateMethodSelector was not found, Sundew.Injection must be referenced"),
            compilation.GetConstructed(),
            compilation.GetIReadOnlyListOfT(),
            compilation.GetIEnumerableOfT(),
        }.AllOrFailed();

        if (requiredTypes.TryGet(out var all, out var errors))
        {
            var index = 0;
            return R.Success(new KnownAnalysisTypes(
                all[index++],
                all[index++],
                all[index++],
                all[index++],
                all[index++],
                all[index++],
                all[index++],
                all[index++],
                all[index++],
                all[index++],
                all[index++],
                all[index++],
                all[index++]));
        }

        return R.Error(new Diagnostics(errors
            .Select(x => Diagnostic.Create(Diagnostics.RequiredTypeNotFoundError, null, x.Error))));
    }
}