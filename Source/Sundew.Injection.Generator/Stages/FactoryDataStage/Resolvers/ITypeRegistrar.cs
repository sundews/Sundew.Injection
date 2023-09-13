// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeRegistrar.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Resolvers;

using Sundew.Injection.Generator.TypeSystem;

internal interface ITypeRegistrar<in TValue>
{
    void Register(TypeId targetType, TypeId? interfaceType, TValue value);
}