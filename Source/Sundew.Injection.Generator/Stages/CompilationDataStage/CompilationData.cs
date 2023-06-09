﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationData.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CompilationDataStage;

using Sundew.Injection.Generator.TypeSystem;

public sealed record CompilationData(
    bool AreNullableAnnotationsSupported,
    NamedType IInitializableType,
    NamedType IAsyncInitializableType,
    NamedType IDisposableType,
    NamedType IAsyncDisposableType,
    DefiniteBoundGenericType DisposableListType,
    GenericType WeakKeyDisposingDictionary,
    NamedType ILifetimeHandlerType,
    NamedType LifetimeHandlerType,
    GenericType ConstructedType,
    NamedType VoidType,
    NamedType ValueTaskType,
    GenericType TaskType,
    GenericType FuncType,
    string AssemblyName);