// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionDefinitionBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.InjectionDefinitionStage;

using Sundew.Base;

internal interface IInjectionDefinitionBuilder
{
    R<InjectionDefinition, Diagnostics> Build();
}