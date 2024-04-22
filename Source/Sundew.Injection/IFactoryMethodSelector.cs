// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFactoryMethodSelector.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection
{
    using System;
    using System.Linq.Expressions;

    public interface IFactoryMethodSelector
    {
        IFactoryMethods Add<TInterface, TImplementation>(Expression<Func<TImplementation>>? constructorSelector = null, string? factoryMethodName = null, Accessibility accessibility = Accessibility.Public, bool isNewOverridable = false)
            where TImplementation : TInterface;

        IFactoryMethods Add<TImplementation>(Expression<Func<TImplementation>>? constructorSelector = null, string? factoryMethodName = null, Accessibility accessibility = Accessibility.Public, bool isNewOverridable = false);
    }
}