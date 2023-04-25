// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveParameters.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using System.Collections.Generic;

internal interface IHaveParameters : IInjectionNode
{
    IReadOnlyList<InjectionNode> Parameters { get; }
}