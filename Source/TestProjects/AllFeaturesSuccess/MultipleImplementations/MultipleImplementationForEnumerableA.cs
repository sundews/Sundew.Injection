// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleImplementationForArrayA.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AllFeaturesSuccess.MultipleImplementations;

using System;

public class MultipleImplementationForEnumerableA : IMultipleImplementationForEnumerable
{
    public MultipleImplementationForEnumerableA()
    {
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
    }
}