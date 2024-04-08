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
    Binding LifecycleHandlerBinding,
    NamedType IGeneratedFactoryType,
    OpenGenericType ConstructedType,
    NamedType VoidType,
    NamedType ValueTaskType,
    OpenGenericType TaskType,
    OpenGenericType FuncType,
    Type ResolverItemsFactoryType,
    Type ResolverItemType,
    ArrayType ResolverItemArrayType,
    NamedType TypeType,
    NamedType ObjectType,
    NamedType IntType,
    NamedType ServiceProviderType,
    ClosedGenericType SpanOfObjectType,
    OpenGenericType ResolverType,
    OpenGenericType IEnumerableOfTType,
    OpenGenericType IReadOnlyListOfTType,
    string AssemblyName,
    string AssemblyNamespace);