// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolvedBinding.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using System.Collections.Generic;
using Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.TypeSystem;
using Sundew.Injection.Generator.Stages.InjectionDefinitionStage;
using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record ResolvedBinding;

internal sealed record ExternalParameter(DefiniteType Type, TypeMetadata TypeMetadata, ParameterSource ParameterSource) : ResolvedBinding;

internal sealed record SingleParameter(Binding Binding) : ResolvedBinding;

internal sealed record DefaultParameter(object? Literal, DefiniteType Type, TypeMetadata TypeMetadata) : ResolvedBinding;

internal sealed record MultiItemParameter(DefiniteType Type, DefiniteType ElementType, IReadOnlyList<Binding> Bindings, bool IsArrayRequired) : ResolvedBinding;
