// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryData.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using System.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal sealed record FactoryData(
    NamedType FactoryType,
    NamedType? FactoryInterfaceType,
    bool GenerateInterface,
    Accessibility Accessibility,
    ImmutableArray<FactoryMethodData> FactoryMethodInfos);