// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolvedBinding.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using System.Collections.Generic;
using Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;
using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record ResolvedBinding;

internal sealed record Error(BindingError BindingError) : ResolvedBinding;

internal sealed record ExternalParameter(DefiniteType Type, TypeMetadata TypeMetadata) : ResolvedBinding;

internal sealed record SingleParameter(Binding Binding) : ResolvedBinding;

internal sealed record ArrayParameter(DefiniteArrayType ArrayType, IReadOnlyList<Binding> Bindings) : ResolvedBinding;
