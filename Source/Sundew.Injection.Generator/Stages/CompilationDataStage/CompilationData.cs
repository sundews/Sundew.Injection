// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationData.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CompilationDataStage;

using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
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
    OpenGenericType TypeResolverBinarySearch,
    OpenGenericType TypeResolverLinearSearch,
    NamedType TypeType,
    NamedType ObjectType,
    DefiniteClosedGenericType SpanOfObjectType,
    OpenGenericType Resolver,
    UnboundGenericType IEnumerableOfTType,
    string AssemblyName);