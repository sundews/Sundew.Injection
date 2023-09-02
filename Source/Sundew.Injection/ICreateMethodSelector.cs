// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICreateMethodSelector.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection
{
    using System;
    using System.Linq.Expressions;

    public interface ICreateMethodSelector<TFactory>
    {
        ICreateMethods<TFactory> Add<TInterface, TImplementation>(Expression<Func<TFactory, TImplementation>> constructorSelector)
            where TImplementation : TInterface;

        ICreateMethods<TFactory> Add<TImplementation>(Expression<Func<TFactory, TImplementation>> constructorSelector);
    }
}