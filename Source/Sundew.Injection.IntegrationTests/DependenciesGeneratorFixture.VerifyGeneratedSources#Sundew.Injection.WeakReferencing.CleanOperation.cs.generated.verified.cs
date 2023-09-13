//HintName: Sundew.Injection.WeakReferencing.CleanOperation.cs.generated.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CleanOperation.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Sundew.Injection.WeakReferencing
{
    using System;

    internal class CleanOperation
    {
        private readonly Action action;

        public CleanOperation(Action action)
        {
            this.action = action;
        }

        ~CleanOperation()
        {
            this.action();
        }
    }
}