﻿namespace OverallSuccess.NestingTypes;

using System;
using OverallSuccessDependency;

public class NestedConsumer : IPrint
{
    public NestedConsumer(Nestee.Nested nested)
    {
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
    }
}