//HintName: Sundew.Injection.Disposal.CleanOperation.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CleanOperation.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Disposal
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