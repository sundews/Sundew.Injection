// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionDeclaration.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection
{
    public interface IInjectionDeclaration
    {
        void Configure(IInjectionBuilder injectionBuilder);
    }
}