// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeRegistrar.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

public interface INameRegistrar<in TValue>
{
    void Register(string name, TValue value);
}