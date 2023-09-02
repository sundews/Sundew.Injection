// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICreateMethods.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection
{
    public interface ICreateMethods<TFactory> : ICreateMethodSelector<TFactory>
    {
    }
}