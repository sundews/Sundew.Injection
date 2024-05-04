// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationData.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CompilationDataStage;

using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record CompilationData(
    bool AreNullableAnnotationsSupported,
    NamedType IInitializableType,
    NamedType IAsyncInitializableType,
    NamedType IDisposableType,
    NamedType IAsyncDisposableType,
    NamedType VoidType,
    NamedType ValueTaskType,
    OpenGenericType TaskType,
    OpenGenericType FuncType,
    NamedType TypeType,
    NamedType ObjectType,
    NamedType IntType,
    NamedType ServiceProviderType,
    ClosedGenericType SpanOfObjectType,
    OpenGenericType IEnumerableOfTType,
    OpenGenericType IReadOnlyListOfTType,
    SundewInjectionReferencedCompilationData ReferencedSundewInjectionCompilationData,
    SundewInjectionProvidedCompilationData ProvidedSundewInjectionCompilationData,
    string AssemblyName,
    string AssemblyNamespace);

internal sealed record SundewInjectionProvidedCompilationData(
    Binding LifecycleHandlerBinding,
    Type ResolverItemsFactoryType,
    Type ResolverItemType,
    ArrayType ResolverItemArrayType,
    OpenGenericType ResolverItemOfTType);

internal sealed record SundewInjectionReferencedCompilationData(
    Type ILifecycleHandlerType,
    OpenGenericType ConstructedType,
    NamedType IGeneratedFactoryType);