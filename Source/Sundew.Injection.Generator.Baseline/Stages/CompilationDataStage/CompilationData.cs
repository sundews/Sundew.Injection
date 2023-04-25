// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationData.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CompilationDataStage;

using Sundew.Injection.Generator.TypeSystem;

public sealed record CompilationData(bool AreNullableAnnotationsSupported,
    DefiniteBoundGenericType DisposableListType,
    NamedType IDisposableType,
    GenericType WeakKeyDisposingDictionary,
    NamedType VoidType,
    GenericType FuncType,
    string AssemblyName);