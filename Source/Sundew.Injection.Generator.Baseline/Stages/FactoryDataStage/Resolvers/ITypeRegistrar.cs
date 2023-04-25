// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeRegistrar.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using Sundew.Injection.Generator.TypeSystem;

public interface ITypeRegistrar<in TValue>
{
    void Register(Type targetType, Type? interfaceType, TValue value);
}