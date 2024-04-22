// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFactoryMethodBindingSelector.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection
{
    using System;
    using System.Linq.Expressions;

    public interface IFactoryMethodBindingSelector<TFactory>
    {
        IFactoryMethodBindings<TFactory> Add<TInterface, TImplementation>(Expression<Func<TFactory, TImplementation>> factoryMethodSelector)
            where TImplementation : TInterface;

        IFactoryMethodBindings<TFactory> Add<TImplementation>(Expression<Func<TFactory, TImplementation>> factoryMethodSelector);
    }
}