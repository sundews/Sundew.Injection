namespace AllFeaturesSuccess.RequiredInterface;

using System;

public record RequiredParameter(string Name) : IPrint
{
    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name + @" - " + this.Name);
    }
}