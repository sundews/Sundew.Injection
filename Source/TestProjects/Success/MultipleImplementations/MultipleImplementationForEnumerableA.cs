// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleImplementationForArrayA.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Success.MultipleImplementations;

using System;
using SuccessDependency;

public class MultipleImplementationForEnumerableA : IMultipleImplementationForEnumerable
{
    public MultipleImplementationForEnumerableA()
    {
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
    }
}