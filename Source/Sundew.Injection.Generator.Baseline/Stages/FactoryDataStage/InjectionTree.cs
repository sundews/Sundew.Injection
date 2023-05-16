// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionTree.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

internal sealed record InjectionTree(InjectionNode Root, bool ImplementDisposable, ImmutableList<FactoryConstructorParameterInjectionNode> FactoryConstructorParameters);