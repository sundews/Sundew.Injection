namespace AllFeaturesSuccess.NestingTypes;

using System;

public class NestedConsumer : IPrint
{
    public NestedConsumer(Nestee.Nested nested)
    {

    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
    }
}