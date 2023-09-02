// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionTree.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

internal sealed record InjectionTree(InjectionNode Root, bool NeedsLifecycleHandling, ValueList<FactoryConstructorParameter> FactoryConstructorParameters);