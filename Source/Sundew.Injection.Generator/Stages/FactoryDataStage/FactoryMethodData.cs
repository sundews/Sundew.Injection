// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodData.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record FactoryMethodData(
    string FactoryMethodName,
    (DefiniteType Type, TypeMetadata TypeMetadata) Return,
    (DefiniteType Type, TypeMetadata TypeMetadata) Target,
    InjectionNode InjectionTree);
