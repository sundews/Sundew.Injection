namespace AllFeaturesSuccess.RequiredInterface;

using System;
using AllFeaturesSuccessDependency;

public record RequiredParameter : IPrint
{
    public RequiredParameter(string name)
    {
        this.Name = name;
        this.Id = FactoryLifetime.Created(this);
    }

    public string Name { get; }

    public int Id { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name + @" - " + this.Name);
    }
}